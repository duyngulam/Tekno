using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Common.Paging;
using Tekno.Application.Order.DTOs;
using Tekno.Application.Order.Interface;
using Tekno.Domain.Order;

namespace Tekno.Application.Order.Services
{
    /// <summary>
    /// Service for handling order operations including order history and tracking
    /// </summary>
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IAppLogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get user's order history with full product details (paginated)
        /// </summary>
        public async Task<PagedResult<OrderHistoryDto>> GetUserOrderHistoryAsync(
            int userId, 
            OrderStatus? status = null,
            int page = 1, 
            int pageSize = 20)
        {
            _logger.LogInformation("Getting order history for user {UserId}, status={Status}, page={Page}", 
                userId, status, page);

            var paging = new PagingParams(page, pageSize);
            var result = await _orderRepository.GetPagedAsync(userId, status, paging);

            // Map to DTOs and enrich with product details
            var orderDtos = new List<OrderHistoryDto>();
            foreach (var order in result.Data)
            {
                var orderDto = await MapOrderToHistoryDtoAsync(order);
                orderDtos.Add(orderDto);
            }

            return new PagedResult<OrderHistoryDto>(
                orderDtos, 
                result.TotalRecords, 
                result.Page, 
                result.PageSize);
        }

        /// <summary>
        /// Get single order details by order number
        /// </summary>
        public async Task<OrderHistoryDto?> GetOrderByNumberAsync(int userId, string orderNumber)
        {
            _logger.LogInformation("Getting order {OrderNumber} for user {UserId}", orderNumber, userId);

            var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
            
            if (order == null || order.UserId != userId)
            {
                return null;
            }

            return await MapOrderToHistoryDtoAsync(order);
        }

        /// <summary>
        /// Get single order details by order ID
        /// </summary>
        public async Task<OrderHistoryDto?> GetOrderDetailsByIdAsync(int userId, int orderId)
        {
            _logger.LogInformation("Getting order {OrderId} for user {UserId}", orderId, userId);

            var order = await _orderRepository.GetByIdAsync(orderId);
            
            if (order == null || order.UserId != userId)
            {
                return null;
            }

            return await MapOrderToHistoryDtoAsync(order);
        }

        /// <summary>
        /// Complete an order and update product sold counts
        /// </summary>
        public async Task<bool> CompleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", orderId);
                return false;
            }

            // Complete the order
            order.Complete();
            await _orderRepository.UpdateAsync(order);

            // Update sold counts for all products in the order
            foreach (var item in order.Items)
            {
                try
                {
                    await _productRepository.IncrementProductSoldCountAsync(
                        item.ProductId, 
                        item.Quantity);
                    
                    _logger.LogInformation(
                        "Updated sold count for Product {ProductId}: +{Quantity} units", 
                        item.ProductId, 
                        item.Quantity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Failed to update sold count for Product {ProductId} in Order {OrderId}", 
                        item.ProductId, 
                        orderId);
                    // Continue with other products even if one fails
                }
            }

            _logger.LogInformation(
                "Order {OrderId} completed with {ItemCount} items", 
                orderId, 
                order.Items.Count);

            return true;
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        public async Task<Domain.Order.Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }

        /// <summary>
        /// Get user's orders
        /// </summary>
        public async Task<List<Domain.Order.Order>> GetUserOrdersAsync(int userId)
        {
            return await _orderRepository.GetUserOrdersAsync(userId);
        }

        /// <summary>
        /// Map Order entity to OrderHistoryDto with full product details
        /// </summary>
        private async Task<OrderHistoryDto> MapOrderToHistoryDtoAsync(Domain.Order.Order order)
        {
            var dto = new OrderHistoryDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                StatusName = GetOrderStatusName(order.Status),
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                Items = new List<OrderItemDto>()
            };

            // Map payment info if available
            if (order.Payment != null)
            {
                dto.Payment = new OrderPaymentDto
                {
                    PaymentId = order.Payment.Id,
                    TransactionId = order.Payment.TransactionId,
                    Gateway = GetPaymentGatewayName(order.Payment.Gateway),
                    Method = GetPaymentMethodName(order.Payment.Method),
                    Status = GetPaymentStatusName(order.Payment.Status),
                    Amount = order.Payment.Amount,
                    Currency = order.Payment.Currency,
                    CreatedAt = order.Payment.CreatedAt,
                    CompletedAt = order.Payment.CompletedAt,
                    ErrorMessage = order.Payment.ErrorMessage
                };
            }

            // Map delivery info if order is shipped or delivered
            if (order.Status >= OrderStatus.Shipping)
            {
                dto.Delivery = new OrderDeliveryDto
                {
                    Status = GetDeliveryStatus(order.Status),
                    TrackingNumber = order.TrackingNumber,
                    Carrier = order.ShippingCarrier,
                    ShippedAt = order.ShippedAt,
                    DeliveredAt = order.DeliveredAt
                };
            }

            // Enrich order items with product and variant details
            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                var variant = await _productRepository.GetProductVariantByIdAsync(item.VariantId);

                if (product == null || variant == null)
                {
                    _logger.LogWarning("Product {ProductId} or Variant {VariantId} not found for order {OrderId}", 
                        item.ProductId, item.VariantId, order.Id);
                    continue;
                }

                var itemDto = new OrderItemDto
                {
                    Id = item.Id,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    TotalPrice = item.TotalPrice,
                    Product = _mapper.Map<ProductSummaryDto>(product),
                    Variant = _mapper.Map<ProductVariantDto>(variant)
                };

                dto.Items.Add(itemDto);
            }

            return dto;
        }

        private string GetOrderStatusName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Pending",
                OrderStatus.Processing => "Processing",
                OrderStatus.Completed => "Completed",
                OrderStatus.Shipping => "Shipping",
                OrderStatus.Delivered => "Delivered",
                OrderStatus.Cancelled => "Cancelled",
                OrderStatus.RefundRequested => "Refund Requested",
                OrderStatus.Refunded => "Refunded",
                _ => status.ToString()
            };
        }

        private string GetDeliveryStatus(OrderStatus orderStatus)
        {
            return orderStatus switch
            {
                OrderStatus.Processing => "Preparing",
                OrderStatus.Shipping => "In Transit",
                OrderStatus.Delivered => "Delivered",
                OrderStatus.Cancelled => "Cancelled",
                OrderStatus.Refunded => "Refunded",
                _ => "Processing"
            };
        }

        private string GetPaymentGatewayName(Domain.Payment.PaymentGateway gateway)
        {
            return gateway switch
            {
                Domain.Payment.PaymentGateway.Mock => "Mock (Test)",
                Domain.Payment.PaymentGateway.Stripe => "Stripe",
                Domain.Payment.PaymentGateway.PayPal => "PayPal",
                Domain.Payment.PaymentGateway.VNPay => "VNPay",
                Domain.Payment.PaymentGateway.MoMo => "MoMo",
                Domain.Payment.PaymentGateway.ZaloPay => "ZaloPay",
                _ => gateway.ToString()
            };
        }

        private string GetPaymentMethodName(Domain.Payment.PaymentMethod method)
        {
            return method switch
            {
                Domain.Payment.PaymentMethod.CreditCard => "Credit Card",
                Domain.Payment.PaymentMethod.DebitCard => "Debit Card",
                Domain.Payment.PaymentMethod.BankTransfer => "Bank Transfer",
                Domain.Payment.PaymentMethod.EWallet => "E-Wallet",
                Domain.Payment.PaymentMethod.Cash => "Cash (COD)",
                _ => method.ToString()
            };
        }

        private string GetPaymentStatusName(Domain.Payment.PaymentStatus status)
        {
            return status switch
            {
                Domain.Payment.PaymentStatus.Pending => "Pending",
                Domain.Payment.PaymentStatus.Processing => "Processing",
                Domain.Payment.PaymentStatus.Completed => "Completed",
                Domain.Payment.PaymentStatus.Failed => "Failed",
                Domain.Payment.PaymentStatus.Refunded => "Refunded",
                Domain.Payment.PaymentStatus.Cancelled => "Cancelled",
                _ => status.ToString()
            };
        }
    }
}
