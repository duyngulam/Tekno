using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get;   set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<ProductAttribute> Attributes { get;  set; } = new List<ProductAttribute>();

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
