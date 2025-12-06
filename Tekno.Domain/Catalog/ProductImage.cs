using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class ProductImage
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public string ImageUrl { get; private set; } = string.Empty;
        public bool IsPrimary { get; private set; }
        public int SortOrder { get; private set; }

        public Product Product { get; private set; } = null!;
        
        public ProductImage() { }
        
        public ProductImage(int productId, string imageUrl, bool isPrimary, int sortOrder)
        {
            ProductId = productId;
            ImageUrl = imageUrl;
            IsPrimary = isPrimary;
            SortOrder = sortOrder;
        }

        // Methods for updating properties
        public void SetPrimary(bool isPrimary)
        {
            IsPrimary = isPrimary;
        }

        public void SetSortOrder(int sortOrder)
        {
            if (sortOrder < 0)
                throw new ArgumentException("Sort order cannot be negative", nameof(sortOrder));
            
            SortOrder = sortOrder;
        }

        public void UpdateDetails(bool? isPrimary = null, int? sortOrder = null)
        {
            if (isPrimary.HasValue)
                IsPrimary = isPrimary.Value;
            
            if (sortOrder.HasValue)
            {
                if (sortOrder.Value < 0)
                    throw new ArgumentException("Sort order cannot be negative", nameof(sortOrder));
                SortOrder = sortOrder.Value;
            }
        }
    }
}
