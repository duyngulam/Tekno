using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Order.Interface;

namespace Tekno.Application.Order.Services
{
    /// <summary>
    /// Service for handling order operations including product sold count updates
    /// </summary>
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAppLogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IAppLogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
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
            await _orderRepository.CreateAsync(order); // Update

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
        public async Task<System.Collections.Generic.List<Domain.Order.Order>> GetUserOrdersAsync(int userId)
        {
            return await _orderRepository.GetUserOrdersAsync(userId);
        }
    }
}
