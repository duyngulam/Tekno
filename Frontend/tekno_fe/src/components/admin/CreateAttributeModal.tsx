"use client";

import React, { useState, useMemo } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Plus, X } from "lucide-react";
import { createAttribute } from "@/services/categories";

interface CategoryNode {
  id: number;
  name: string;
  subCategories?: CategoryNode[];
}

interface CreateAttributeModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  categories: CategoryNode[];
  onSuccess: () => void;
}

export default function CreateAttributeModal({
  open,
  onOpenChange,
  categories,
  onSuccess,
}: CreateAttributeModalProps) {
  const [formData, setFormData] = useState({
    name: "",
    inputType: "text",
    isGlobal: false,
    categoryId: "",
    initialValues: [] as string[],
  });
  
  const [newValueInput, setNewValueInput] = useState("");
  const [loading, setLoading] = useState(false);

  // Flatten categories for select dropdown
  const flattenedCategories = useMemo(() => {
    const result: Array<{ id: number; name: string; depth: number }> = [];
    
    const traverse = (nodes: CategoryNode[], depth = 0) => {
      nodes.forEach((node) => {
        result.push({
          id: node.id,
          name: node.name,
          depth,
        });
        if (node.subCategories?.length) {
          traverse(node.subCategories, depth + 1);
        }
      });
    };

    traverse(categories);
    return result;
  }, [categories]);

  const handleAddValue = () => {
    if (!newValueInput.trim()) return;
    
    setFormData(prev => ({
      ...prev,
      initialValues: [...prev.initialValues, newValueInput.trim()]
    }));
    setNewValueInput("");
  };

  const handleRemoveValue = (index: number) => {
    setFormData(prev => ({
      ...prev,
      initialValues: prev.initialValues.filter((_, i) => i !== index)
    }));
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleAddValue();
    }
  };

  const resetForm = () => {
    setFormData({
      name: "",
      inputType: "text",
      isGlobal: false,
      categoryId: "",
      initialValues: [],
    });
    setNewValueInput("");
  };

  const handleSubmit = async () => {
    try {
      if (!formData.name.trim()) {
        alert("Tên attribute là bắt buộc!");
        return;
      }

      if (!formData.isGlobal && !formData.categoryId) {
        alert("Vui lòng chọn category!");
        return;
      }

      setLoading(true);

      await createAttribute({
        name: formData.name,
        inputType: formData.inputType,
        isGlobal: formData.isGlobal,
        categoryId: formData.isGlobal ? 0 : parseInt(formData.categoryId),
        initialValues: formData.initialValues,
      });

      alert("Tạo attribute thành công!");
      resetForm();
      onOpenChange(false);
      onSuccess();
    } catch (error) {
      console.error("Create attribute failed:", error);
      alert("Tạo attribute thất bại! " + (error as Error).message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Tạo Attribute Mới</DialogTitle>
        </DialogHeader>

        <div className="space-y-4 mt-4">
          {/* Attribute Name */}
          <div>
            <label className="text-sm font-medium block mb-1">
              Tên Attribute <span className="text-red-500">*</span>
            </label>
            <Input
              placeholder="vd: Brand, Color, Size"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            />
          </div>

          {/* Input Type */}
          <div>
            <label className="text-sm font-medium block mb-1">
              Loại Input <span className="text-red-500">*</span>
            </label>
            <select
              className="w-full border rounded-md p-2"
              value={formData.inputType}
              onChange={(e) => setFormData({ ...formData, inputType: e.target.value })}
            >
              <option value="text">Text (Nhập văn bản)</option>
              <option value="select">Select (Chọn từ danh sách)</option>
              <option value="multiselect">Multi-select (Chọn nhiều)</option>
              <option value="checkbox">Checkbox</option>
              <option value="radio">Radio</option>
            </select>
          </div>

          {/* Is Global */}
          <div className="space-y-2">
            <label className="text-sm font-medium block">Phạm vi Attribute</label>
            <div className="flex gap-4">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="radio"
                  name="isGlobal"
                  checked={!formData.isGlobal}
                  onChange={() => setFormData({ ...formData, isGlobal: false })}
                  className="w-4 h-4"
                />
                <span className="text-sm">Theo Category</span>
              </label>
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="radio"
                  name="isGlobal"
                  checked={formData.isGlobal}
                  onChange={() => setFormData({ ...formData, isGlobal: true, categoryId: "" })}
                  className="w-4 h-4"
                />
                <span className="text-sm">Global (Tất cả categories)</span>
              </label>
            </div>
          </div>

          {/* Category Selection (only if not global) */}
          {!formData.isGlobal && (
            <div>
              <label className="text-sm font-medium block mb-1">
                Chọn Category <span className="text-red-500">*</span>
              </label>
              <select
                className="w-full border rounded-md p-2"
                value={formData.categoryId}
                onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
              >
                <option value="">-- Chọn category --</option>
                {flattenedCategories.map((cat) => (
                  <option key={cat.id} value={cat.id}>
                    {"—".repeat(cat.depth)} {cat.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* Initial Values */}
          <div>
            <label className="text-sm font-medium block mb-2">
              Giá trị ban đầu (tùy chọn)
            </label>
            
            <div className="flex gap-2 mb-3">
              <Input
                placeholder="Nhập giá trị... (Enter để thêm)"
                value={newValueInput}
                onChange={(e) => setNewValueInput(e.target.value)}
                onKeyPress={handleKeyPress}
                className="flex-1"
              />
              <Button onClick={handleAddValue} size="sm" type="button">
                <Plus className="w-4 h-4 mr-1" />
                Thêm
              </Button>
            </div>

            {formData.initialValues.length > 0 && (
              <div className="flex flex-wrap gap-2 p-3 bg-gray-50 rounded-lg border">
                {formData.initialValues.map((value, index) => (
                  <div
                    key={index}
                    className="flex items-center gap-2 px-3 py-1.5 bg-white border rounded-full"
                  >
                    <span className="text-sm">{value}</span>
                    <button
                      onClick={() => handleRemoveValue(index)}
                      className="hover:bg-red-100 rounded-full p-1 transition-colors"
                      type="button"
                    >
                      <X className="w-3 h-3 text-red-600" />
                    </button>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        <div className="flex gap-2 mt-6">
          <Button 
            onClick={handleSubmit} 
            className="flex-1"
            disabled={loading}
          >
            {loading ? "Đang tạo..." : "Tạo Attribute"}
          </Button>
          <Button 
            variant="outline" 
            onClick={() => {
              resetForm();
              onOpenChange(false);
            }} 
            className="flex-1"
            disabled={loading}
          >
            Hủy
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}