"use client";

import React, { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Edit2, Trash2, X, Globe } from "lucide-react";
import AttributeValuesManager from "@/components/admin/AttributeValueManager";
import {
  getGlobalAttributes,
  updateCategoryAttribute,
  deleteCategoryAttribute,
} from "@/services/categories";

interface GlobalAttribute {
  id: number;
  name: string;
  inputType: string;
  isGlobal: boolean;
}

interface GlobalAttributesManagerProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function GlobalAttributesManager({
  open,
  onOpenChange,
}: GlobalAttributesManagerProps) {
  const [attributes, setAttributes] = useState<GlobalAttribute[]>([]);
  const [loading, setLoading] = useState(false);
  const [editingAttribute, setEditingAttribute] = useState<GlobalAttribute | null>(null);
  const [showEditForm, setShowEditForm] = useState(false);
  const [expandedAttributeId, setExpandedAttributeId] = useState<number | null>(null);
  
  const [editForm, setEditForm] = useState({
    name: "",
    inputType: "text",
  });

  useEffect(() => {
    if (open) {
      loadGlobalAttributes();
    }
  }, [open]);

  const loadGlobalAttributes = async () => {
    try {
      setLoading(true);
      const data = await getGlobalAttributes();
      setAttributes(data || []);
    } catch (error) {
      console.error("Failed to load global attributes:", error);
      alert("Không thể tải danh sách global attributes");
      setAttributes([]);
    } finally {
      setLoading(false);
    }
  };

  const resetEditForm = () => {
    setEditForm({
      name: "",
      inputType: "text",
    });
    setEditingAttribute(null);
    setShowEditForm(false);
  };

  const openEditAttribute = (attr: GlobalAttribute) => {
    setEditingAttribute(attr);
    setEditForm({
      name: attr.name,
      inputType: attr.inputType || "text",
    });
    setShowEditForm(true);
  };

  const handleUpdateAttribute = async () => {
    try {
      if (!editingAttribute) {
        alert("Chưa chọn attribute để cập nhật!");
        return;
      }

      if (!editForm.name.trim()) {
        alert("Tên attribute là bắt buộc!");
        return;
      }

      await updateCategoryAttribute(
        editingAttribute.id,
        editForm.name,
        editForm.inputType
      );

      resetEditForm();
      await loadGlobalAttributes();
      alert("Cập nhật attribute thành công!");
    } catch (error) {
      console.error("Update failed:", error);
      alert("Cập nhật attribute thất bại! " + (error as Error).message);
    }
  };

  const handleDeleteAttribute = async (attributeId: number) => {
    if (!confirm("Bạn có chắc muốn xóa global attribute này?")) return;

    try {
      await deleteCategoryAttribute(attributeId);

      if (expandedAttributeId === attributeId) {
        setExpandedAttributeId(null);
      }

      await loadGlobalAttributes();
      alert("Xóa attribute thành công!");
    } catch (error) {
      console.error("Delete failed:", error);
      alert("Xóa attribute thất bại!");
    }
  };

  const toggleValuesSection = (attributeId: number) => {
    if (expandedAttributeId === attributeId) {
      setExpandedAttributeId(null);
    } else {
      setExpandedAttributeId(attributeId);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-5xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Globe className="w-5 h-5" />
            Quản lý Global Attributes
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-6 mt-4">
          {/* Edit Form */}
          {showEditForm && editingAttribute && (
            <div className="border rounded-lg p-4 bg-gray-50">
              <div className="flex justify-between items-center mb-4">
                <h3 className="font-semibold text-lg">
                  Chỉnh sửa Attribute: {editingAttribute.name}
                </h3>
                <Button variant="ghost" size="sm" onClick={resetEditForm}>
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
                    value={editForm.name}
                    onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
                  />
                </div>

                <div>
                  <label className="text-sm font-medium block mb-1">
                    Loại Input <span className="text-red-500">*</span>
                  </label>
                  <select
                    className="w-full border rounded-md p-2 bg-gray-200 cursor-not-allowed"
                    value={editForm.inputType}
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
                <Button variant="outline" onClick={resetEditForm} className="flex-1">
                  Hủy
                </Button>
              </div>
            </div>
          )}

          {/* Attributes List */}
          <div className="space-y-3">
            <div className="flex justify-between items-center">
              <h3 className="font-semibold text-lg">Danh sách Global Attributes</h3>
              <span className="text-sm text-gray-500">
                Tổng số: {attributes.length} attributes
              </span>
            </div>

            {loading ? (
              <div className="text-center py-12">
                <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600 mx-auto mb-3" />
                <p className="text-gray-500">Đang tải...</p>
              </div>
            ) : attributes.length === 0 ? (
              <div className="text-center py-12 text-gray-500 border rounded-lg bg-gray-50">
                <Globe className="w-12 h-12 mx-auto mb-3 text-gray-400" />
                <p>Chưa có global attribute nào.</p>
                <p className="text-sm mt-1">Sử dụng nút "Create Attribute" để tạo mới.</p>
              </div>
            ) : (
              <div className="space-y-3">
                {attributes.map((attr) => (
                  <div
                    key={attr.id}
                    className="border rounded-lg p-4 bg-white shadow-sm hover:shadow-md transition-shadow"
                  >
                    <div className="flex justify-between items-start mb-3">
                      <div className="flex-1">
                        <div className="flex items-center gap-2">
                          <Globe className="w-4 h-4 text-green-600" />
                          <h4 className="font-semibold text-base">{attr.name}</h4>
                          <span className="text-xs px-2 py-0.5 bg-gray-100 text-gray-600 rounded">
                            ID: {attr.id}
                          </span>
                          <span className="text-xs px-2 py-0.5 bg-blue-100 text-blue-700 rounded">
                            {attr.inputType || "text"}
                          </span>
                          <span className="text-xs px-2 py-0.5 bg-green-100 text-green-700 rounded">
                            Global
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
      </DialogContent>
    </Dialog>
  );
}