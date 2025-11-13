using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class Brand
    {
        public int Id { get;  set; }
        public string Name { get;  set; } = string.Empty;
        public string Slug { get;  set; } = string.Empty;
        public string? Country { get;  set; }
        public string? LogoPath { get;  set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public Brand() { }

        public Brand(string name, string slug, string? country = null, string? logoPath = null)
        {
            Name = name;
            Slug = slug;
            Country = country;
            LogoPath = logoPath;
        }
    }
}
