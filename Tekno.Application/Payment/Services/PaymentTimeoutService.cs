using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tekno.Application.Cart.Interface;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Order.Interface;
using Tekno.Application.Payment.Interfaces;
using Tekno.Domain.Order;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Services
{
    /// <summary>
    /// Background service to check for timed-out payments
    /// Handles payments that have been processing for too long without callback
    /// </summary>
    public class PaymentTimeoutService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IAppLogger<PaymentTimeoutService> _logger;
        private readonly TimeSpan _paymentTimeout;

        public PaymentTimeoutService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IAppLogger<PaymentTimeoutService> logger)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _logger = logger;
            
            // Default timeout: 15 minutes (reasonable time for customer to complete payment)
            // Can be configured via environment variable
            var timeoutMinutes = int.TryParse(
                Environment.GetEnvironmentVariable("PAYMENT_TIMEOUT_MINUTES"), 
                out var minutes) ? minutes : 15;
            
            _paymentTimeout = TimeSpan.FromMinutes(timeoutMinutes);
            
            _logger.LogInformation("PaymentTimeoutService initialized with timeout: {Timeout} minutes", 
                timeoutMinutes);
        }

        /// <summary>
        /// Check for timed-out payments and mark them as failed
        /// Should be called periodically (e.g., every 5 minutes)
        /// </summary>
        public async Task CheckTimeoutsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting payment timeout check...");

                // Get all payments in Processing status
                var processingPayments = await GetProcessingPaymentsAsync();

                if (!processingPayments.Any())
                {
                    _logger.LogInformation("No processing payments found");
                    return;
                }

                _logger.LogInformation("Found {Count} processing payments to check", 
                    processingPayments.Count);

                var timedOutCount = 0;
                var cutoffTime = DateTime.UtcNow - _paymentTimeout;

                foreach (var payment in processingPayments)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Payment timeout check cancelled");
                        break;
                    }

                    // Check if payment has timed out
                    if (payment.CreatedAt < cutoffTime)
                    {
                        await MarkPaymentAsTimedOutAsync(payment);
                        timedOutCount++;
                    }
                }

                _logger.LogInformation(
                    "Payment timeout check completed: {TimedOut} out of {Total} payments marked as timed out",
                    timedOutCount, processingPayments.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during payment timeout check");
                // Don't throw - background service should continue running
            }
        }

        /// <summary>
        /// Get single payment status with timeout check
        /// Marks payment as timed out if necessary
        /// </summary>
        public async Task<Domain.Payment.Payment?> GetPaymentWithTimeoutCheckAsync(string transactionId)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
            
            if (payment == null)
            {
                return null;
            }

            // Check if payment is processing and has timed out
            if (payment.Status == PaymentStatus.Processing)
            {
                var cutoffTime = DateTime.UtcNow - _paymentTimeout;
                
                if (payment.CreatedAt < cutoffTime)
                {
                    _logger.LogWarning(
                        "Payment {TransactionId} has timed out (created: {CreatedAt}, timeout: {Timeout} minutes)",
                        transactionId, payment.CreatedAt, _paymentTimeout.TotalMinutes);
                    
                    await MarkPaymentAsTimedOutAsync(payment);
                }
            }

            return payment;
        }

        /// <summary>
        /// Check if a specific payment has timed out
        /// </summary>
        public bool IsPaymentTimedOut(Domain.Payment.Payment payment)
        {
            if (payment.Status != PaymentStatus.Processing)
            {
                return false;
            }

            var cutoffTime = DateTime.UtcNow - _paymentTimeout;
            return payment.CreatedAt < cutoffTime;
        }

        /// <summary>
        /// Get all payments currently in Processing status
        /// </summary>
        private async Task<List<Domain.Payment.Payment>> GetProcessingPaymentsAsync()
        {
            // Use paging to avoid loading too many records
            var paging = new Common.Paging.PagingParams(1, 100); // Check first 100 processing payments
            
            var result = await _paymentRepository.GetPagedAsync(
                status: PaymentStatus.Processing,
                paging: paging);

            return result.Data.ToList();
        }

        /// <summary>
        /// Mark payment as timed out (failed due to timeout)
        /// Also cancels order and restores cart items
        /// Order can be reactivated later for retry if within 24 hours
        /// </summary>
        private async Task MarkPaymentAsTimedOutAsync(Domain.Payment.Payment payment)
        {
            await using var transaction = await _paymentRepository.BeginTransactionAsync();
            
            try
            {
                var errorMessage = $"Payment timed out after {_paymentTimeout.TotalMinutes} minutes withoutReceiving callback from gateway. " +
                                   "Customer may have closed the payment window or abandoned the transaction.";
                
                // 1. Mark payment as failed
                payment.MarkAsFailed(errorMessage, gatewayResponse: null);
                await _paymentRepository.UpdateAsync(payment);

                _logger.LogWarning(
                    "Payment marked as timed out: TransactionId={TransactionId}, OrderId={OrderId}, " +
                    "CreatedAt={CreatedAt}, Age={Age} minutes",
                    payment.TransactionId, payment.OrderId, payment.CreatedAt, 
                    (DateTime.UtcNow - payment.CreatedAt).TotalMinutes);

                // 2. Cancel the order (but preserve items for potential retry)
                var order = await _orderRepository.GetByIdAsync(payment.OrderId);
                if (order != null)
                {
                    // If order is not in a final state (Delivered/Shipping/Completed), cancel it due to payment timeout
                    if (order.Status != OrderStatus.Delivered && order.Status != OrderStatus.Shipping && order.Status != OrderStatus.Completed)
                    {
                        order.Cancel("Payment timeout - customer did not complete payment within allocated time");
                        await _orderRepository.UpdateAsync(order);
                        
                        _logger.LogInformation(
                            "Order {OrderId} cancelled due to payment timeout for transaction {TransactionId}. " +
                            "Order can be reactivated for retry if customer acts within 24 hours.",
                            order.Id, payment.TransactionId);

                        // 3. Restore cart items
                        await RestoreCartItemsAsync(payment.UserId, order);

                        _logger.LogInformation(
                            "Cart items restored for user {UserId} after payment timeout for transaction {TransactionId}. " +
                            "Customer can retry payment for this order within 24 hours.",
                            payment.UserId, payment.TransactionId);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Order {OrderId} is in final status {Status}; skipping cancel for timed out payment {TransactionId}.",
                            order.Id, order.Status, payment.TransactionId);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, 
                    "Failed to mark payment as timed out: TransactionId={TransactionId}",
                    payment.TransactionId);
            }
        }

        /// <summary>
        /// Restore cart items from timed-out order
        /// </summary>
        private async Task RestoreCartItemsAsync(int userId, Domain.Order.Order order)
        {
            _logger.LogInformation("Restoring cart items for user {UserId} from timed-out order {OrderId}", 
                userId, order.Id);

            try
            {
                if (!order.Items.Any())
                {
                    _logger.LogWarning("No order items to restore for order {OrderId}", order.Id);
                    return;
                }

                // Get user's cart
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart == null)
                {
                    // Cart might have been deleted - create new one
                    _logger.LogWarning("Cart not found for user {UserId}, creating new cart", userId);
                    cart = new Domain.Cart.UserCart(userId);
                    cart = await _cartRepository.CreateAsync(cart);
                }

                // Add order items back to cart
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
                            _logger.LogInformation(
                                "Updated cart item: VariantId={VariantId}, Quantity={OldQty} -> {NewQty}", 
                                orderItem.VariantId, existingCartItem.Quantity - orderItem.Quantity, newQuantity);
                        }
                        else
                        {
                            // Add new cart item
                            cart.AddItem(orderItem.VariantId, orderItem.Quantity, orderItem.Price);
                            _logger.LogInformation(
                                "Added cart item: VariantId={VariantId}, Quantity={Quantity}, Price={Price}", 
                                orderItem.VariantId, orderItem.Quantity, orderItem.Price);
                        }
                        
                        restoredCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to restore cart item: VariantId={VariantId}", 
                            orderItem.VariantId);
                        // Continue with other items
                    }
                }

                // Save cart
                await _cartRepository.UpdateAsync(cart);

                _logger.LogInformation(
                    "Cart restored for user {UserId}: {RestoredCount}/{TotalCount} items added back", 
                    userId, restoredCount, order.Items.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore cart for user {UserId}, order {OrderId}", 
                    userId, order.Id);
                // Don't throw - cart restoration is best effort
            }
        }

        /// <summary>
        /// Get payment timeout duration
        /// </summary>
        public TimeSpan GetTimeoutDuration()
        {
            return _paymentTimeout;
        }

        /// <summary>
        /// Get statistics about timed-out payments
        /// </summary>
        public async Task<PaymentTimeoutStatistics> GetTimeoutStatisticsAsync()
        {
            try
            {
                var processingPayments = await GetProcessingPaymentsAsync();
                var cutoffTime = DateTime.UtcNow - _paymentTimeout;

                var timedOut = processingPayments.Count(p => p.CreatedAt < cutoffTime);
                var atRisk = processingPayments.Count(p => 
                    p.CreatedAt >= cutoffTime && 
                    p.CreatedAt < DateTime.UtcNow - TimeSpan.FromMinutes(_paymentTimeout.TotalMinutes * 0.8));

                return new PaymentTimeoutStatistics
                {
                    TotalProcessing = processingPayments.Count,
                    TimedOut = timedOut,
                    AtRisk = atRisk, // Within 80% of timeout
                    Healthy = processingPayments.Count - timedOut - atRisk,
                    TimeoutThreshold = _paymentTimeout,
                    CheckedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get timeout statistics");
                return new PaymentTimeoutStatistics
                {
                    CheckedAt = DateTime.UtcNow,
                    TimeoutThreshold = _paymentTimeout
                };
            }
        }
    }

    /// <summary>
    /// Statistics about payment timeouts
    /// </summary>
    public class PaymentTimeoutStatistics
    {
        /// <summary>
        /// Total payments currently in Processing status
        /// </summary>
        public int TotalProcessing { get; set; }

        /// <summary>
        /// Payments that have exceeded timeout threshold
        /// </summary>
        public int TimedOut { get; set; }

        /// <summary>
        /// Payments approaching timeout (within 80% of threshold)
        /// </summary>
        public int AtRisk { get; set; }

        /// <summary>
        /// Payments that are healthy (not close to timeout)
        /// </summary>
        public int Healthy { get; set; }

        /// <summary>
        /// Configured timeout threshold
        /// </summary>
        public TimeSpan TimeoutThreshold { get; set; }

        /// <summary>
        /// When this check was performed
        /// </summary>
        public DateTime CheckedAt { get; set; }
    }
}
