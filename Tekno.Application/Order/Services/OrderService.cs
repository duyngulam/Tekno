using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Tekno.Application.Cart.Interface;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Exceptions;
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
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ICartRepository cartRepository,
            IMapper mapper,
            IAppLogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Create pending order from cart (Step 1 of two-step checkout)
        /// Does NOT clear cart - cart is cleared when payment succeeds
        /// Shipping address will be added during payment step
        /// </summary>
        public async Task<CreateOrderResponseDto> CreateOrderFromCartAsync(int userId, CreateOrderRequestDto request)
        {
            _logger.LogInformation("Creating pending order for user {UserId}", userId);

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

            // 2. Determine which items to order
            List<Domain.Cart.CartItem> itemsToOrder;
            
            if (request.SelectedItems != null && request.SelectedItems.Any())
            {
                // Partial checkout - only selected items
                itemsToOrder = new List<Domain.Cart.CartItem>();
                
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
                    var orderItem = new Domain.Cart.CartItem(
                        cart.Id,
                        selectedItem.VariantId,
                        selectedItem.Quantity,
                        cartItem.Price
                    );
                    itemsToOrder.Add(orderItem);
                }

                _logger.LogInformation("Creating order with selected items: {SelectedCount} of {TotalCount}", 
                    itemsToOrder.Count, cart.Items.Count);
            }
            else
            {
                // Full cart order
                itemsToOrder = cart.Items.ToList();
                
                _logger.LogInformation("Creating order with all cart items: {ItemCount}", itemsToOrder.Count);
            }

            // 3. Calculate order total
            var orderTotal = itemsToOrder.Sum(item => item.Quantity * item.Price);

            // 4. Create pending order (no shipping address yet)
            var orderNumber = GenerateOrderNumber();
            var order = new Domain.Order.Order(userId, orderNumber, orderTotal, request.Note);

            var createdOrder = await _orderRepository.CreateAsync(order);

            // 5. Add order items
            foreach (var cartItem in itemsToOrder)
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

            // 6. Save order with items
            await _orderRepository.UpdateAsync(createdOrder);

            _logger.LogInformation("Created pending order {OrderNumber} (ID: {OrderId}) for user {UserId} with {ItemCount} items. Shipping address will be added during payment.",
                orderNumber, createdOrder.Id, userId, itemsToOrder.Count);

            return new CreateOrderResponseDto
            {
                OrderId = createdOrder.Id,
                OrderNumber = orderNumber,
                TotalAmount = orderTotal,
                ItemsCount = itemsToOrder.Count,
                Status = "Pending",
                Note = request.Note
            };
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

                // Map shipping address if available
                if (order.ShippingAddress != null)
                {
                    dto.Delivery.ShippingAddress = new OrderShippingAddressDto
                    {
                        RecipientName = order.ShippingAddress.RecipientName,
                        PhoneNumber = order.ShippingAddress.PhoneNumber,
                        AddressLine = order.ShippingAddress.AddressLine,
                        ProvinceCode = order.ShippingAddress.ProvinceCode,
                        ProvinceName = order.ShippingAddress.ProvinceName,
                        DistrictCode = order.ShippingAddress.DistrictCode,
                        DistrictName = order.ShippingAddress.DistrictName,
                        WardCode = order.ShippingAddress.WardCode,
                        WardName = order.ShippingAddress.WardName
                    };
                }
            }
            // Even if order is still in Processing/Pending, map shipping address if it exists
            else if (order.ShippingAddress != null)
            {
                dto.Delivery = new OrderDeliveryDto
                {
                    Status = GetDeliveryStatus(order.Status),
                    ShippingAddress = new OrderShippingAddressDto
                    {
                        RecipientName = order.ShippingAddress.RecipientName,
                        PhoneNumber = order.ShippingAddress.PhoneNumber,
                        AddressLine = order.ShippingAddress.AddressLine,
                        ProvinceCode = order.ShippingAddress.ProvinceCode,
                        ProvinceName = order.ShippingAddress.ProvinceName,
                        DistrictCode = order.ShippingAddress.DistrictCode,
                        DistrictName = order.ShippingAddress.DistrictName,
                        WardCode = order.ShippingAddress.WardCode,
                        WardName = order.ShippingAddress.WardName
                    }
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

        private static string GenerateOrderNumber()
        {
            var guidPart = Guid.NewGuid().ToString("N")[..8].ToUpper();
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{guidPart}";
        }
    }
}
