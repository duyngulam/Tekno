"use client";

import React, { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Edit2, Trash2, X } from "lucide-react";
import {
  getCategoryAttributes,
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
  const [editingAttribute, setEditingAttribute] = useState<CategoryAttribute | null>(null);
  const [showEditForm, setShowEditForm] = useState(false);
  
  const [attributeForm, setAttributeForm] = useState({
    name: "",
    inputType: "text",
  });

  // State riêng cho việc toggle values section
  const [expandedAttributeId, setExpandedAttributeId] = useState<number | null>(null);

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
      inputType: "text",
    });
    setEditingAttribute(null);
    setShowEditForm(false);
  };

  const handleUpdateAttribute = async () => {
    try {
      if (!editingAttribute) {
        alert("Chưa chọn attribute để cập nhật!");
        return;
      }

      if (!attributeForm.name.trim()) {
        alert("Tên attribute là bắt buộc!");
        return;
      }

      await updateCategoryAttribute(
        editingAttribute.id, 
        attributeForm.name,
        attributeForm.inputType
      );

      // Reset form trước khi load lại
      resetAttributeForm();
      
      // Load lại danh sách attributes
      await loadAttributes();
      alert("Cập nhật attribute thành công!");
    } catch (error) {
      alert("Cập nhật attribute thất bại! " + (error as Error).message);
    }
  };

  const handleDeleteAttribute = async (attributeId: number) => {
    if (!confirm("Bạn có chắc muốn xóa attribute này?")) return;

    try {
      await deleteCategoryAttribute(attributeId);
      await loadAttributes();
      
      // Close values section if this attribute was expanded
      if (expandedAttributeId === attributeId) {
        setExpandedAttributeId(null);
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
      inputType: (attr as any).inputType || "text",
    });
    setShowEditForm(true);
  };

  const toggleValuesSection = (attributeId: number) => {
    if (expandedAttributeId === attributeId) {
      setExpandedAttributeId(null);
    } else {
      setExpandedAttributeId(attributeId);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h2 className="text-xl font-bold">Quản lý Attributes - {categoryName}</h2>
      </div>

      {/* Edit Attribute Form */}
      {showEditForm && editingAttribute && (
        <div className="border rounded-lg p-4 bg-gray-50">
          <div className="flex justify-between items-center mb-4">
            <h3 className="font-semibold text-lg">
              Chỉnh sửa Attribute: {editingAttribute.name}
            </h3>
            <Button variant="ghost" size="sm" onClick={resetAttributeForm}>
              <X className="w-4 h-4" />
            </Button>
          </div>

          <div className="space-y-4">
            <div>
              <label className="text-sm font-medium block mb-1">
                Tên Attribute <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="vd: Brand, Color, Size"
                value={attributeForm.name}
                onChange={(e) => setAttributeForm({ ...attributeForm, name: e.target.value })}
              />
              <p className="text-xs text-gray-500 mt-1">
                Tên này sẽ hiển thị cho người dùng khi lọc hoặc xem sản phẩm
              </p>
            </div>

            <div>
              <label className="text-sm font-medium block mb-1">
                Loại Input <span className="text-red-500">*</span>
              </label>
              <select
                className="w-full border rounded-md p-2 bg-gray-200 cursor-not-allowed"
                value={attributeForm.inputType}
                disabled
              >
                <option value="text">Text (Nhập văn bản)</option>
                <option value="select">Select (Chọn từ danh sách)</option>
                <option value="multiselect">Multi-select (Chọn nhiều)</option>
                <option value="checkbox">Checkbox</option>
                <option value="radio">Radio</option>
              </select>
              <p className="text-xs text-gray-500 mt-1">
                Loại input không thể thay đổi sau khi tạo
              </p>
            </div>
          </div>

          <div className="flex gap-2 mt-4">
            <Button onClick={handleUpdateAttribute} className="flex-1">
              Cập nhật Attribute
            </Button>
            <Button variant="outline" onClick={resetAttributeForm} className="flex-1">
              Hủy
            </Button>
          </div>
        </div>
      )}

      {/* Attributes List */}
      <div className="space-y-3">
        <div className="flex justify-between items-center">
          <h3 className="font-semibold text-lg">Danh sách Attributes</h3>
          <p className="text-sm text-gray-500">
            Sử dụng nút "Create Attribute" ở trên để thêm attribute mới cho category này
          </p>
        </div>

        {loadingAttributes ? (
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto" />
          </div>
        ) : attributes.length === 0 ? (
          <div className="text-center py-8 text-gray-500 border rounded-lg bg-gray-50">
            Chưa có attribute nào. Sử dụng nút "Create Attribute" ở header để thêm!
          </div>
        ) : (
          <div className="space-y-3">
            {attributes.map((attr) => (
              <div key={attr.id} className="border rounded-lg p-4 bg-white shadow-sm hover:shadow-md transition-shadow">
                <div className="flex justify-between items-start mb-3">
                  <div className="flex-1">
                    <div className="flex items-center gap-2">
                      <h4 className="font-semibold text-base">{attr.name}</h4>
                      <span className="text-xs px-2 py-0.5 bg-gray-100 text-gray-600 rounded">
                        ID: {attr.id}
                      </span>
                      <span className="text-xs px-2 py-0.5 bg-blue-100 text-blue-700 rounded">
                        {(attr as any).inputType || "text"}
                      </span>
                    </div>
                  </div>

                  <div className="flex gap-2">
                    <Button
                      size="sm"
                      variant={expandedAttributeId === attr.id ? "default" : "outline"}
                      onClick={() => toggleValuesSection(attr.id)}
                    >
                      Values {expandedAttributeId === attr.id ? "▲" : "▼"}
                    </Button>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => openEditAttribute(attr)}
                      title="Chỉnh sửa attribute"
                    >
                      <Edit2 className="w-4 h-4" />
                    </Button>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => handleDeleteAttribute(attr.id)}
                      title="Xóa attribute"
                    >
                      <Trash2 className="w-4 h-4 text-red-600" />
                    </Button>
                  </div>
                </div>

                {/* Attribute Values Section */}
                {expandedAttributeId === attr.id && (
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