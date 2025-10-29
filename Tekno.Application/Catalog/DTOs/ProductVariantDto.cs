using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Application.Catalog.DTOs
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
    }
}

