using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    /// Admin service for order management
    /// </summary>
    public class AdminOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<AdminOrderService> _logger;

        public AdminOrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IAppLogger<AdminOrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders with pagination and filtering
        /// </summary>
        public async Task<PagedResult<AdminOrderListDto>> GetAllOrdersAsync(
            OrderStatus? status = null,
            string? search = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 20)
        {
            _logger.LogInformation(
                "Admin getting orders: status={Status}, search={Search}, page={Page}",
                status, search, page);

            var paging = new PagingParams(page, pageSize);
            var result = await _orderRepository.GetPagedAsync(null, status, paging);

            // Filter by search and date range (in-memory for now)
            var filteredData = result.Data.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredData = filteredData.Where(o =>
                    o.OrderNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    o.UserId.ToString().Contains(search));
            }

            if (startDate.HasValue)
            {
                filteredData = filteredData.Where(o => o.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                filteredData = filteredData.Where(o => o.CreatedAt <= endDate.Value.AddDays(1));
            }

            var orders = filteredData.ToList();
            var totalRecords = orders.Count;

            // Map to admin DTOs
            var orderDtos = new List<AdminOrderListDto>();
            foreach (var order in orders)
            {
                var orderDto = new AdminOrderListDto
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    UserId = order.UserId,
                    UserEmail = $"user{order.UserId}@example.com", // TODO: Get from User entity
                    Status = order.Status,
                    StatusName = GetOrderStatusName(order.Status),
                    TotalAmount = order.TotalAmount,
                    ItemsCount = order.Items.Count,
                    CreatedAt = order.CreatedAt,
                    ShippedAt = order.ShippedAt,
                    DeliveredAt = order.DeliveredAt,
                    TrackingNumber = order.TrackingNumber,
                    ShippingCarrier = order.ShippingCarrier
                };

                // Add payment info if available
                if (order.Payment != null)
                {
                    orderDto.PaymentGateway = GetPaymentGatewayName(order.Payment.Gateway);
                    orderDto.PaymentStatus = GetPaymentStatusName(order.Payment.Status);
                    orderDto.PaymentMethod = GetPaymentMethodName(order.Payment.Method);
                }

                orderDtos.Add(orderDto);
            }

            return new PagedResult<AdminOrderListDto>(
                orderDtos,
                totalRecords,
                page,
                pageSize);
        }

        /// <summary>
        /// Get order details by ID
        /// </summary>
        public async Task<AdminOrderDetailDto?> GetOrderDetailsByIdAsync(int orderId)
        {
            _logger.LogInformation("Admin getting order details: {OrderId}", orderId);

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return null;
            }

            var orderDto = new AdminOrderDetailDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserId = order.UserId,
                UserEmail = $"user{order.UserId}@example.com", // TODO: Get from User entity
                UserPhone = null, // TODO: Get from User entity
                Status = order.Status,
                StatusName = GetOrderStatusName(order.Status),
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                ShippedAt = order.ShippedAt,
                DeliveredAt = order.DeliveredAt,
                TrackingNumber = order.TrackingNumber,
                ShippingCarrier = order.ShippingCarrier,
                CustomerNote = order.CustomerNote,
                Items = new List<OrderItemDto>()
            };

            // Map shipping address if available
            if (order.ShippingAddress != null)
            {
                orderDto.ShippingAddress = new OrderAddressDto
                {
                    FullName = order.ShippingAddress.RecipientName,
                    Phone = order.ShippingAddress.PhoneNumber,
                    AddressLine = order.ShippingAddress.AddressLine,
                    Ward = order.ShippingAddress.WardName,
                    District = order.ShippingAddress.DistrictName,
                    Province = order.ShippingAddress.ProvinceName
                };
            }

            // Add payment info
            if (order.Payment != null)
            {
                orderDto.Payment = new OrderPaymentDto
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

            // Enrich order items with product details
            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                var variant = await _productRepository.GetProductVariantByIdAsync(item.VariantId);

                if (product == null || variant == null)
                {
                    _logger.LogWarning(
                        "Product {ProductId} or Variant {VariantId} not found for order {OrderId}",
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

                orderDto.Items.Add(itemDto);
            }

            return orderDto;
        }

        /// <summary>
        /// Update order status
        /// </summary>
        public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto, int adminUserId)
        {
            _logger.LogInformation(
                "Admin {AdminId} updating order {OrderId} status to {Status}",
                adminUserId, orderId, dto.Status);

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            // Update status based on target status
            switch (dto.Status)
            {
                case OrderStatus.Processing:
                    order.MarkAsProcessing();
                    break;
                case OrderStatus.Cancelled:
                    order.Cancel(dto.Note);
                    break;
                case OrderStatus.RefundRequested:
                    order.RequestRefund();
                    break;
                case OrderStatus.Refunded:
                    order.Refund();
                    break;
                default:
                    _logger.LogWarning("Invalid status update: {Status}", dto.Status);
                    return false;
            }

            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation(
                "Order {OrderId} status updated to {Status} by admin {AdminId}",
                orderId, dto.Status, adminUserId);

            return true;
        }

        /// <summary>
        /// Ship an order
        /// </summary>
        public async Task<bool> ShipOrderAsync(int orderId, ShipOrderDto dto, int adminUserId)
        {
            _logger.LogInformation(
                "Admin {AdminId} shipping order {OrderId} with tracking {TrackingNumber}",
                adminUserId, orderId, dto.TrackingNumber);

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            if (order.Status != OrderStatus.Processing)
            {
                _logger.LogWarning(
                    "Cannot ship order {OrderId} with status {Status}",
                    orderId, order.Status);
                return false;
            }

            order.Ship(dto.TrackingNumber, dto.Carrier);
            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation(
                "Order {OrderId} marked as shipped by admin {AdminId}",
                orderId, adminUserId);

            return true;
        }

        /// <summary>
        /// Mark order as delivered
        /// </summary>
        public async Task<bool> DeliverOrderAsync(int orderId, int adminUserId)
        {
            _logger.LogInformation(
                "Admin {AdminId} marking order {OrderId} as delivered",
                adminUserId, orderId);

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            if (order.Status != OrderStatus.Shipping)
            {
                _logger.LogWarning(
                    "Cannot deliver order {OrderId} with status {Status}",
                    orderId, order.Status);
                return false;
            }

            order.Deliver();
            await _orderRepository.UpdateAsync(order);

            // Update product sold counts
            foreach (var item in order.Items)
            {
                try
                {
                    await _productRepository.IncrementProductSoldCountAsync(
                        item.ProductId,
                        item.Quantity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to update sold count for Product {ProductId}",
                        item.ProductId);
                }
            }

            _logger.LogInformation(
                "Order {OrderId} marked as delivered by admin {AdminId}",
                orderId, adminUserId);

            return true;
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        public async Task<bool> CancelOrderAsync(int orderId, CancelOrderDto dto, int adminUserId)
        {
            _logger.LogInformation(
                "Admin {AdminId} cancelling order {OrderId}",
                adminUserId, orderId);

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            {
                _logger.LogWarning(
                    "Cannot cancel order {OrderId} with status {Status}",
                    orderId, order.Status);
                return false;
            }

            order.Cancel(dto.Reason);
            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation(
                "Order {OrderId} cancelled by admin {AdminId}: {Reason}",
                orderId, adminUserId, dto.Reason);

            return true;
        }

        /// <summary>
        /// Get order statistics for dashboard
        /// </summary>
        public async Task<OrderStatisticsDto> GetOrderStatisticsAsync()
        {
            _logger.LogInformation("Admin getting order statistics");

            var paging = new PagingParams(1, int.MaxValue);
            var allOrders = await _orderRepository.GetPagedAsync(null, null, paging);

            var orders = allOrders.Data.ToList();
            var now = DateTime.UtcNow;

            var statistics = new OrderStatisticsDto
            {
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
                ProcessingOrders = orders.Count(o => o.Status == OrderStatus.Processing),
                ShippingOrders = orders.Count(o => o.Status == OrderStatus.Shipping),
                DeliveredOrders = orders.Count(o => o.Status == OrderStatus.Delivered),
                CancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled),
                RefundRequestedOrders = orders.Count(o => o.Status == OrderStatus.RefundRequested),

                TotalRevenue = orders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .Sum(o => o.TotalAmount),

                TodayRevenue = orders
                    .Where(o => o.Status == OrderStatus.Delivered &&
                               o.DeliveredAt.HasValue &&
                               o.DeliveredAt.Value.Date == now.Date)
                    .Sum(o => o.TotalAmount),

                ThisMonthRevenue = orders
                    .Where(o => o.Status == OrderStatus.Delivered &&
                               o.DeliveredAt.HasValue &&
                               o.DeliveredAt.Value.Year == now.Year &&
                               o.DeliveredAt.Value.Month == now.Month)
                    .Sum(o => o.TotalAmount),

                TodayOrders = orders.Count(o => o.CreatedAt.Date == now.Date),
                ThisWeekOrders = orders.Count(o => o.CreatedAt >= now.AddDays(-7)),
                ThisMonthOrders = orders.Count(o =>
                    o.CreatedAt.Year == now.Year &&
                    o.CreatedAt.Month == now.Month)
            };

            return statistics;
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
