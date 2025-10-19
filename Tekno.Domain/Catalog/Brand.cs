using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class Brand
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public string? Country { get; private set; }
        public string? LogoPath { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public ICollection<Product> Products { get; private set; } = new List<Product>();

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
