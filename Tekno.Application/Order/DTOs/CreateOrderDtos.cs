using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tekno.Application.Cart.DTOs;

namespace Tekno.Application.Order.DTOs
{
    /// <summary>
    /// Request to create a pending order from cart (Step 1 of checkout)
    /// </summary>
    public class CreateOrderRequestDto
    {
        /// <summary>
        /// Optional customer note
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Selected items for partial checkout (optional)
        /// If null or empty, all cart items will be ordered
        /// </summary>
        public List<SelectedCartItemDto>? SelectedItems { get; set; }
    }

    /// <summary>
    /// Response after creating pending order
    /// </summary>
    public class CreateOrderResponseDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Note { get; set; }
    }
}
