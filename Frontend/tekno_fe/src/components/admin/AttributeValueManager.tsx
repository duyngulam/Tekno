"use client";

import React, { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Plus, X } from "lucide-react";
import {
  getCategoryAttributeValues,
  addCategoryAttributeValue,
  deleteCategoryAttributeValue,
  type AttributeValue,
} from "@/services/categories";

interface AttributeValuesManagerProps {
  attributeId: number;
}

export default function AttributeValuesManager({ attributeId }: AttributeValuesManagerProps) {
  const [values, setValues] = useState<AttributeValue[]>([]);
  const [newValue, setNewValue] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadValues();
  }, [attributeId]);

  const loadValues = async () => {
    try {
      setLoading(true);
      const response = await getCategoryAttributeValues(attributeId);
      
      // Response is AttributeValuesResponse, extract values array
      const valuesData = response?.values || [];
      
      console.log("Loaded attribute values:", valuesData);
      setValues(valuesData);
    } catch (error) {
      console.error("Failed to load attribute values:", error);
      setValues([]);
    } finally {
      setLoading(false);
    }
  };

  const handleAddValue = async () => {
    if (!newValue.trim()) {
      alert("Vui lòng nhập giá trị!");
      return;
    }

    try {
      await addCategoryAttributeValue(attributeId, newValue.trim());
      await loadValues();
      setNewValue("");
      alert("Thêm giá trị thành công!");
    } catch (error) {
      console.error("Add value failed:", error);
      alert("Thêm giá trị thất bại!");
    }
  };

  const handleDeleteValue = async (valueId: number, value: string) => {
    if (!confirm(`Xóa giá trị "${value}"?`)) return;

    try {
      await deleteCategoryAttributeValue(valueId, value);
      await loadValues();
      alert("Xóa giá trị thành công!");
    } catch (error) {
      console.error("Delete value failed:", error);
      alert("Xóa giá trị thất bại!");
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      handleAddValue();
    }
  };

  return (
    <div className="mt-4 pt-4 border-t bg-gray-50 rounded-lg p-4">
      <h5 className="font-semibold text-sm mb-3">Giá trị (Values)</h5>

      {/* Add new value */}
      <div className="flex gap-2 mb-4">
        <Input
          placeholder="Nhập giá trị mới... (Enter để thêm)"
          value={newValue}
          onChange={(e) => setNewValue(e.target.value)}
          onKeyPress={handleKeyPress}
          className="flex-1"
        />
        <Button onClick={handleAddValue} size="sm">
          <Plus className="w-4 h-4 mr-1" />
          Thêm
        </Button>
      </div>

      {/* Values list */}
      {loading ? (
        <div className="text-center py-4">
          <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600 mx-auto" />
        </div>
      ) : values.length === 0 ? (
        <p className="text-sm text-gray-500 text-center py-4 border rounded bg-white">
          Chưa có giá trị nào
        </p>
      ) : (
        <div className="flex flex-wrap gap-2">
          {values.map((item) => (
            <div
              key={item.id}
              className="flex items-center gap-2 px-3 py-1.5 bg-white border rounded-full hover:shadow transition-shadow"
            >
              <span className="text-sm font-medium">{item.value}</span>
              <button
                onClick={() => handleDeleteValue(item.id, item.value)}
                className="hover:bg-red-100 rounded-full p-1 transition-colors"
                title="Xóa giá trị"
              >
                <X className="w-3 h-3 text-red-600" />
              </button>
            </div>
          ))}
        </div>
      )}

      <div className="mt-3 text-xs text-gray-500">
        Tổng số: {values.length} giá trị
      </div>
    </div>
  );
}