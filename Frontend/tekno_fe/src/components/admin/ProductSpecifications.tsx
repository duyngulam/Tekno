"use client";

import { useEffect, useState } from "react";
import { Plus, Trash2, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  getCategoryAttributes,
  getCategoryAttributeValues,
  type AttributeValue,
  type AttributeValuesResponse,
} from "@/services/categories";

type ProductSpecificationsProps = {
  productId?: number;
  categoryId: number;
  initialSpecs?: ProductSpec[];
  onChange: (specs: ProductSpec[]) => void;
};

type ProductSpec = {
  attributeId: number;
  attributeName: string;
  values: string[]; // Can be from existing values or custom
};

type CategoryAttr = {
  id: number;
  name: string;
};

export default function ProductSpecifications({
  productId,
  categoryId,
  initialSpecs = [],
  onChange,
}: ProductSpecificationsProps) {
  const [categoryAttributes, setCategoryAttributes] = useState<CategoryAttr[]>([]);
  const [specifications, setSpecifications] = useState<ProductSpec[]>(initialSpecs);
  const [loading, setLoading] = useState(true);

  // Load category attributes
  useEffect(() => {
    loadCategoryAttributes();
  }, [categoryId]);

  // Sync initial specs
  useEffect(() => {
    setSpecifications(initialSpecs);
  }, [initialSpecs]);

  const loadCategoryAttributes = async () => {
    try {
      setLoading(true);
      const attrs = await getCategoryAttributes(categoryId);
      setCategoryAttributes(
        attrs.map((a: any) => ({
          id: a.id,
          name: a.name,
        }))
      );
    } catch (error) {
      console.error("Failed to load category attributes:", error);
      setCategoryAttributes([]);
    } finally {
      setLoading(false);
    }
  };

  const addSpecification = async (attributeId: number) => {
    const attr = categoryAttributes.find((a) => a.id === attributeId);
    if (!attr) return;

    // Check if already exists
    if (specifications.some((s) => s.attributeId === attributeId)) {
      alert("This attribute is already added!");
      return;
    }

    const newSpec: ProductSpec = {
      attributeId: attr.id,
      attributeName: attr.name,
      values: [],
    };

    const updated = [...specifications, newSpec];
    setSpecifications(updated);
    onChange(updated);
  };

  const removeSpecification = (attributeId: number) => {
    const updated = specifications.filter((s) => s.attributeId !== attributeId);
    setSpecifications(updated);
    onChange(updated);
  };

  const addValueToSpec = (attributeId: number, value: string) => {
    if (!value.trim()) return;

    const updated = specifications.map((spec) => {
      if (spec.attributeId === attributeId) {
        // Check duplicate
        if (spec.values.includes(value.trim())) {
          alert("This value already exists!");
          return spec;
        }
        return {
          ...spec,
          values: [...spec.values, value.trim()],
        };
      }
      return spec;
    });

    setSpecifications(updated);
    onChange(updated);
  };

  const removeValueFromSpec = (attributeId: number, valueIndex: number) => {
    const updated = specifications.map((spec) => {
      if (spec.attributeId === attributeId) {
        return {
          ...spec,
          values: spec.values.filter((_, idx) => idx !== valueIndex),
        };
      }
      return spec;
    });

    setSpecifications(updated);
    onChange(updated);
  };

  if (loading) {
    return (
      <div className="p-4 border rounded bg-gray-50">
        <p className="text-center text-gray-500">Loading attributes...</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h3 className="font-semibold text-lg">Product Specifications</h3>
      </div>

      {/* Add Attribute Dropdown */}
      <div className="flex gap-2">
        <select
          className="flex-1 border rounded p-2"
          onChange={(e) => {
            const attrId = Number(e.target.value);
            if (attrId) {
              addSpecification(attrId);
              e.target.value = ""; // Reset
            }
          }}
        >
          <option value="">-- Select attribute to add --</option>
          {categoryAttributes
            .filter((attr) => !specifications.some((s) => s.attributeId === attr.id))
            .map((attr) => (
              <option key={attr.id} value={attr.id}>
                {attr.name}
              </option>
            ))}
        </select>
      </div>

      {/* Specifications List */}
      {specifications.length === 0 ? (
        <div className="p-8 border rounded bg-gray-50 text-center text-gray-500">
          No specifications added yet. Select an attribute above to add.
        </div>
      ) : (
        <div className="space-y-4">
          {specifications.map((spec) => (
            <SpecificationItem
              key={spec.attributeId}
              spec={spec}
              onAddValue={(value) => addValueToSpec(spec.attributeId, value)}
              onRemoveValue={(idx) => removeValueFromSpec(spec.attributeId, idx)}
              onRemove={() => removeSpecification(spec.attributeId)}
            />
          ))}
        </div>
      )}
    </div>
  );
}

// Sub-component for each specification item
type SpecificationItemProps = {
  spec: ProductSpec;
  onAddValue: (value: string) => void;
  onRemoveValue: (index: number) => void;
  onRemove: () => void;
};

function SpecificationItem({
  spec,
  onAddValue,
  onRemoveValue,
  onRemove,
}: SpecificationItemProps) {
  const [availableValues, setAvailableValues] = useState<string[]>([]);
  const [customValue, setCustomValue] = useState("");
  const [loadingValues, setLoadingValues] = useState(false);

  // Load available values for this attribute
  useEffect(() => {
    loadAvailableValues();
  }, [spec.attributeId]);

  const loadAvailableValues = async () => {
    try {
      setLoadingValues(true);
      const response: AttributeValuesResponse = await getCategoryAttributeValues(
        spec.attributeId
      );
      setAvailableValues(response.values.map((v) => v.value));
    } catch (error) {
      console.error("Failed to load attribute values:", error);
      setAvailableValues([]);
    } finally {
      setLoadingValues(false);
    }
  };

  const handleAddFromDropdown = (value: string) => {
    if (value) {
      onAddValue(value);
    }
  };

  const handleAddCustom = () => {
    if (customValue.trim()) {
      onAddValue(customValue);
      setCustomValue("");
    }
  };

  return (
    <div className="border rounded p-4 bg-white">
      <div className="flex justify-between items-start mb-3">
        <h4 className="font-semibold text-base">{spec.attributeName}</h4>
        <button
          onClick={onRemove}
          className="text-red-600 hover:bg-red-50 p-1 rounded"
          title="Remove specification"
        >
          <Trash2 size={16} />
        </button>
      </div>

      {/* Add value from existing options */}
      <div className="mb-3">
        <label className="text-xs text-gray-600 block mb-1">
          Select from available values:
        </label>
        {loadingValues ? (
          <p className="text-xs text-gray-400">Loading values...</p>
        ) : (
          <select
            className="w-full border rounded p-2 text-sm"
            onChange={(e) => {
              handleAddFromDropdown(e.target.value);
              e.target.value = ""; // Reset
            }}
          >
            <option value="">-- Select value --</option>
            {availableValues
              .filter((v) => !spec.values.includes(v))
              .map((value, idx) => (
                <option key={idx} value={value}>
                  {value}
                </option>
              ))}
          </select>
        )}
      </div>

      {/* Add custom value */}
      <div className="mb-3">
        <label className="text-xs text-gray-600 block mb-1">
          Or add custom value for this product:
        </label>
        <div className="flex gap-2">
          <Input
            placeholder="Enter custom value..."
            value={customValue}
            onChange={(e) => setCustomValue(e.target.value)}
            onKeyPress={(e) => {
              if (e.key === "Enter") {
                handleAddCustom();
              }
            }}
            className="flex-1"
          />
          <Button onClick={handleAddCustom} size="sm">
            <Plus className="w-4 h-4" />
          </Button>
        </div>
      </div>

      {/* Current values */}
      <div>
        <label className="text-xs text-gray-600 block mb-2">Current values:</label>
        {spec.values.length === 0 ? (
          <p className="text-xs text-gray-400 text-center py-2">No values added</p>
        ) : (
          <div className="flex flex-wrap gap-2">
            {spec.values.map((value, idx) => (
              <div
                key={idx}
                className="flex items-center gap-2 px-3 py-1 bg-blue-50 border border-blue-200 rounded-full text-sm"
              >
                <span>{value}</span>
                <button
                  onClick={() => onRemoveValue(idx)}
                  className="hover:bg-red-100 rounded-full p-0.5"
                >
                  <X className="w-3 h-3 text-red-600" />
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}