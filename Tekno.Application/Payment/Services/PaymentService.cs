using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Tekno.Application.Cart.Interface;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Paging;
using Tekno.Application.Order.Interface;
using Tekno.Application.Payment.DTOs;
using Tekno.Application.Payment.Interfaces;
using Tekno.Application.Payment.Models;
using Tekno.Domain.Order;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Services
{
    /// <summary>
    /// Payment service - Creates orders and handles payments
    /// </summary>
    public class PaymentService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IProductRepository _productRepository;
        private readonly PaymentGatewayFactory _gatewayFactory;
        private readonly PaymentTimeoutService _timeoutService;
        private readonly IMapper _mapper;
        private readonly IAppLogger<PaymentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            IProductRepository productRepository,
            PaymentGatewayFactory gatewayFactory,
            PaymentTimeoutService timeoutService,
            IMapper mapper,
            IAppLogger<PaymentService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _productRepository = productRepository;
            _gatewayFactory = gatewayFactory;
            _timeoutService = timeoutService;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Process payment for an existing pending order (Step 2 of two-step checkout)
        /// Applies shipping address and coupon before initiating payment
        /// Supports retry for failed/timed-out payments and reactivation of cancelled orders
        /// </summary>
        public async Task<PaymentResponseDto> ProcessOrderPaymentAsync(int userId, ProcessOrderPaymentRequestDto request)
        {
            _logger.LogInformation("Processing payment for order {OrderId}, user {UserId}", request.OrderId, userId);

            // 1. Get and validate order
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                throw new NotFoundException("Order", request.OrderId);
            }

            if (order.UserId != userId)
            {
                throw new UnauthorizedAccessException("Order does not belong to the user");
            }

            // 2. Check if order is in a valid state for payment
            // Allow Pending (new order) or Cancelled (after timeout/failure - if retryable)
            if (order.Status == OrderStatus.Cancelled)
            {
                // Check if order can be reactivated
                if (!order.CanRetryPayment())
                {
                    throw new InvalidOperationException(
                        $"Cannot retry payment for this order. Order is too old or in invalid state. " +
                        $"Please create a new order. (Current status: {order.Status}, Created: {order.CreatedAt:yyyy-MM-dd HH:mm})");
                }

                _logger.LogInformation(
                    "Reactivating cancelled order {OrderId} for payment retry (cancelled at: {CreatedAt}, age: {Age} hours)",
                    order.Id, order.CreatedAt, (DateTime.UtcNow - order.CreatedAt).TotalHours);

                // Reactivate order to Pending status
                order.ReactivateForPaymentRetry();
                await _orderRepository.UpdateAsync(order);
                
                _logger.LogInformation("Order {OrderId} reactivated to Pending status", order.Id);
            }
            else if (order.Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Order is not in Pending status (current: {order.Status}). Cannot process payment.");
            }

            // 3. Check payment state and handle accordingly
            var existingPayments = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            
            // Get the most recent payment
            var latestPayment = existingPayments
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            // Check for active/completed payments
            var activePayment = existingPayments.FirstOrDefault(p => 
                p.Status == PaymentStatus.Processing || 
                p.Status == PaymentStatus.Completed);

            if (activePayment != null)
            {
                if (activePayment.Status == PaymentStatus.Completed)
                {
                    throw new InvalidOperationException(
                        $"Order already has a completed payment (TransactionId: {activePayment.TransactionId})");
                }

                // Previously we returned existing payment URL for retry. Remove retry: always create a new payment attempt.
                // Mark the existing processing payment as failed/superseded so it won't block creating a new one.
                try
                {
                    activePayment.MarkAsFailed("Superseded by a new payment attempt", null);
                    await _paymentRepository.UpdateAsync(activePayment);

                    _logger.LogInformation(
                        "Marked existing processing payment as superseded: TransactionId={TransactionId}, OrderId={OrderId}",
                        activePayment.TransactionId, activePayment.OrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to mark existing processing payment as superseded for OrderId={OrderId}", request.OrderId);
                    // Continue - we still attempt to create a new payment
                }
            }

            // 4. Log payment retry info if there were previous attempts
            if (latestPayment != null && 
                (latestPayment.Status == PaymentStatus.Failed || latestPayment.Status == PaymentStatus.Cancelled || latestPayment.Status == PaymentStatus.Processing))
            {
                _logger.LogInformation(
                    "Creating new payment for order {OrderId} after previous attempts. " +
                    "Previous payment: TransactionId={TransactionId}, Status={Status}, Error={Error}, Previous attempts: {Count}",
                    request.OrderId, latestPayment.TransactionId, latestPayment.Status, latestPayment.ErrorMessage, existingPayments.Count);
            }

            await using var transaction = await _paymentRepository.BeginTransactionAsync();

            try
            {
                // 5. Update order with latest shipping address (may have changed)
                order.SetShippingAddress(request.ShippingAddressId);

                // 6. Apply/update coupon if provided (may have changed)
                if (!string.IsNullOrWhiteSpace(request.CouponCode))
                {
                    // TODO: Validate and apply coupon discount
                    // For now, just store the coupon code
                    order.ApplyCoupon(request.CouponCode, 0); // Pass actual discount amount
                    
                    _logger.LogInformation("Coupon {CouponCode} applied to order {OrderId}", 
                        request.CouponCode, request.OrderId);
                }

                // 7. Save order updates
                await _orderRepository.UpdateAsync(order);

                // 7.1 Refresh order number for this payment attempt to ensure external gateways
                // (like VNPay) receive a fresh transaction reference. This avoids collisions when
                // previous payment attempts timed out but the order remained with the old order number.
                var newOrderNumber = GenerateOrderNumber();
                order.UpdateOrderNumber(newOrderNumber);
                await _orderRepository.UpdateAsync(order);
                _logger.LogInformation("Order number refreshed for payment attempt: {OrderId} -> {OrderNumber}", order.Id, newOrderNumber);

                // 8. Create NEW payment record (don't reuse failed/timed-out payment)
                var payment = new Domain.Payment.Payment(
                    order.Id,
                    userId,
                    request.Gateway,
                    request.Method,
                    order.TotalAmount,
                    "VND");

                var createdPayment = await _paymentRepository.CreateAsync(payment);
 
                // 9. Get payment gateway and initiate payment
                var gateway = _gatewayFactory.GetGateway(request.Gateway);
                
                var paymentRequest = new PaymentRequest
                {
                    OrderId = order.Id,
                    UserId = userId,
                    OrderNumber = order.OrderNumber,
                    Amount = order.TotalAmount,
                    Currency = "VND",
                    Method = request.Method,
                    ReturnUrl = request.ReturnUrl,
                    CallbackUrl = $"{GetBaseUrl()}/api/payment/callback"
                };

                var initResult = await gateway.InitiatePaymentAsync(paymentRequest);

                if (!initResult.Success)
                {
                    throw new InvalidOperationException($"Payment initiation failed: {initResult.ErrorMessage}");
                }

                // 10. Update payment with transaction ID
                createdPayment.MarkAsProcessing(initResult.TransactionId);
                await _paymentRepository.UpdateAsync(createdPayment);

                // 11. Commit transaction
                await transaction.CommitAsync();

                var isRetry = existingPayments.Any();
                _logger.LogInformation(
                    "Payment initiated for order {OrderNumber} (OrderId: {OrderId}), transaction {TransactionId}, " +
                    "shipping address {AddressId}, coupon {CouponCode}, isRetry: {IsRetry}, attempt: {Attempt}",
                    order.OrderNumber, order.Id, initResult.TransactionId, request.ShippingAddressId, 
                    request.CouponCode ?? "none", isRetry, existingPayments.Count + 1);

                return new PaymentResponseDto
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    PaymentId = createdPayment.Id,
                    TransactionId = initResult.TransactionId,
                    PaymentUrl = initResult.PaymentUrl,
                    PaymentToken = initResult.PaymentToken,
                    QrCodeUrl = initResult.QrCodeUrl,
                    Status = PaymentStatus.Processing,
                    TotalAmount = order.TotalAmount,
                    ItemsCount = order.Items.Count,
                    IsRetry = isRetry
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payment processing failed for order {OrderId}", request.OrderId);
                throw;
            }
        }

        /// <summary>
        /// Process payment: Create order and initiate payment (DEPRECATED - use two-step checkout)
        /// Supports partial cart checkout via SelectedItems
        /// </summary>
        [Obsolete("Use two-step checkout: CreateOrderFromCart then ProcessOrderPayment")]
        public async Task<PaymentResponseDto> ProcessPaymentAsync(int userId, PaymentRequestDto request)
        {
            // 1. Get user's cart
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                throw new ValidationException(
                    new Dictionary<string, string[]>
                    {
                        { "Cart", new[] { "Cart is empty" } }
                    });
            }

            // 2. Determine which items to checkout
            List<Domain.Cart.CartItem> itemsToCheckout;
            
            if (request.SelectedItems != null && request.SelectedItems.Any())
            {
                // Partial checkout - only selected items
                itemsToCheckout = new List<Domain.Cart.CartItem>();
                
                foreach (var selectedItem in request.SelectedItems)
                {
                    var cartItem = cart.Items.FirstOrDefault(i => i.VariantId == selectedItem.VariantId);
                    if (cartItem == null)
                    {
                        throw new ValidationException(
                            new Dictionary<string, string[]>
                            {
                                { "SelectedItems", new[] { $"Variant {selectedItem.VariantId} not found in cart" } }
                            });
                    }

                    // Validate quantity
                    if (selectedItem.Quantity > cartItem.Quantity)
                    {
                        throw new ValidationException(
                            new Dictionary<string, string[]>
                            {
                                { "SelectedItems", new[] { $"Selected quantity ({selectedItem.Quantity}) exceeds cart quantity ({cartItem.Quantity}) for variant {selectedItem.VariantId}" } }
                            });
                    }

                    // Create a temporary cart item with selected quantity
                    var checkoutItem = new Domain.Cart.CartItem(
                        cart.Id,
                        selectedItem.VariantId,
                        selectedItem.Quantity,
                        cartItem.Price
                    );
                    itemsToCheckout.Add(checkoutItem);
                }

                _logger.LogInformation(
                    "Partial checkout for user {UserId}: {SelectedCount} of {TotalCount} cart items",
                    userId, itemsToCheckout.Count, cart.Items.Count);
            }
            else
            {
                // Full checkout - all cart items
                itemsToCheckout = cart.Items.ToList();
                
                _logger.LogInformation(
                    "Full cart checkout for user {UserId}: {ItemCount} items",
                    userId, itemsToCheckout.Count);
            }

            await using var transaction = await _paymentRepository.BeginTransactionAsync();

            try
            {
                // 3. Calculate order total
                var orderTotal = itemsToCheckout.Sum(item => item.Quantity * item.Price);

                // 4. Create order
                var orderNumber = GenerateOrderNumber();
                var order = new Domain.Order.Order(userId, orderNumber, orderTotal);

                var createdOrder = await _orderRepository.CreateAsync(order);

                // 5. Add order items
                foreach (var cartItem in itemsToCheckout)
                {
                    // Get variant to find ProductId
                    var variant = await _productRepository.GetProductVariantByIdAsync(cartItem.VariantId);
                    if (variant == null)
                    {
                        throw new NotFoundException("ProductVariant", cartItem.VariantId);
                    }

                    createdOrder.AddItem(
                        variant.ProductId,
                        cartItem.VariantId,
                        cartItem.Quantity,
                        cartItem.Price
                    );
                }

                // Update order with items (EF Core will track the changes)
                // Note: In full implementation, you might need explicit save

                // 6. Create payment record
                var payment = new Domain.Payment.Payment(
                    createdOrder.Id,
                    userId,
                    request.Gateway,
                    request.Method,
                    orderTotal,
                    "VND");

                var createdPayment = await _paymentRepository.CreateAsync(payment);
 
                // 7. Get payment gateway and initiate payment
                var gateway = _gatewayFactory.GetGateway(request.Gateway);
                
                var paymentRequest = new PaymentRequest
                {
                    OrderId = createdOrder.Id,
                    UserId = userId,
                    OrderNumber = orderNumber,
                    Amount = orderTotal,
                    Currency = "VND",
                    Method = request.Method,
                    ReturnUrl = request.ReturnUrl,
                    CallbackUrl = $"{GetBaseUrl()}/api/payment/callback"
                };

                var initResult = await gateway.InitiatePaymentAsync(paymentRequest);

                if (!initResult.Success)
                {
                    throw new InvalidOperationException($"Payment initiation failed: {initResult.ErrorMessage}");
                }

                // 8. Update payment with transaction ID
                createdPayment.MarkAsProcessing(initResult.TransactionId);
                await _paymentRepository.UpdateAsync(createdPayment);

                // 9. Commit transaction
                await transaction.CommitAsync();

                // ? REMOVED: Don't clear cart here - wait for payment confirmation
                // Cart will be cleared in HandlePaymentCallbackAsync when payment succeeds

                _logger.LogInformation(
                    "Payment initiated for user {UserId}, order {OrderNumber}, transaction {TransactionId}, items: {ItemCount}",
                    userId, orderNumber, initResult.TransactionId, itemsToCheckout.Count);

                return new PaymentResponseDto
                {
                    OrderId = createdOrder.Id,
                    OrderNumber = orderNumber,
                    PaymentId = createdPayment.Id,
                    TransactionId = initResult.TransactionId,
                    PaymentUrl = initResult.PaymentUrl,
                    PaymentToken = initResult.PaymentToken,
                    QrCodeUrl = initResult.QrCodeUrl,
                    Status = PaymentStatus.Processing,
                    TotalAmount = orderTotal,
                    ItemsCount = itemsToCheckout.Count
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Payment processing failed for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Handle payment callback from gateway
        /// Clears cart, reduces stock, and increments sold count when payment succeeds
        /// Restores cart items if payment fails or times out
        /// Handles idempotency - safe to call multiple times
        /// </summary>
        public async Task<PaymentStatusDto> HandlePaymentCallbackAsync(PaymentCallbackDto callback)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(callback.TransactionId);
            if (payment == null)
            {
                throw new NotFoundException("Payment", callback.TransactionId);
            }

            // ? IDEMPOTENCY: If payment already processed (Completed or Failed), return existing status
            // DO NOT process cart clearing/restoration again
            if (payment.Status == PaymentStatus.Completed || payment.Status == PaymentStatus.Failed)
            {
                _logger.LogInformation(
                    "Payment {TransactionId} already processed with status {Status}. Returning existing status (idempotent). " +
                    "Cart operations were already performed, not repeating.",
                    callback.TransactionId, payment.Status);
                
                return _mapper.Map<PaymentStatusDto>(payment);
            }

            // Only process if status is currently Processing (prevents race conditions)
            if (payment.Status != PaymentStatus.Processing)
            {
                _logger.LogWarning(
                    "Payment {TransactionId} is not in Processing status (current: {Status}). Skipping callback processing.",
                    callback.TransactionId, payment.Status);
                
                return _mapper.Map<PaymentStatusDto>(payment);
            }

            // Get gateway and verify payment
            var gateway = _gatewayFactory.GetGateway(payment.Gateway);
            var verifyResult = await gateway.VerifyPaymentAsync(callback.TransactionId, callback.CallbackData ?? new object());

            if (!verifyResult.IsValid)
            {
                _logger.LogWarning("Invalid payment callback for transaction {TransactionId}", callback.TransactionId);
                throw new InvalidOperationException("Invalid payment callback");
            }

            // Get order to check status
            var order = await _orderRepository.GetByIdAsync(payment.OrderId);
            if (order == null)
            {
                _logger.LogError("Order {OrderId} not found for payment {TransactionId}", 
                    payment.OrderId, callback.TransactionId);
                throw new NotFoundException("Order", payment.OrderId);
            }

            await using var transaction = await _paymentRepository.BeginTransactionAsync();

            try
            {
                if (verifyResult.IsSuccessful)
                {
                    // ? PAYMENT SUCCESSFUL
                    payment.MarkAsCompleted(JsonSerializer.Serialize(verifyResult.GatewayResponse));
                    await _paymentRepository.UpdateAsync(payment);

                    // Mark order as Processing (payment received, preparing order)
                    order.MarkAsProcessing();
                    await _orderRepository.UpdateAsync(order);
                    
                    _logger.LogInformation(
                        "Payment {TransactionId} marked as Completed. Now clearing cart and updating stock for order {OrderId}...",
                        callback.TransactionId, order.Id);
                    
                    // ? Clear cart items (full or partial)
                    await ClearCartItemsAfterSuccessfulPaymentAsync(payment.UserId, order);
                    
                    // ? Reduce stock and increment sold count
                    await ReduceStockAndIncrementSoldCountAsync(order);
                    
                    _logger.LogInformation(
                        "Payment completed for transaction {TransactionId}. Order {OrderId} marked as Processing. " +
                        "Cart cleared, stock reduced, sold count updated.",
                        callback.TransactionId, order.Id);
                }
                else
                {
                    // ? PAYMENT FAILED - Mark as failed and restore cart
                    payment.MarkAsFailed(verifyResult.ErrorMessage ?? "Payment verification failed",
                        JsonSerializer.Serialize(verifyResult.GatewayResponse));
                    await _paymentRepository.UpdateAsync(payment);

                    // Mark order as Cancelled
                    order.Cancel("Payment failed");
                    await _orderRepository.UpdateAsync(order);

                    _logger.LogWarning(
                        "Payment {TransactionId} marked as Failed. Order {OrderId} cancelled. Now restoring cart items...",
                        callback.TransactionId, order.Id);

                    // Restore cart items (Business logic in Backend, not DB trigger)
                    await RestoreCartItemsAsync(payment.UserId, payment.OrderId);

                    _logger.LogWarning("Payment failed for transaction {TransactionId}: {Error}. Cart items restored.",
                        callback.TransactionId, verifyResult.ErrorMessage);
                }

                await transaction.CommitAsync();

                return _mapper.Map<PaymentStatusDto>(payment);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error handling payment callback for transaction {TransactionId}", callback.TransactionId);
                throw;
            }
        }

        /// <summary>
        /// Clear cart items after successful payment
        /// Handles both full and partial checkout
        /// </summary>
        private async Task ClearCartItemsAfterSuccessfulPaymentAsync(int userId, Domain.Order.Order order)
        {
            _logger.LogInformation("Clearing cart items for user {UserId} after successful payment for order {OrderId}", 
                userId, order.Id);

            try
            {
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart == null)
                {
                    _logger.LogWarning("Cart not found for user {UserId} - nothing to clear", userId);
                    return;
                }

                // Remove order items from cart
                int removedCount = 0;
                foreach (var orderItem in order.Items)
                {
                    try
                    {
                        var cartItem = cart.Items.FirstOrDefault(ci => ci.VariantId == orderItem.VariantId);
                        
                        if (cartItem != null)
                        {
                            if (orderItem.Quantity >= cartItem.Quantity)
                            {
                                // Remove entire item using repository method
                                await _cartRepository.RemoveItemAsync(cart.Id, orderItem.VariantId);
                                _logger.LogInformation("Removed cart item: VariantId={VariantId}", orderItem.VariantId);
                            }
                            else
                            {
                                // Decrease quantity (partial checkout) - use domain method
                                cartItem.UpdateQuantity(cartItem.Quantity - orderItem.Quantity);
                                _logger.LogInformation("Decreased cart item: VariantId={VariantId}, OldQty={OldQty}, NewQty={NewQty}", 
                                    orderItem.VariantId, cartItem.Quantity + orderItem.Quantity, cartItem.Quantity);
                            }
                            
                            removedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to remove cart item: VariantId={VariantId}", orderItem.VariantId);
                        // Continue with other items
                    }
                }

                // ? FIX: Reload cart from database to get fresh state after deletions
                // This prevents EF Core from re-inserting deleted items
                cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart != null)
                {
                    // Only update if we decreased quantities (partial checkout)
                    // If we only deleted items, no need to update
                    var hasPartialCheckout = order.Items.Any(oi => 
                        cart.Items.Any(ci => ci.VariantId == oi.VariantId));
                    
                    if (hasPartialCheckout)
                    {
                        await _cartRepository.UpdateAsync(cart);
                        _logger.LogInformation("Updated cart quantities for partial checkout");
                    }
                }

                _logger.LogInformation("Cart cleared for user {UserId}: {RemovedCount}/{TotalCount} items removed", 
                    userId, removedCount, order.Items.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cart for user {UserId}, order {OrderId}", userId, order.Id);
                // Don't throw - cart clearing failure shouldn't stop payment callback processing
            }
        }

        /// <summary>
        /// Reduce product variant stock and increment sold count for each order item
        /// Called when payment completes successfully
        /// </summary>
        private async Task ReduceStockAndIncrementSoldCountAsync(Domain.Order.Order order)
        {
            _logger.LogInformation("Reducing stock and incrementing sold count for order {OrderId}", order.Id);

            try
            {
                foreach (var orderItem in order.Items)
                {
                    try
                    {
                        // Reduce variant stock
                        var variant = await _productRepository.GetProductVariantByIdAsync(orderItem.VariantId);
                        if (variant == null)
                        {
                            _logger.LogWarning("Variant {VariantId} not found - cannot reduce stock", orderItem.VariantId);
                            continue;
                        }

                        variant.ReduceStock(orderItem.Quantity);
                        await _productRepository.UpdateProductVariantAsync(variant);
                        
                        _logger.LogInformation("Reduced stock for variant {VariantId}: Quantity={Quantity}, NewStock={NewStock}", 
                            orderItem.VariantId, orderItem.Quantity, variant.Stock);

                        // Increment product sold count
                        await _productRepository.IncrementProductSoldCountAsync(orderItem.ProductId, orderItem.Quantity);
                        
                        _logger.LogInformation("Incremented sold count for product {ProductId}: Quantity={Quantity}", 
                            orderItem.ProductId, orderItem.Quantity);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to reduce stock for variant {VariantId}", orderItem.VariantId);
                        // Don't throw - continue with other items
                        // In production, you might want to implement compensation logic here
                    }
                }

                _logger.LogInformation("Stock reduction and sold count update completed for order {OrderId}", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reduce stock for order {OrderId}", order.Id);
                // Don't throw - stock reduction failure shouldn't stop payment callback processing
                // You might want to implement a retry mechanism or manual reconciliation
            }
        }

        /// <summary>
        /// Restore cart items from failed payment order
        /// This implements the business logic in the Backend (Clean Architecture)
        /// instead of using database triggers
        /// </summary>
        private async Task RestoreCartItemsAsync(int userId, int orderId)
        {
            _logger.LogInformation("Restoring cart items for user {UserId} from order {OrderId}", userId, orderId);

            try
            {
                // 1. Get order items
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null || !order.Items.Any())
                {
                    _logger.LogWarning("No order items to restore for order {OrderId}", orderId);
                    return;
                }

                // 2. Get user's cart (cart should exist as we removed items during checkout)
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart == null)
                {
                    // Cart was deleted - log warning but continue
                    _logger.LogWarning("Cart not found for user {UserId} - cannot restore items", userId);
                    return;
                }

                // 3. Add order items back to cart
                int restoredCount = 0;
                foreach (var orderItem in order.Items)
                {
                    try
                    {
                        // Check if item already exists in cart
                        var existingCartItem = cart.Items.FirstOrDefault(ci => ci.VariantId == orderItem.VariantId);
                        
                        if (existingCartItem != null)
                        {
                            // Increase quantity
                            var newQuantity = existingCartItem.Quantity + orderItem.Quantity;
                            existingCartItem.UpdateQuantity(newQuantity);
                            _logger.LogInformation("Updated cart item: VariantId={VariantId}, Quantity={OldQty}?{NewQty}", 
                                orderItem.VariantId, existingCartItem.Quantity, newQuantity);
                        }
                        else
                        {
                            // Add new cart item
                            cart.AddItem(orderItem.VariantId, orderItem.Quantity, orderItem.Price);
                            _logger.LogInformation("Added cart item: VariantId={VariantId}, Quantity={Quantity}, Price={Price}", 
                                orderItem.VariantId, orderItem.Quantity, orderItem.Price);
                        }
                        
                        restoredCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to restore cart item: VariantId={VariantId}", orderItem.VariantId);
                        // Continue with other items
                    }
                }

                // 4. Save cart
                await _cartRepository.UpdateAsync(cart);

                _logger.LogInformation("Cart restored for user {UserId}: {RestoredCount}/{TotalCount} items added back", 
                    userId, restoredCount, order.Items.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore cart for user {UserId}, order {OrderId}", userId, orderId);
                // Don't throw - cart restoration failure shouldn't stop payment callback processing
            }
        }

        /// <summary>
        /// Get payment status (without full details)
        /// Handles timeouts - marks payment as failed and restores cart if timed out
        /// </summary>
        public async Task<PaymentStatusDto?> GetPaymentStatusAsync(string transactionId)
        {
            // Use timeout service to get payment and check for timeout
            var payment = await _timeoutService.GetPaymentWithTimeoutCheckAsync(transactionId);
            
            if (payment == null)
            {
                return null;
            }

            // If payment was just marked as timed out, restore cart items and cancel order
            if (payment.Status == PaymentStatus.Failed && 
                payment.ErrorMessage != null && 
                payment.ErrorMessage.Contains("timed out"))
            {
                _logger.LogInformation("Payment {TransactionId} timed out, restoring cart items and cancelling order", 
                    transactionId);

                // Get order and cancel it
                var order = await _orderRepository.GetByIdAsync(payment.OrderId);
                if (order != null && order.Status == OrderStatus.Pending)
                {
                    order.Cancel("Payment timeout");
                    await _orderRepository.UpdateAsync(order);
                    _logger.LogInformation("Order {OrderId} cancelled due to payment timeout", order.Id);
                }

                // Restore cart items
                await RestoreCartItemsAsync(payment.UserId, payment.OrderId);
            }

            return _mapper.Map<PaymentStatusDto>(payment);
        }

        /// <summary>
        /// Get order payment status with retry information
        /// Shows if order can be retried and provides payment history
        /// </summary>
        public async Task<OrderPaymentStatusDto> GetOrderPaymentStatusAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException("Order", orderId);
            }

            if (order.UserId != userId)
            {
                throw new UnauthorizedAccessException("Order does not belong to the user");
            }

            var payments = await _paymentRepository.GetByOrderIdAsync(orderId);
            var latestPayment = payments.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

            // Check if any payment is still processing (not timed out)
            var activePayment = payments.FirstOrDefault(p => p.Status == PaymentStatus.Processing);
            bool hasActivePayment = false;
            
            if (activePayment != null)
            {
                hasActivePayment = !_timeoutService.IsPaymentTimedOut(activePayment);
                
                if (!hasActivePayment)
                {
                    // Mark timed out payment as failed
                    activePayment.MarkAsFailed(
                        $"Payment timed out after {_timeoutService.GetTimeoutDuration().TotalMinutes} minutes", 
                        null);
                    await _paymentRepository.UpdateAsync(activePayment);
                }
            }

            var canRetry = order.CanRetryPayment();
            var orderAge = DateTime.UtcNow - order.CreatedAt;

            return new OrderPaymentStatusDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                OrderStatus = order.Status,
                OrderCreatedAt = order.CreatedAt,
                OrderAgeHours = orderAge.TotalHours,
                TotalAmount = order.TotalAmount,
                
                CanRetryPayment = canRetry,
                RetryReason = GetRetryReason(order, hasActivePayment, orderAge),
                
                PaymentAttempts = payments.Count,
                LatestPaymentStatus = latestPayment?.Status,
                LatestPaymentTransactionId = latestPayment?.TransactionId,
                LatestPaymentError = latestPayment?.ErrorMessage,
                LatestPaymentCreatedAt = latestPayment?.CreatedAt,
                
                HasActivePayment = hasActivePayment,
                
                PaymentHistory = payments.OrderByDescending(p => p.CreatedAt)
                    .Select(p => new PaymentAttemptDto
                    {
                        TransactionId = p.TransactionId,
                        Status = p.Status,
                        Gateway = p.Gateway,
                        Amount = p.Amount,
                        CreatedAt = p.CreatedAt,
                        CompletedAt = p.CompletedAt,
                        FailedAt = p.FailedAt,
                        ErrorMessage = p.ErrorMessage
                    }).ToList()
            };
        }

        private string GetRetryReason(Domain.Order.Order order, bool hasActivePayment, TimeSpan orderAge)
        {
            if (hasActivePayment)
            {
                return "Payment is currently processing. Please complete the payment or wait for timeout.";
            }

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Processing)
            {
                return "Order already paid and processing.";
            }

            if (order.Status == OrderStatus.Cancelled && orderAge.TotalHours > 24)
            {
                return $"Order is too old ({orderAge.TotalHours:F1} hours). Please create a new order.";
            }

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Pending)
            {
                return "You can retry payment for this order.";
            }

            return $"Order status is {order.Status}, retry not available.";
        }

        private static string GenerateOrderNumber()
        {
            var guidPart = Guid.NewGuid().ToString("N")[..8].ToUpper();
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{guidPart}";
        }

        private string GetBaseUrl()
        {
            // Get the actual base URL from the current HTTP request
            var request = _httpContextAccessor.HttpContext?.Request;
            
            if (request != null)
            {
                var scheme = request.Scheme; // http or https
                var host = request.Host.Value; // localhost:5000 or localhost:7145
                var baseUrl = $"{scheme}://{host}";
                
                _logger.LogInformation("Using base URL from request: {BaseUrl}", baseUrl);
                return baseUrl;
            }
            
            // Fallback to environment variable or default
            var envBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
            if (!string.IsNullOrEmpty(envBaseUrl))
            {
                _logger.LogInformation("Using base URL from environment: {BaseUrl}", envBaseUrl);
                return envBaseUrl;
            }
            
            // Last resort fallback
            _logger.LogWarning("Using fallback base URL: http://localhost:5000");
            return "http://localhost:5000";
        }
    }
}
