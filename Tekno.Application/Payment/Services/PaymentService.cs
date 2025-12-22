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
        /// Process payment: Create order and initiate payment
        /// Supports partial cart checkout via SelectedItems
        /// </summary>
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
        /// Restores cart items if payment fails
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
            if (payment.Status == PaymentStatus.Completed || payment.Status == PaymentStatus.Failed)
            {
                _logger.LogInformation(
                    "Payment {TransactionId} already processed with status {Status}. Returning existing status (idempotent).",
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

            await using var transaction = await _paymentRepository.BeginTransactionAsync();

            try
            {
                if (verifyResult.IsSuccessful)
                {
                    // ? PAYMENT SUCCESSFUL
                    payment.MarkAsCompleted(JsonSerializer.Serialize(verifyResult.GatewayResponse));
                    await _paymentRepository.UpdateAsync(payment);

                    // Mark order as Processing (payment received, preparing order)
                    var order = await _orderRepository.GetByIdAsync(payment.OrderId);
                    if (order != null)
                    {
                        order.MarkAsProcessing();
                        await _orderRepository.UpdateAsync(order);
                        
                        // ? NEW: Clear cart items (full or partial)
                        await ClearCartItemsAfterSuccessfulPaymentAsync(payment.UserId, order);
                        
                        // ? NEW: Reduce stock and increment sold count
                        await ReduceStockAndIncrementSoldCountAsync(order);
                        
                        _logger.LogInformation(
                            "Payment completed for transaction {TransactionId}. Order {OrderId} marked as Processing. Cart cleared, stock reduced, sold count updated.",
                            callback.TransactionId, order.Id);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Payment completed for transaction {TransactionId} but order {OrderId} not found.",
                            callback.TransactionId, payment.OrderId);
                    }
                }
                else
                {
                    // ? PAYMENT FAILED - Mark as failed and restore cart
                    payment.MarkAsFailed(verifyResult.ErrorMessage ?? "Payment verification failed",
                        JsonSerializer.Serialize(verifyResult.GatewayResponse));
                    await _paymentRepository.UpdateAsync(payment);

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
                                // Remove entire item
                                await _cartRepository.RemoveItemAsync(cart.Id, orderItem.VariantId);
                                _logger.LogInformation("Removed cart item: VariantId={VariantId}", orderItem.VariantId);
                            }
                            else
                            {
                                // Decrease quantity (partial checkout)
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

                // Save cart
                await _cartRepository.UpdateAsync(cart);

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
        /// </summary>
        public async Task<PaymentStatusDto?> GetPaymentStatusAsync(string transactionId)
        {
            // Use timeout service to get payment and check for timeout
            var payment = await _timeoutService.GetPaymentWithTimeoutCheckAsync(transactionId);
            
            if (payment == null)
            {
                return null;
            }

            // If payment was just marked as timed out, restore cart items
            if (payment.Status == PaymentStatus.Failed && 
                payment.ErrorMessage != null && 
                payment.ErrorMessage.Contains("timed out"))
            {
                _logger.LogInformation("Payment {TransactionId} timed out, restoring cart items", transactionId);
                await RestoreCartItemsAsync(payment.UserId, payment.OrderId);
            }

            return _mapper.Map<PaymentStatusDto>(payment);
        }

        /// <summary>
        /// Get user's completed payment history (for support/verification)
        /// Returns lightweight payment info without order details
        /// </summary>
        public async Task<PagedResult<PaymentStatusDto>> GetUserCompletedPaymentsAsync(int userId, int page = 1, int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);
            
            // Only get completed payments
            var result = await _paymentRepository.GetPagedAsync(
                userId: userId, 
                status: PaymentStatus.Completed,
                paging: paging);

            var dtos = _mapper.Map<List<PaymentStatusDto>>(result.Data);
            
            // Don't enrich with order details - use /api/orders/history for that
            // This keeps payment history lightweight and focused on payment verification
            
            return new PagedResult<PaymentStatusDto>(dtos, result.TotalRecords, result.Page, result.PageSize);
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
