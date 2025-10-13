using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class ProductVariantAttribute
    {
        public int VariantId { get; set; }
        public int AttributeId { get;  set; }
        public int ValueId { get;  set; }

        public ProductVariant Variant { get;  set; } = null!;
        public ProductAttribute Attribute { get;  set; } = null!;
        public AttributeValue Value { get;  set; } = null!;
        public ProductVariantAttribute() { }
        public ProductVariantAttribute(int variantId, int attributeId, int valueId)
        {
            VariantId = variantId;
            AttributeId = attributeId;
            ValueId = valueId;
        }
    }
}
