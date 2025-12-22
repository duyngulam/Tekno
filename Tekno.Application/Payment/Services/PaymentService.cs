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

                // 10. Remove checked out items from cart
                if (request.SelectedItems != null && request.SelectedItems.Any())
                {
                    // Partial checkout - remove only selected items
                    foreach (var selectedItem in request.SelectedItems)
                    {
                        var cartItem = cart.Items.First(i => i.VariantId == selectedItem.VariantId);
                        
                        if (selectedItem.Quantity == cartItem.Quantity)
                        {
                            // Remove entire item
                            await _cartRepository.RemoveItemAsync(cart.Id, selectedItem.VariantId);
                        }
                        else
                        {
                            // Decrease quantity
                            cartItem.UpdateQuantity(cartItem.Quantity - selectedItem.Quantity);
                        }
                    }
                    
                    await _cartRepository.UpdateAsync(cart);
                    
                    _logger.LogInformation(
                        "Removed {Count} selected items from cart for user {UserId}",
                        request.SelectedItems.Count, userId);
                }
                else
                {
                    // Full checkout - clear entire cart
                    cart.Clear();
                    await _cartRepository.UpdateAsync(cart);
                    
                    _logger.LogInformation(
                        "Cleared entire cart for user {UserId} after checkout",
                        userId);
                }

                _logger.LogInformation(
                    "Payment processed successfully for user {UserId}, order {OrderNumber}, transaction {TransactionId}, items: {ItemCount}",
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
        /// Restores cart items if payment fails
        /// </summary>
        public async Task<PaymentStatusDto> HandlePaymentCallbackAsync(PaymentCallbackDto callback)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(callback.TransactionId);
            if (payment == null)
            {
                throw new NotFoundException("Payment", callback.TransactionId);
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

                    // Mark order as completed
                    var order = await _orderRepository.GetByIdAsync(payment.OrderId);
                    if (order != null)
                    {
                        order.Complete();
                        // Note: IOrderRepository doesn't have UpdateAsync, we'll skip this
                        // In full implementation, add UpdateAsync to IOrderRepository
                    }

                    _logger.LogInformation("Payment completed for transaction {TransactionId}", callback.TransactionId);
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
        /// Get payment status with full order details including products and variants
        /// </summary>
        public async Task<PaymentStatusDto?> GetPaymentStatusWithDetailsAsync(string transactionId)
        {
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

            var dto = _mapper.Map<PaymentStatusDto>(payment);
            
            // Enrich with full order details
            await EnrichPaymentWithOrderDetailsAsync(dto);
            
            return dto;
        }

        /// <summary>
        /// Get user's payment history with pagination
        /// </summary>
        public async Task<PagedResult<PaymentStatusDto>> GetUserPaymentsAsync(int userId, int page = 1, int pageSize = 20)
        {
            var paging = new PagingParams(page, pageSize);
            var result = await _paymentRepository.GetPagedAsync(userId: userId, paging: paging);

            var dtos = _mapper.Map<List<PaymentStatusDto>>(result.Data);
            
            // Enrich with order details (products and variants)
            foreach (var dto in dtos)
            {
                await EnrichPaymentWithOrderDetailsAsync(dto);
            }
            
            return new PagedResult<PaymentStatusDto>(dtos, result.TotalRecords, result.Page, result.PageSize);
        }

        /// <summary>
        /// Get user's completed payment history (for support/verification)
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
            
            // Don't enrich with order details for payment history (use orders API for that)
            // This keeps payment history lightweight and focused on payment verification
            
            return new PagedResult<PaymentStatusDto>(dtos, result.TotalRecords, result.Page, result.PageSize);
        }

        /// <summary>
        /// Enrich payment DTO with full order details including products and variants
        /// </summary>
        private async Task EnrichPaymentWithOrderDetailsAsync(PaymentStatusDto paymentDto)
        {
            try
            {
                // Get order with items
                var order = await _orderRepository.GetByIdAsync(paymentDto.OrderId);
                if (order == null || !order.Items.Any())
                {
                    _logger.LogWarning("Order {OrderId} not found or has no items", paymentDto.OrderId);
                    return;
                }

                // Map order to DTO
                var orderDto = _mapper.Map<OrderDetailsDto>(order);

                // Enrich each order item with product and variant details
                foreach (var item in order.Items)
                {
                    var itemDto = orderDto.Items.FirstOrDefault(i => i.Id == item.Id);
                    if (itemDto == null) continue;

                    // Get product variant with full details
                    var variant = await _productRepository.GetProductVariantByIdAsync(item.VariantId);
                    if (variant == null)
                    {
                        _logger.LogWarning("Variant {VariantId} not found for order item {ItemId}", item.VariantId, item.Id);
                        continue;
                    }

                    // Get product details
                    var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning("Product {ProductId} not found for order item {ItemId}", item.ProductId, item.Id);
                        continue;
                    }

                    // Map product to ProductSummaryDto (reuse existing mapping)
                    itemDto.Product = _mapper.Map<ProductSummaryDto>(product);

                    // Map variant to ProductVariantDto (reuse existing mapping)
                    itemDto.Variant = _mapper.Map<ProductVariantDto>(variant);
                }

                // Attach enriched order to payment DTO
                paymentDto.Order = orderDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enrich payment {PaymentId} with order details", paymentDto.PaymentId);
                // Don't throw - just log and continue without order details
            }
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
