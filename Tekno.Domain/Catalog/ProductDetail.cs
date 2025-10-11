using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class ProductDetail
    {
        public int ProductId { get; private set; }
        public string? LongDescription { get; private set; }
        public string? WarrantyInfo { get; private set; }
        public string Specs { get; private set; } = "{}"; // JSONB

        public Product Product { get; private set; } = null!;
        public ProductDetail() { }
        public ProductDetail(int productId, string specs, string? longDescription = null, string? warrantyInfo = null)
        {
            ProductId = productId;
            Specs = specs;
            LongDescription = longDescription;
            WarrantyInfo = warrantyInfo;
        }
    }
}

