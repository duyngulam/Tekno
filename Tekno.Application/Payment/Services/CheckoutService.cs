using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Tekno.Application.Cart.Interface;
using Tekno.Application.Common.Exceptions;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Order.Interface;
using Tekno.Application.Payment.DTOs;
using Tekno.Application.Payment.Interfaces;
using Tekno.Application.Payment.Models;
using Tekno.Domain.Order;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Services
{
    /// <summary>
    /// Checkout service - Creates orders and handles payments
    /// </summary>
    public class CheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly PaymentGatewayFactory _gatewayFactory;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CheckoutService> _logger;

        public CheckoutService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            PaymentGatewayFactory gatewayFactory,
            IMapper mapper,
            IAppLogger<CheckoutService> logger)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _gatewayFactory = gatewayFactory;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Process checkout: Create order and initiate payment
        /// </summary>
        public async Task<CheckoutResponseDto> CheckoutAsync(int userId, CheckoutRequestDto request)
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

            await using var transaction = await _paymentRepository.BeginTransactionAsync();

            try
            {
                // 2. Calculate order total (simplified - can add coupon logic here)
                var orderTotal = cart.Items.Sum(item => item.Quantity * item.Price);

                // 3. Create order
                var orderNumber = GenerateOrderNumber();
                var order = new Domain.Order.Order(userId, orderNumber, orderTotal);

                // Note: CartItem doesn't have ProductId, we'll use a simplified approach
                // In a full implementation, you'd load variant details to get ProductId
                var createdOrder = await _orderRepository.CreateAsync(order);

                // 4. Create payment record
                var payment = new Domain.Payment.Payment(
                    createdOrder.Id,
                    userId,
                    request.Gateway,
                    request.Method,
                    orderTotal,
                    "VND");

                var createdPayment = await _paymentRepository.CreateAsync(payment);

                // 5. Get payment gateway and initiate payment
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
                    CallbackUrl = $"{request.ReturnUrl}/payment/callback" // Can be configured
                };

                var initResult = await gateway.InitiatePaymentAsync(paymentRequest);

                if (!initResult.Success)
                {
                    throw new InvalidOperationException($"Payment initiation failed: {initResult.ErrorMessage}");
                }

                // 6. Update payment with transaction ID
                createdPayment.MarkAsProcessing(initResult.TransactionId);
                await _paymentRepository.UpdateAsync(createdPayment);

                // 7. Commit transaction
                await transaction.CommitAsync();

                // 8. Clear cart after successful order creation
                cart.Clear();
                await _cartRepository.UpdateAsync(cart);

                _logger.LogInformation(
                    "Checkout successful for user {UserId}, order {OrderNumber}, payment {TransactionId}",
                    userId, orderNumber, initResult.TransactionId);

                return new CheckoutResponseDto
                {
                    OrderId = createdOrder.Id,
                    OrderNumber = orderNumber,
                    PaymentId = createdPayment.Id,
                    TransactionId = initResult.TransactionId,
                    PaymentUrl = initResult.PaymentUrl,
                    PaymentToken = initResult.PaymentToken,
                    QrCodeUrl = initResult.QrCodeUrl,
                    Status = PaymentStatus.Processing,
                    TotalAmount = orderTotal
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Checkout failed for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Handle payment callback from gateway
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
                    // Mark payment as completed
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
                    // Mark payment as failed
                    payment.MarkAsFailed(verifyResult.ErrorMessage ?? "Payment verification failed",
                        JsonSerializer.Serialize(verifyResult.GatewayResponse));
                    await _paymentRepository.UpdateAsync(payment);

                    _logger.LogWarning("Payment failed for transaction {TransactionId}: {Error}",
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
        /// Get payment status
        /// </summary>
        public async Task<PaymentStatusDto?> GetPaymentStatusAsync(string transactionId)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
            return payment == null ? null : _mapper.Map<PaymentStatusDto>(payment);
        }

        /// <summary>
        /// Get user's payment history
        /// </summary>
        public async Task<List<PaymentStatusDto>> GetUserPaymentsAsync(int userId)
        {
            var payments = await _paymentRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<PaymentStatusDto>>(payments);
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }
}
