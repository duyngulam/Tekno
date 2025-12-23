"use client";

import { useEffect, useState } from "react";
import { Plus, Trash2, Edit2, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  getCategoryAttributes,
  getCategoryAttributeValues,
  type AttributeValuesResponse,
} from "@/services/categories";

type ProductVariantsProps = {
  productId?: number;
  basePrice?: number;
  initialVariants?: ProductVariant[];
  onChange: (variants: ProductVariant[]) => void;
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

type GlobalAttr = {
  id: number;
  name: string;
  inputType: string;
  availableValues: string[];
};

export default function ProductVariants({
  productId,
  basePrice = 0,
  initialVariants = [],
  onChange,
}: ProductVariantsProps) {
  const [globalAttributes, setGlobalAttributes] = useState<GlobalAttr[]>([]);
  const [variants, setVariants] = useState<ProductVariant[]>(initialVariants);
  const [loading, setLoading] = useState(true);
  
  const [showForm, setShowForm] = useState(false);
  const [editingVariant, setEditingVariant] = useState<ProductVariant | null>(null);
  
  const [formData, setFormData] = useState<ProductVariant>({
    sku: "",
    price: basePrice,
    stock: 0,
    status: "Active",
    attributes: [],
  });

  // Load global attributes
  useEffect(() => {
    loadGlobalAttributes();
  }, []);

  // Sync variants
  useEffect(() => {
    setVariants(initialVariants);
  }, [initialVariants]);

  const loadGlobalAttributes = async () => {
    try {
      setLoading(true);
      
      // Fetch all attributes (we'll filter global ones)
      // Note: You might need an API endpoint that returns only global attributes
      // For now, we'll use a workaround by fetching from a known global attribute category
      
      // Alternative: Create a service function to get global attributes
      // const globalAttrs = await getGlobalAttributes();
      
      // Temporary: Hardcode or fetch from specific endpoint
      // Let's assume we have a way to get global attributes
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL}/api/admin/categories/attributes/global`, {
        cache: "no-store",
      });
      
      if (!response.ok) {
        throw new Error("Failed to fetch global attributes");
      }
      
      const result = await response.json();
      const attrs = result.data || [];
      
      // Load values for each global attribute
      const attrsWithValues = await Promise.all(
        attrs.map(async (attr: any) => {
          try {
            const valuesResponse: AttributeValuesResponse = 
              await getCategoryAttributeValues(attr.id);
            return {
              id: attr.id,
              name: attr.name,
              inputType: attr.inputType,
              availableValues: valuesResponse.values.map((v) => v.value),
            };
          } catch (error) {
            console.error(`Failed to load values for attribute ${attr.id}:`, error);
            return {
              id: attr.id,
              name: attr.name,
              inputType: attr.inputType,
              availableValues: [],
            };
          }
        })
      );
      
      setGlobalAttributes(attrsWithValues);
    } catch (error) {
      console.error("Failed to load global attributes:", error);
      setGlobalAttributes([]);
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
    setFormData({ ...variant });
    setShowForm(true);
  };

  const handleSave = () => {
    // Validation
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
      // Update existing
      updated = variants.map((v) =>
        v.sku === editingVariant.sku ? formData : v
      );
    } else {
      // Add new
      // Check duplicate SKU
      if (variants.some((v) => v.sku === formData.sku)) {
        alert("SKU already exists!");
        return;
      }
      updated = [...variants, formData];
    }

    setVariants(updated);
    onChange(updated);
    setShowForm(false);
  };

  const handleDelete = (sku: string) => {
    if (!confirm("Delete this variant?")) return;

    const updated = variants.filter((v) => v.sku !== sku);
    setVariants(updated);
    onChange(updated);
  };

  const updateFormAttribute = (attributeId: number, value: string) => {
    const attr = globalAttributes.find((a) => a.id === attributeId);
    if (!attr) return;

    // Check if attribute already exists in form
    const existingIndex = formData.attributes.findIndex(
      (a) => a.attributeId === attributeId
    );

    let newAttributes: VariantAttribute[];

    if (existingIndex >= 0) {
      // Update existing
      newAttributes = formData.attributes.map((a, idx) =>
        idx === existingIndex
          ? { ...a, value }
          : a
      );
    } else {
      // Add new
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

  if (loading) {
    return (
      <div className="p-4 border rounded bg-gray-50">
        <p className="text-center text-gray-500">Loading global attributes...</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h3 className="font-semibold text-lg">Product Variants</h3>
        <Button onClick={openAddForm} disabled={globalAttributes.length === 0}>
          <Plus className="w-4 h-4 mr-2" />
          Add Variant
        </Button>
      </div>

      {globalAttributes.length === 0 && (
        <div className="p-4 border rounded bg-yellow-50">
          <p className="text-sm text-yellow-800">
            ⚠️ No global attributes found. Please create global attributes first to add variants.
          </p>
        </div>
      )}

      {/* Variants List */}
      {variants.length === 0 ? (
        <div className="p-8 border rounded bg-gray-50 text-center text-gray-500">
          No variants added yet.
        </div>
      ) : (
        <div className="space-y-3">
          {variants.map((variant, idx) => (
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
                      <p className="font-medium">{variant.status || "Active"}</p>
                    </div>
                  </div>

                  <div>
                    <span className="text-xs text-gray-600">Attributes:</span>
                    <div className="flex flex-wrap gap-2 mt-1">
                      {variant.attributes.map((attr, attrIdx) => (
                        <span
                          key={attrIdx}
                          className="px-2 py-1 bg-blue-100 text-blue-700 rounded text-xs"
                        >
                          <strong>{attr.attributeName || `Attr ${attr.attributeId}`}:</strong>{" "}
                          {attr.value}
                        </span>
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

            {/* Global Attributes */}
            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">
                Variant Attributes * (Select values for global attributes)
              </label>

              <div className="space-y-3">
                {globalAttributes.map((attr) => {
                  const currentValue =
                    formData.attributes.find((a) => a.attributeId === attr.id)
                      ?.value || "";

                  return (
                    <div key={attr.id} className="flex items-center gap-3">
                      <div className="flex-1">
                        <label className="text-xs text-gray-600 block mb-1">
                          {attr.name} ({attr.inputType})
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
                    <span
                      key={idx}
                      className="px-2 py-1 bg-blue-200 text-blue-800 rounded text-xs"
                    >
                      {attr.attributeName}: {attr.value}
                    </span>
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