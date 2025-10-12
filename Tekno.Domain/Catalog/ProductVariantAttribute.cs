using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Catalog
{
    public class ProductVariantAttribute
    {
        public int VariantId { get; private set; }
        public int AttributeId { get; private set; }
        public int ValueId { get; private set; }

        public ProductVariant Variant { get; private set; } = null!;
        public ProductAttribute Attribute { get; private set; } = null!;
        public AttributeValue Value { get; private set; } = null!;
        public ProductVariantAttribute() { }
        public ProductVariantAttribute(int variantId, int attributeId, int valueId)
        {
            VariantId = variantId;
            AttributeId = attributeId;
            ValueId = valueId;
        }
    }
}
