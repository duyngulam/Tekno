using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Tekno.Domain.Catalog
{
    public class AttributeValue
    {
        public int Id { get; private set; }
        public int AttributeId { get; private set; }
        public string Value { get; private set; } = string.Empty;

        public ProductAttribute Attribute { get; private set; } = null!;

        public AttributeValue() { }

        public AttributeValue(int attributeId, string value)
        {
            AttributeId = attributeId;
            Value = value;
        }
    }
}
