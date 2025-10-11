using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class ProductAttribute
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string InputType { get; private set; } = "select"; // select, text, number
        public bool IsGlobal { get; private set; } = false; // dùng chung nhiều category?
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Quan hệ với Category
        public int? CategoryId { get; private set; }
        public Category? Category { get; private set; }

        public ICollection<AttributeValue> Values { get; private set; } = new List<AttributeValue>();

        public ProductAttribute() { }

        public ProductAttribute(string name, string inputType, bool isGlobal = false, int? categoryId = null)
        {
            Name = name;
            InputType = inputType;
            IsGlobal = isGlobal;
            CategoryId = categoryId;
        }
    }

}

