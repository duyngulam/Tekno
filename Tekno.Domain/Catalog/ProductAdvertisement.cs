using System;

namespace Tekno.Domain.Catalog
{
    /// <summary>
    /// Simple product advertisement banner with just image and product link
    /// </summary>
    public class ProductAdvertisement
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public string ImageUrl { get; private set; } = string.Empty;
        public string Position { get; private set; } = "HomeTop";
        public int Priority { get; private set; } = 0;
        public bool IsActive { get; private set; } = true;
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Navigation property
        public Product Product { get; private set; } = null!;

        private ProductAdvertisement() { }

        public ProductAdvertisement(
            int productId,
            string imageUrl,
            string position = "HomeTop",
            int priority = 0,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL cannot be empty", nameof(imageUrl));

            ProductId = productId;
            ImageUrl = imageUrl.Trim();
            Position = position;
            Priority = priority;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL cannot be empty", nameof(imageUrl));

            ImageUrl = imageUrl.Trim();
        }

        public void UpdateProduct(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));

            ProductId = productId;
        }

        public void UpdatePosition(string position)
        {
            Position = position;
        }

        public void UpdatePriority(int priority)
        {
            Priority = priority;
        }

        public void UpdateSchedule(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool IsCurrentlyActive()
        {
            if (!IsActive) return false;

            var now = DateTime.UtcNow;

            if (StartDate.HasValue && now < StartDate.Value)
                return false;

            if (EndDate.HasValue && now > EndDate.Value)
                return false;

            return true;
        }
    }
}
