"use client";

import { useEffect, useState } from "react";
import { Plus, Trash2, Edit2, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

type ProductVariantsProps = {
  productId?: number;
  basePrice?: number;
  initialVariants?: ProductVariant[];
  onChange: (variants: ProductVariant[]) => void;
  categoryId?: number | string;
};

export type ProductVariant = {
  id?: number;
  sku: string;
  price: number;
  stock: number;
  status?: string;
  attributes: VariantAttribute[];
};

type VariantAttribute = {
  attributeId: number;
  attributeName?: string;
  value: string;
};

type Attr = {
  id: number;
  name: string;
  inputType: string;
  isGlobal: boolean;
  availableValues: string[];
  isCustom?: boolean; // Flag for custom attributes
};

export default function ProductVariants({
  productId,
  basePrice = 0,
  initialVariants = [],
  onChange,
  categoryId,
}: ProductVariantsProps) {
  const [allAttributes, setAllAttributes] = useState<Attr[]>([]);
  const [variants, setVariants] = useState<ProductVariant[]>([]);
  const [loading, setLoading] = useState(false);
  
  const [showForm, setShowForm] = useState(false);
  const [editingVariant, setEditingVariant] = useState<ProductVariant | null>(null);
  
  const [formData, setFormData] = useState<ProductVariant>({
    sku: "",
    price: basePrice,
    stock: 0,
    status: "Active",
    attributes: [],
  });

  // Custom attribute form states
  const [showCustomAttrForm, setShowCustomAttrForm] = useState(false);
  const [customAttrName, setCustomAttrName] = useState("");
  const [customAttrValues, setCustomAttrValues] = useState<string[]>([]);
  const [tempCustomValue, setTempCustomValue] = useState("");

  // ✅ Sync variants when parent changes - DEEP COPY to avoid reference issues
  useEffect(() => {
    setVariants(JSON.parse(JSON.stringify(initialVariants)));
  }, [initialVariants]);

  // Load both global and category attributes
  useEffect(() => {
    loadAllAttributes();
  }, [categoryId]);

  const loadAllAttributes = async () => {
    try {
      setLoading(true);
      
      const { getGlobalAttributes } = await import("@/services/categories");
      const globalAttrs = await getGlobalAttributes();
      
      let categoryAttrs: any[] = [];
      if (categoryId) {
        try {
          const { getCategoryAttributes } = await import("@/services/categories");
          categoryAttrs = await getCategoryAttributes(Number(categoryId));
        } catch (error) {
          console.error("Failed to load category attributes:", error);
        }
      }
      
      const combinedAttrs = [
        ...globalAttrs.map((attr: any) => ({ ...attr, isGlobal: true })),
        ...categoryAttrs.map((attr: any) => ({ ...attr, isGlobal: false }))
      ];
      
      const { getCategoryAttributeValues } = await import("@/services/categories");
      const attrsWithValues = await Promise.all(
        combinedAttrs.map(async (attr: any) => {
          try {
            const valuesResponse = await getCategoryAttributeValues(attr.id);
            return {
              id: attr.id,
              name: attr.name,
              inputType: attr.inputType || "text",
              isGlobal: attr.isGlobal,
              availableValues: valuesResponse.values.map((v: any) => v.value),
              isCustom: false,
            };
          } catch (error) {
            console.error(`Failed to load values for attribute ${attr.id}:`, error);
            return {
              id: attr.id,
              name: attr.name,
              inputType: attr.inputType || "text",
              isGlobal: attr.isGlobal,
              availableValues: [],
              isCustom: false,
            };
          }
        })
      );
      
      setAllAttributes(attrsWithValues);
    } catch (error) {
      console.error("Failed to load attributes:", error);
      setAllAttributes([]);
    } finally {
      setLoading(false);
    }
  };

  const openAddForm = () => {
    setEditingVariant(null);
    setFormData({
      sku: `SKU-${Date.now()}`,
      price: basePrice,
      stock: 0,
      status: "Active",
      attributes: [],
    });
    setShowForm(true);
  };

  const openEditForm = (variant: ProductVariant) => {
    setEditingVariant(variant);
    // ✅ Deep copy to avoid modifying original
    setFormData(JSON.parse(JSON.stringify(variant)));
    setShowForm(true);
  };

  const handleSave = () => {
    if (!formData.sku.trim()) {
      alert("SKU is required!");
      return;
    }

    if (formData.price <= 0) {
      alert("Price must be greater than 0!");
      return;
    }

    if (formData.attributes.length === 0) {
      alert("Please select at least one attribute value!");
      return;
    }

    let updated: ProductVariant[];
    
    if (editingVariant) {
      // ✅ Update existing - deep copy to avoid reference issues
      updated = variants.map((v) =>
        v.sku === editingVariant.sku ? JSON.parse(JSON.stringify(formData)) : v
      );
    } else {
      // Add new
      if (variants.some((v) => v.sku === formData.sku)) {
        alert("SKU already exists!");
        return;
      }
      // ✅ Deep copy when adding
      updated = [...variants, JSON.parse(JSON.stringify(formData))];
    }

    setVariants(updated);
    // ✅ Pass deep copy to parent
    onChange(JSON.parse(JSON.stringify(updated)));
    setShowForm(false);
  };

  const handleDelete = (sku: string) => {
    if (!confirm("Delete this variant?")) return;

    const updated = variants.filter((v) => v.sku !== sku);
    setVariants(updated);
    onChange(JSON.parse(JSON.stringify(updated)));
  };

  const updateFormAttribute = (attributeId: number, value: string) => {
    const attr = allAttributes.find((a) => a.id === attributeId);
    if (!attr) return;

    const existingIndex = formData.attributes.findIndex(
      (a) => a.attributeId === attributeId
    );

    let newAttributes: VariantAttribute[];

    if (existingIndex >= 0) {
      newAttributes = formData.attributes.map((a, idx) =>
        idx === existingIndex ? { ...a, value } : a
      );
    } else {
      newAttributes = [
        ...formData.attributes,
        {
          attributeId: attr.id,
          attributeName: attr.name,
          value,
        },
      ];
    }

    setFormData({
      ...formData,
      attributes: newAttributes,
    });
  };

  const removeFormAttribute = (attributeId: number) => {
    setFormData({
      ...formData,
      attributes: formData.attributes.filter(
        (a) => a.attributeId !== attributeId
      ),
    });
  };

  // ✅ Add custom attribute to form
  const addCustomAttributeValue = () => {
    if (!tempCustomValue.trim()) return;
    
    if (customAttrValues.includes(tempCustomValue.trim())) {
      alert("This value already exists!");
      return;
    }

    setCustomAttrValues([...customAttrValues, tempCustomValue.trim()]);
    setTempCustomValue("");
  };

  const removeCustomAttributeValue = (index: number) => {
    setCustomAttrValues(customAttrValues.filter((_, idx) => idx !== index));
  };

  const saveCustomAttribute = () => {
    if (!customAttrName.trim()) {
      alert("Please enter attribute name!");
      return;
    }

    if (customAttrValues.length === 0) {
      alert("Please add at least one value!");
      return;
    }

    // Check if already exists
    if (allAttributes.some((a) => a.name.toLowerCase() === customAttrName.trim().toLowerCase())) {
      alert("An attribute with this name already exists!");
      return;
    }

    // Create custom attribute with negative ID
    const customId = -(Date.now());

    const newAttr: Attr = {
      id: customId,
      name: customAttrName.trim(),
      inputType: "select",
      isGlobal: false,
      availableValues: [...customAttrValues],
      isCustom: true,
    };

    setAllAttributes([...allAttributes, newAttr]);

    // Reset form
    setCustomAttrName("");
    setCustomAttrValues([]);
    setTempCustomValue("");
    setShowCustomAttrForm(false);
    
    alert(`Custom attribute "${newAttr.name}" added! You can now use it in variants.`);
  };

  if (allAttributes.length === 0 && !loading) {
    return (
      <div className="space-y-4">
        <div className="flex justify-between items-center">
          <h3 className="font-semibold text-lg">Product Variants</h3>
        </div>
        <div className="p-8 border-2 border-dashed rounded-lg bg-yellow-50 text-center">
          <p className="text-yellow-800 font-medium mb-2">
            ⚠️ No global or category attributes found
          </p>
          <p className="text-sm text-yellow-700">
            Please create global attributes or category-specific attributes first.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h3 className="font-semibold text-lg">Product Variants</h3>
        <Button onClick={openAddForm}>
          <Plus className="w-4 h-4 mr-2" />
          Add Variant
        </Button>
      </div>

      {variants.length === 0 ? (
        <div className="p-8 border rounded bg-gray-50 text-center text-gray-500">
          No variants added yet. Click "Add Variant" to create one.
        </div>
      ) : (
        <div className="space-y-3">
          {variants.map((variant) => (
            <div
              key={variant.id || variant.sku}
              className="border rounded p-4 bg-white hover:shadow-md transition-shadow"
            >
              <div className="flex justify-between items-start">
                <div className="flex-1">
                  <div className="grid grid-cols-4 gap-4 mb-3">
                    <div>
                      <span className="text-xs text-gray-600">SKU:</span>
                      <p className="font-medium">{variant.sku}</p>
                    </div>
                    <div>
                      <span className="text-xs text-gray-600">Price:</span>
                      <p className="font-medium">{variant.price.toLocaleString()}đ</p>
                    </div>
                    <div>
                      <span className="text-xs text-gray-600">Stock:</span>
                      <p className="font-medium">{variant.stock}</p>
                    </div>
                    <div>
                      <span className="text-xs text-gray-600">Status:</span>
                      <p className="font-medium">{variant.status || "available"}</p>
                    </div>
                  </div>

                  <div>
                    <span className="text-xs text-gray-600">Attributes:</span>
                    <div className="flex flex-wrap gap-2 mt-1">
                      {variant.attributes?.map((attr, idx) => (
                        <div key={idx} className="px-2 py-1 bg-blue-200 text-blue-800 rounded text-xs">
                          <span>
                            {attr.attributeName || `Attr ${attr.attributeId}`}:
                          </span>
                          <span> {attr.value}</span>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>

                <div className="flex gap-2 ml-4">
                  <button
                    onClick={() => openEditForm(variant)}
                    className="p-2 text-blue-600 hover:bg-blue-50 rounded"
                    title="Edit"
                  >
                    <Edit2 size={16} />
                  </button>
                  <button
                    onClick={() => handleDelete(variant.sku)}
                    className="p-2 text-red-600 hover:bg-red-50 rounded"
                    title="Delete"
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Add/Edit Form Modal */}
      {showForm && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white w-full max-w-2xl rounded-lg shadow-lg p-6 max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-xl font-semibold">
                {editingVariant ? "Edit Variant" : "Add New Variant"}
              </h3>
              <button
                onClick={() => setShowForm(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X size={24} />
              </button>
            </div>

            {/* Basic Info */}
            <div className="grid grid-cols-3 gap-4 mb-4">
              <div>
                <label className="block text-sm font-medium mb-1">SKU *</label>
                <Input
                  value={formData.sku}
                  onChange={(e) => setFormData({ ...formData, sku: e.target.value })}
                  placeholder="e.g., PROD-001"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Price *</label>
                <Input
                  type="number"
                  value={formData.price}
                  onChange={(e) =>
                    setFormData({ ...formData, price: Number(e.target.value) })
                  }
                  placeholder="0"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Stock</label>
                <Input
                  type="number"
                  value={formData.stock}
                  onChange={(e) =>
                    setFormData({ ...formData, stock: Number(e.target.value) })
                  }
                  placeholder="0"
                />
              </div>
            </div>

            {/* Custom Attribute Form */}
            <div className="mb-4 p-4 border rounded bg-gray-50">
              {!showCustomAttrForm ? (
                <Button 
                  onClick={() => setShowCustomAttrForm(true)}
                  variant="outline"
                  className="w-full"
                  size="sm"
                >
                  <Plus className="w-4 h-4 mr-2" />
                  Add Custom Attribute
                </Button>
              ) : (
                <div className="space-y-3">
                  <div className="flex justify-between items-center mb-2">
                    <h5 className="font-medium text-sm">Create Custom Attribute</h5>
                    <button
                      onClick={() => {
                        setShowCustomAttrForm(false);
                        setCustomAttrName("");
                        setCustomAttrValues([]);
                        setTempCustomValue("");
                      }}
                      className="text-gray-500 hover:text-gray-700"
                    >
                      <X size={18} />
                    </button>
                  </div>

                  <div>
                    <label className="block text-xs font-medium mb-1">Attribute Name *</label>
                    <Input
                      placeholder="e.g., Color, Size, Material..."
                      value={customAttrName}
                      onChange={(e) => setCustomAttrName(e.target.value)}
                      size= {16}
                    />
                  </div>

                  <div>
                    <label className="block text-xs font-medium mb-1">Add Values *</label>
                    <div className="flex gap-2">
                      <Input
                        placeholder="Enter value and press Enter"
                        value={tempCustomValue}
                        onChange={(e) => setTempCustomValue(e.target.value)}
                        onKeyPress={(e) => {
                          if (e.key === "Enter") {
                            e.preventDefault();
                            addCustomAttributeValue();
                          }
                        }}
                        size= {16}
                      />
                      <Button onClick={addCustomAttributeValue} size="sm">
                        <Plus className="w-4 h-4" />
                      </Button>
                    </div>

                    {customAttrValues.length > 0 && (
                      <div className="mt-2 flex flex-wrap gap-2">
                        {customAttrValues.map((value, idx) => (
                          <div key={idx} className="flex items-center gap-1 px-2 py-1 bg-blue-50 border rounded">
                            <span className="text-sm">{value}</span>
                            <button onClick={() => removeCustomAttributeValue(idx)}>
                              <X className="w-3 h-3 text-red-600" />
                            </button>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>

                  <div className="flex justify-end gap-2">
                    <Button 
                      variant="outline" 
                      size="sm"
                      onClick={() => {
                        setShowCustomAttrForm(false);
                        setCustomAttrName("");
                        setCustomAttrValues([]);
                      }}
                    >
                      Cancel
                    </Button>
                    <Button 
                      size="sm"
                      onClick={saveCustomAttribute}
                      disabled={!customAttrName.trim() || customAttrValues.length === 0}
                    >
                      Add Attribute
                    </Button>
                  </div>
                </div>
              )}
            </div>

            {/* Attributes Selection */}
            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">
                Variant Attributes *
              </label>

              <div className="space-y-3">
                {allAttributes.map((attr) => {
                  const currentValue =
                    formData.attributes.find((a) => a.attributeId === attr.id)
                      ?.value || "";

                  return (
                    <div key={attr.id} className="flex items-center gap-3">
                      <div className="flex-1">
                        <label className="text-xs text-gray-600 block mb-1">
                          {attr.name}
                          {attr.isCustom && (
                            <span className="ml-2 text-xs bg-purple-100 text-purple-700 px-2 py-0.5 rounded">
                              Custom
                            </span>
                          )}
                        </label>
                        <select
                          className="w-full border rounded p-2"
                          value={currentValue}
                          onChange={(e) =>
                            updateFormAttribute(attr.id, e.target.value)
                          }
                        >
                          <option value="">-- Select {attr.name} --</option>
                          {attr.availableValues.map((value, idx) => (
                            <option key={idx} value={value}>
                              {value}
                            </option>
                          ))}
                        </select>
                      </div>

                      {currentValue && (
                        <button
                          onClick={() => removeFormAttribute(attr.id)}
                          className="mt-5 text-red-600 hover:bg-red-50 p-2 rounded"
                          title="Remove attribute"
                        >
                          <X size={16} />
                        </button>
                      )}
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Selected Attributes Preview */}
            {formData.attributes.length > 0 && (
              <div className="mb-4 p-3 bg-blue-50 rounded">
                <p className="text-xs text-gray-600 mb-2">Selected attributes:</p>
                <div className="flex flex-wrap gap-2">
                  {formData.attributes.map((attr, idx) => (
                    <div 
                      key={idx} 
                      className="flex items-center gap-1 px-2 py-1 bg-blue-200 text-blue-800 rounded-full text-xs"
                    >
                      <span>
                        {attr.attributeName || `Attr ${attr.attributeId}`}: {attr.value}
                      </span>
                      <button
                        onClick={() => removeFormAttribute(attr.attributeId)}
                        className="ml-1 hover:bg-red-200 rounded-full p-0.5"
                        title="Remove this attribute"
                        type="button"
                      >
                        <X className="w-3 h-3 text-red-600" />
                      </button>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Form Actions */}
            <div className="flex justify-end gap-3 mt-6">
              <Button variant="outline" onClick={() => setShowForm(false)}>
                Cancel
              </Button>
              <Button onClick={handleSave}>
                {editingVariant ? "Update Variant" : "Add Variant"}
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}