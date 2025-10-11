using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public int? ParentId { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        // Navigation
        public Category? ParentCategory { get; private set; }
        public ICollection<Category> SubCategories { get; private set; } = new List<Category>();
        public ICollection<Product> Products { get; private set; } = new List<Product>();
        public ICollection<ProductAttribute> Attributes { get; private set; } = new List<ProductAttribute>();

        private Category() { }

        public Category(string name, string slug, string? description = null, int? parentId = null)
        {
            Name = name;
            Slug = slug;
            Description = description;
            ParentId = parentId;
        }
    }
}
