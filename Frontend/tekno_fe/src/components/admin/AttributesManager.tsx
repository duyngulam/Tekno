"use client";

import React, { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Plus, Edit2, Trash2, X } from "lucide-react";
import {
  getCategoryAttributes,
  createCategoryAttribute,
  updateCategoryAttribute,
  deleteCategoryAttribute,
} from "@/services/categories";
import { CategoryAttribute } from "@/type/categories";
import AttributeValuesManager from "@/components/admin/AttributeValueManager";

interface AttributesManagerProps {
  categoryId: number;
  categoryName: string;
}

export default function AttributesManager({ categoryId, categoryName }: AttributesManagerProps) {
  const [attributes, setAttributes] = useState<CategoryAttribute[]>([]);
  const [loadingAttributes, setLoadingAttributes] = useState(false);
  const [showAddAttribute, setShowAddAttribute] = useState(false);
  const [editingAttribute, setEditingAttribute] = useState<CategoryAttribute | null>(null);
  
  const [attributeForm, setAttributeForm] = useState({
    name: "",
    value: [] as string[],
  });

  const [selectedAttributeId, setSelectedAttributeId] = useState<number | null>(null);

  useEffect(() => {
    loadAttributes();
  }, [categoryId]);

  const loadAttributes = async () => {
    try {
      setLoadingAttributes(true);
      const data = await getCategoryAttributes(categoryId);
      setAttributes(data || []);
    } catch (error) {
      console.error("Failed to load attributes:", error);
      setAttributes([]);
    } finally {
      setLoadingAttributes(false);
    }
  };

  const resetAttributeForm = () => {
    setAttributeForm({
      name: "",
        value: [],
    });
    setEditingAttribute(null);
    setShowAddAttribute(false);
  };

const handleCreateAttribute = async () => {
  try {
    if (!attributeForm.name.trim()) {
      alert("Tên attribute là bắt buộc!");
      return;
    }

    await createCategoryAttribute(categoryId, attributeForm.name);

    await loadAttributes();
    resetAttributeForm();
    alert("Tạo attribute thành công!");
  } catch (error) {
    console.error("Create attribute failed:", error);
    alert("Tạo attribute thất bại!");
  }
};


  const handleUpdateAttribute = async () => {
    try {
      if (!attributeForm.name.trim()) {
        alert("Tên và Tên hiển thị là bắt buộc!");
        return;
      }

      const fd = new FormData();
      fd.append("Name", attributeForm.name);
        fd.append("Value", String(attributeForm.value));

      await updateCategoryAttribute(editingAttribute!.id, fd);
      await loadAttributes();
      resetAttributeForm();
      alert("Cập nhật attribute thành công!");
    } catch (error) {
      console.error("Update attribute failed:", error);
      alert("Cập nhật attribute thất bại!");
    }
  };

  const handleDeleteAttribute = async (attributeId: number) => {
    if (!confirm("Bạn có chắc muốn xóa attribute này?")) return;

    try {
      await deleteCategoryAttribute(attributeId);
      await loadAttributes();
      
      // Close values section if this attribute was selected
      if (selectedAttributeId === attributeId) {
        setSelectedAttributeId(null);
      }
      
      alert("Xóa attribute thành công!");
    } catch (error) {
      console.error("Delete attribute failed:", error);
      alert("Xóa attribute thất bại!");
    }
  };

  const openEditAttribute = (attr: CategoryAttribute) => {
    setEditingAttribute(attr);
    setAttributeForm({
      name: attr.name,
      value: [],
    });
    setShowAddAttribute(true);
  };

  const toggleValuesSection = (attributeId: number) => {
    if (selectedAttributeId === attributeId) {
      setSelectedAttributeId(null);
    } else {
      setSelectedAttributeId(attributeId);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-xl font-bold">Quản lý Attributes - {categoryName}</h2>
      </div>

      {/* Add Attribute Button */}
      {!showAddAttribute && (
        <Button onClick={() => setShowAddAttribute(true)} className="w-full">
          <Plus className="w-4 h-4 mr-2" />
          Thêm Attribute Mới
        </Button>
      )}

      {/* Attribute Form */}
      {showAddAttribute && (
        <div className="border rounded-lg p-4 bg-gray-50">
          <div className="flex justify-between items-center mb-4">
            <h3 className="font-semibold text-lg">
              {editingAttribute ? "Chỉnh sửa Attribute" : "Thêm Attribute Mới"}
            </h3>
            <Button variant="ghost" size="sm" onClick={resetAttributeForm}>
              <X className="w-4 h-4" />
            </Button>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="text-sm font-medium block mb-1">
                Tên (Name) <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="vd: brand, color"
                value={attributeForm.name}
                onChange={(e) => setAttributeForm({ ...attributeForm, name: e.target.value })}
              />
            </div>

          </div>

          <div className="flex gap-2 mt-4">
            <Button
              onClick={editingAttribute ? handleUpdateAttribute : handleCreateAttribute}
              className="flex-1"
            >
              {editingAttribute ? "Cập nhật" : "Tạo Attribute"}
            </Button>
            <Button variant="outline" onClick={resetAttributeForm} className="flex-1">
              Hủy
            </Button>
          </div>
        </div>
      )}

      {/* Attributes List */}
      <div className="space-y-3">
        <h3 className="font-semibold text-lg">Danh sách Attributes</h3>

        {loadingAttributes ? (
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto" />
          </div>
        ) : attributes.length === 0 ? (
          <div className="text-center py-8 text-gray-500 border rounded-lg bg-gray-50">
            Chưa có attribute nào. Hãy thêm attribute mới!
          </div>
        ) : (
          <div className="space-y-3">
            {attributes.map((attr) => (
              <div key={attr.id} className="border rounded-lg p-4 bg-white shadow-sm">
                <div className="flex justify-between items-start mb-3">
                  <div className="flex-1">
                    <p className="text-sm text-gray-600">Name: {attr.name}</p>
                  </div>

                  <div className="flex gap-2">
                    <Button
                      size="sm"
                      variant={selectedAttributeId === attr.id ? "default" : "outline"}
                      onClick={() => toggleValuesSection(attr.id)}
                    >
                      Values {selectedAttributeId === attr.id ? "▲" : "▼"}
                    </Button>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => openEditAttribute(attr)}
                    >
                      <Edit2 className="w-4 h-4" />
                    </Button>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => handleDeleteAttribute(attr.id)}
                    >
                      <Trash2 className="w-4 h-4 text-red-600" />
                    </Button>
                  </div>
                </div>

                {/* Attribute Values Section */}
                {selectedAttributeId === attr.id && (
                  <AttributeValuesManager attributeId={attr.id} />
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}