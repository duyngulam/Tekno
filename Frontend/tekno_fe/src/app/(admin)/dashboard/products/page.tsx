"use client";

import React, { useEffect, useMemo, useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { X } from "lucide-react";
import { Input } from "@/components/ui/input";
import { getBrandList } from "@/services/brand";
import { getCategoriesList } from "@/services/categories";
import { ChevronDown, ChevronRight } from "lucide-react";
import Actions from "@/components/admin/Actions";
import Image from "next/image";
import { uploadImage, updateImageMeta, deleteImage, reorderImages, deleteVariant } from "@/lib/productsImageApi";
import { getAdminProducts, getAdminProduct, createAdminProduct, updateAdminProduct, deleteAdminProduct } from "@/services/products";
import { getCategoryAttributes } from "@/services/categories";
import AddProductVariant from "@/components/admin/AddProductVariant";
import ProductSpecifications from "@/components/admin/ProductSpecifications";
import ProductVariants from "@/components/admin/ProductVariants";

type Product = {
  id: number;
  brandName?: string;
  categoryName?: string;
  name: string;
  slug: string;
  basePrice?: number;
  discountPercent?: number;
  finalPrice?: number;
  overview?: string | null;
  totalSold?: number;
  averageRating?: number;
  totalReviews?: number;
  primaryImagePath?: string | null;
  [k: string]: any;
};

type CategoryNode = {
  id: number;
  name: string;
  subCategories?: CategoryNode[];
  parentId?: number | null;
  [k: string]: any;
};

type Brand = {
  id: number;
  name: string;
};

type ProductSpec = {
  attributeId: number;
  attributeName: string;
  values: string[];
};

type ProductVariant = {
  id?: number;
  sku: string;
  price: number;
  stock: number;
  status?: string;
  attributes: {
    attributeId: number;
    attributeName?: string;
    value: string;
  }[];
};

export default function ProductPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  const [openCreate, setOpenCreate] = useState(false);
  const [openEdit, setOpenEdit] = useState(false);

  const [treeCategories, setTreeCategories] = useState<CategoryNode[]>([]);
  const [brands, setBrands] = useState<Brand[]>([]);

  const [openDetail, setOpenDetail] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<any>(null);

  const [editImages, setEditImages] = useState<any[]>([]);
  const [newImages, setNewImages] = useState<File[]>([]);

  const [draggingIndex, setDraggingIndex] = useState<number | null>(null);

  const [createData, setCreateData] = useState({
    name: "",
    slug: "",
    categoryId: "" as string | number,
    brandId: "" as string | number,
    basePrice: 0,
    discountPercent: 0,
    overview: "",
    images: [] as File[],
  });

  const [editData, setEditData] = useState<any>(null);

  // Specifications & Variants states
  const [specifications, setSpecifications] = useState<ProductSpec[]>([]);
  const [variants, setVariants] = useState<ProductVariant[]>([]);

  const [openAddVariant, setOpenAddVariant] = useState(false);

  // load products, categories, brands
  useEffect(() => {
    loadAll();
  }, []);

  const loadAll = async () => {
    setLoading(true);
    await Promise.all([loadBrands(), loadCategories(), loadProducts()]);
    setLoading(false);
  };

  const loadBrands = async () => {
    try {
      const res = await getBrandList();
      const list = res?.data?.data || res?.data || (Array.isArray(res) ? res : []) || [];
      setBrands(list);
    } catch (err) {
      console.error("Brand load failed", err);
      setBrands([]);
    }
  };

  const buildTreeFromFlat = (items: any[]) => {
    const map = new Map();
    items.forEach((it) => map.set(it.id, { ...it, subCategories: [] }));
    const roots: any[] = [];
    map.forEach((node) => {
      const parentId = node.parentId ?? node.parent?.id ?? null;
      if (parentId) {
        const parent = map.get(parentId);
        if (parent) parent.subCategories.push(node);
        else roots.push(node);
      } else {
        roots.push(node);
      }
    });
    return roots;
  };

  const loadCategories = async () => {
    try {
      const res = await getCategoriesList();
      let list = Array.isArray(res) ? res : (res ?? []);

      if (list.length > 0 && !("subCategories" in list[0])) {
        list = buildTreeFromFlat(list);
      }

      const tree = assignParentIds(list);
      setTreeCategories(tree);
    } catch (err) {
      console.error("Category load failed", err);
      setTreeCategories([]);
    }
  };

  const loadProducts = async () => {
    try {
      const res = await getAdminProducts();
      const list = res?.data?.data || res?.data || [];
      setProducts(list);
    } catch (err) {
      console.error("Product load failed", err);
      setProducts([]);
    }
  };

  const loadProductDetail = async (prod: any) => {
    try {
      const detail = await fetchProductDetail(prod);
      if (detail) {
      console.log("📦 Product Detail:", detail); // Debug để xem structure
      console.log("🖼️ Images:", detail.images); // Debug images
      console.log("📋 Specifications:", detail.specifications); // Debug specs
        setSelectedProduct(detail);
        setOpenDetail(true);
      }
    } catch (err) {
      console.error("Failed to load product detail", err);
    }
  };

  const assignParentIds = (nodes: any[], parentId: number | string | null = null) => {
    return nodes.map((node) => {
      node.parentId = parentId;
      if (node.subCategories && node.subCategories.length) {
        node.subCategories = assignParentIds(node.subCategories, node.id);
      }
      return node;
    });
  };

  const flatCategories = useMemo(() => {
    const out: { id: number; name: string }[] = [];
    const walk = (nodes: any[], depth = 0) => {
      for (const n of nodes) {
        out.push({ id: n.id, name: `${"—".repeat(depth)} ${n.name}` });
        if (n.subCategories && n.subCategories.length) walk(n.subCategories, depth + 1);
      }
    };
    walk(treeCategories);
    return out;
  }, [treeCategories]);

  const calcFinalPrice = (base: number = 0, disc: number = 0) => {
    const b = Number(base) || 0;
    const d = Number(disc) || 0;
    return Math.round(b - (b * d) / 100);
  };

  // Create Modal
  const handleCreate = async () => {
    try {
      if (!createData.name || !createData.slug || !createData.categoryId || !createData.brandId) {
        alert("Please fill required fields: Name, Slug, Category, Brand");
        return;
      }

      const fd = new FormData();
      fd.append("Name", createData.name);
      fd.append("Slug", createData.slug);
      fd.append("CategoryId", String(createData.categoryId));
      fd.append("BrandId", String(createData.brandId));
      if (createData.basePrice) fd.append("BasePrice", String(createData.basePrice));
      if (createData.discountPercent) fd.append("DiscountPercent", String(createData.discountPercent));
      if (createData.overview) fd.append("Overview", createData.overview);

      for (const f of createData.images) fd.append("Images", f);

      // Add specifications
      if (specifications.length > 0) {
        fd.append("Specifications", JSON.stringify(specifications));
      }

      // Add variants
      if (variants.length > 0) {
        fd.append("Variants", JSON.stringify(variants));
      }

      await createAdminProduct(fd);

      await loadAll();
      setOpenCreate(false);
      
      // Reset form
      setCreateData({
        name: "",
        slug: "",
        categoryId: "",
        brandId: "",
        basePrice: 0,
        discountPercent: 0,
        overview: "",
        images: [],
      });
      setSpecifications([]);
      setVariants([]);
      
      alert("Product created successfully!");
    } catch (err) {
      console.error("Create failed:", err);
      alert(`Create failed: ${err instanceof Error ? err.message : "Unknown error"}`);
    }
  };

  async function fetchProductDetail(prod: any) {
    try {
      if (!prod?.slug) return null;
      const res = await getAdminProduct(prod.slug);
      return res?.data ?? res;
    } catch (err) {
      console.error("fetchProductDetail error", err);
      return null;
    }
  }

const openEditModal = async (p: any) => {
  const detail = await fetchProductDetail(p);

  if (!detail) {
    alert("Không load được dữ liệu sản phẩm!");
    return;
  }

  console.log("🔍 Product detail:", detail);
  console.log("🖼️ Images from API:", detail.images);

  const matchedBrand = brands.find(
    (b: any) => b.name.toLowerCase() === detail.brandName?.toLowerCase()
  );

  const matchedCategory = flatCategories.find(
    (c: any) => c.name.replace(/—/g, "").trim().toLowerCase() === detail.categoryName?.toLowerCase()
  );

  setEditData({
    id: detail.id,
    name: detail.name,
    slug: detail.slug,
    basePrice: detail.basePrice,
    discountPercent: detail.discountPercent ?? 0,
    description: detail.description ?? "",
    longDescription: detail.longDescription ?? "",
    warrantyInfo: detail.warrantyInfo ?? "",
    overview: detail.overview ?? "",
    brandId: matchedBrand?.id ?? null,
    categoryId: matchedCategory?.id ?? null,
  });

  // ✅ FIX: Map images properly
  const images = detail.images || [];
  
  if (Array.isArray(images) && images.length > 0) {
    const mappedImages = images
      .map((img: any, index: number) => {
        // Nếu là string
        if (typeof img === 'string') {
          console.warn("⚠️ Image is string, cannot edit:", img);
          return null; // Không thể edit image dạng string
        }
        
        // Nếu là object
        const imageId = img.id || img.imageId;
        const imageUrl = img.imageUrl || img.url || img.path || img.imagePath;
        
        if (!imageId || !imageUrl) {
          console.warn("⚠️ Invalid image:", img);
          return null;
        }
        
        return {
          id: Number(imageId), // Đảm bảo là number
          imageUrl: imageUrl,
          isPrimary: img.isPrimary ?? false,
          sortOrder: img.sortOrder ?? index,
        };
      })
      .filter(Boolean); // Loại bỏ null values
    
    console.log("✅ Mapped images:", mappedImages);
    setEditImages(mappedImages);
  } else {
    setEditImages([]);
  }

  // Load specifications & variants
  setSpecifications(detail.specifications || []);
  
  // Fix variants with attributeName
  const variantsWithNames = (detail.variants || []).map((variant: any) => ({
    ...variant,
    attributes: variant.attributes.map((attr: any) => {
      if (!attr.attributeName && attr.attributeId) {
        const spec = (detail.specifications || []).find(
          (s: any) => s.attributeId === attr.attributeId
        );
        return {
          ...attr,
          attributeName: spec?.attributeName || attr.name || `Attribute ${attr.attributeId}`
        };
      }
      return {
        ...attr,
        attributeName: attr.attributeName || attr.name || `Attribute ${attr.attributeId}`
      };
    })
  }));
  
  setVariants(variantsWithNames);

  setNewImages([]);
  setOpenEdit(true);
};

  const handleDragStart = (e: any, index: number) => {
    e.dataTransfer.setData("drag-index", index);
  };

  const handleDrop = (e: any, dropIndex: number) => {
    const dragIndex = Number(e.dataTransfer.getData("drag-index"));
    if (dragIndex === dropIndex) return;

    const updated = [...editImages];
    const [moved] = updated.splice(dragIndex, 1);
    updated.splice(dropIndex, 0, moved);

    const reordered = updated.map((img, i) => ({
      ...img,
      sortOrder: i,
    }));

    setEditImages(reordered);
  };

  const makePrimary = (index: number) => {
    const updated = editImages.map((img, i) => ({
      ...img,
      isPrimary: i === index,
    }));
    setEditImages(updated);
  };

const handleEditSave = async () => {
  try {
    if (!editData.name || !editData.slug || !editData.brandId || !editData.categoryId) {
      alert("Please fill required fields: Name, Slug, Brand, Category");
      return;
    }

    const productForm = new FormData();
    productForm.append("Name", editData.name);
    productForm.append("Slug", editData.slug);
    productForm.append("CategoryId", String(editData.categoryId));
    productForm.append("BrandId", String(editData.brandId));
    productForm.append("Status", editData.status || "");
    productForm.append("BasePrice", String(editData.basePrice || 0));
    productForm.append("Description", editData.description || "");
    productForm.append("LongDescription", editData.longDescription || "");
    productForm.append("WarrantyInfo", editData.warrantyInfo || "");
    productForm.append("Overview", editData.overview || "");
    productForm.append("DiscountPercent", String(editData.discountPercent || 0));

    // Add specifications
    if (specifications.length > 0) {
      productForm.append("Specifications", JSON.stringify(specifications));
    }

    // Add variants
    if (variants.length > 0) {
      productForm.append("Variants", JSON.stringify(variants));
    }

    await updateAdminProduct(editData.id, productForm);

    console.log("✅ Step 1: Product info updated");

    // ✅ FIX: Filter out invalid images (id phải là số > 0)
    const validImages = editImages.filter(img => {
      const isValid = img.id && typeof img.id === 'number' && img.id > 0;
      if (!isValid) {
        console.warn("⚠️ Skipping invalid image:", img);
      }
      return isValid;
    });

    console.log("📸 Valid images to update:", validImages);

    // Update images metadata (chỉ với valid images)
    for (const img of validImages) {
      try {
        const updatePayload = {
          isPrimary: !!img.isPrimary,
          sortOrder: img.sortOrder ?? 0,
        };
        console.log(`Updating image ${img.id}:`, updatePayload);
        await updateImageMeta(img.id, updatePayload);
        console.log(`✅ Image ${img.id} updated`);
      } catch (e) {
        console.error(`❌ Failed to update image ${img.id}:`, e);
        // Không throw error, tiếp tục với ảnh khác
      }
    }

    // Upload new images
    const uploadedImageIds: number[] = [];
    if (newImages.length > 0) {
      console.log(`📤 Uploading ${newImages.length} new images...`);
      
      for (let i = 0; i < newImages.length; i++) {
        const file = newImages[i];
        // Chỉ set isPrimary nếu không có ảnh nào là primary
        const isPrimaryForThis = validImages.length === 0 && i === 0;

        try {
          const result = await uploadImage(file, editData.id, isPrimaryForThis);
          const newId = result?.id ?? result?.data?.id;
          if (newId && typeof newId === 'number' && newId > 0) {
            uploadedImageIds.push(newId);
            console.log(`✅ Uploaded image ${i + 1}, ID: ${newId}`);
          }
        } catch (e) {
          console.error(`❌ Failed to upload image ${i + 1}:`, e);
        }
      }
    }

    // ✅ FIX: Reorder images - chỉ dùng valid IDs
    const existingImageIds = validImages.map(img => img.id);
    const allImageIds = [...existingImageIds, ...uploadedImageIds];
    
    console.log("🔢 Image IDs for reordering:", allImageIds);

    if (allImageIds.length > 0) {
      // Kiểm tra tất cả IDs đều hợp lệ
      const allValid = allImageIds.every(id => id && typeof id === 'number' && id > 0);
      
      if (allValid) {
        try {
          await reorderImages(editData.id, allImageIds);
          console.log("✅ Images reordered successfully");
        } catch (e) {
          console.error("❌ Failed to reorder images:", e);
          // Không throw, vì product đã update thành công
        }
      } else {
        console.warn("⚠️ Skipping reorder - some IDs are invalid:", allImageIds);
      }
    }

    alert("Product updated successfully!");
    await loadProducts();
    setOpenEdit(false);
    setEditImages([]);
    setNewImages([]);
    setSpecifications([]);
    setVariants([]);
  } catch (err) {
    console.error("Update failed:", err);
    alert(`Update failed: ${err instanceof Error ? err.message : "Unknown error"}`);
  }
};

  const deleteProduct = async (id: number) => {
    if (!confirm("Delete product?")) return;
    try {
      await deleteAdminProduct(id);
      await loadAll();
      alert("Deleted");
    } catch (err) {
      console.error(err);
      alert("Delete failed");
    }
  };

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-xl font-semibold">Products</h2>
        <Button onClick={() => setOpenCreate(true)}>+ Create Product</Button>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-sm bg-white shadow rounded">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2">ID</th>
                <th>Brand</th>
                <th>Category</th>
                <th>Name</th>
                <th>BasePrice</th>
                <th>Discount%</th>
                <th>FinalPrice</th>
                <th>Sold</th>
                <th>Rating</th>
                <th>Reviews</th>
                <th>Image</th>
                <th></th>
              </tr>
            </thead>

            <tbody>
              {products.map((p) => (
                <tr
                  key={p.id}
                  className="cursor-pointer hover:bg-gray-100"
                  onClick={() => loadProductDetail(p)}
                >
                  <td className="p-2">{p.id}</td>
                  <td className="p-2">{p.brandName ?? "-"}</td>
                  <td className="p-2">{p.categoryName ?? "-"}</td>
                  <td className="p-2 font-medium">{p.name}</td>
                  <td className="p-2">{p.basePrice ?? "-"}</td>
                  <td className="p-2">{p.discountPercent ?? 0}</td>
                  <td className="p-2">
                    {p.finalPrice ?? calcFinalPrice(p.basePrice, p.discountPercent)}
                  </td>
                  <td className="p-2">{p.totalSold ?? 0}</td>
                  <td className="p-2">{p.averageRating ?? 0}</td>
                  <td className="p-2">{p.totalReviews ?? 0}</td>
                  <td className="p-2">
                    {(() => {
                      const imgs = p.images || [];
                      const primary = imgs.find((i: any) => i.isPrimary);
                      const count = imgs.length;

                      if (!primary)
                        return (
                          <div className="w-20 h-12 bg-gray-100 rounded flex items-center justify-center text-xs text-gray-500">
                            No image
                          </div>
                        );

                      return (
                        <div className="relative w-20 h-12">
                          <Image
                            src={primary.imageUrl}
                            alt={p.name}
                            fill
                            className="object-cover rounded"
                            sizes="80px"
                          />
                          {count > 1 && (
                            <span className="absolute bottom-1 right-1 bg-black bg-opacity-60 text-white text-xs px-1 py-[1px] rounded">
                              {count}
                            </span>
                          )}
                        </div>
                      );
                    })()}
                  </td>

                  <td className="p-2">
                    <div className="flex gap-2">
                      <Actions
                        onEdit={() => openEditModal(p)}
                        onDelete={() => deleteProduct(p.id)}
                      />
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* === EDIT PRODUCT MODAL === */}
      {openEdit && editData && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
          <div className="bg-white w-full max-w-6xl rounded-lg shadow-lg p-6 overflow-y-auto max-h-[90vh]">
            <h2 className="text-xl font-semibold mb-4">Edit Product</h2>

            {/* Basic Fields */}
            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <label className="block font-medium mb-1">Name *</label>
                <input
                  className="border rounded p-2 w-full"
                  value={editData.name || ""}
                  onChange={(e) => setEditData({ ...editData, name: e.target.value })}
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Slug *</label>
                <input
                  className="border rounded p-2 w-full"
                  value={editData.slug || ""}
                  onChange={(e) => setEditData({ ...editData, slug: e.target.value })}
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Category *</label>
                <select
                  className="border rounded p-2 w-full"
                  value={editData.categoryId || ""}
                  onChange={(e) =>
                    setEditData({ ...editData, categoryId: Number(e.target.value) })
                  }
                >
                  <option value="">-- Select --</option>
                  {flatCategories.map((c: any) => (
                    <option key={c.id} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block font-medium mb-1">Brand *</label>
                <select
                  className="border rounded p-2 w-full"
                  value={editData.brandId || ""}
                  onChange={(e) =>
                    setEditData({ ...editData, brandId: Number(e.target.value) })
                  }
                >
                  <option value="">-- Select --</option>
                  {brands.map((b: any) => (
                    <option key={b.id} value={b.id}>
                      {b.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block font-medium mb-1">Base Price</label>
                <input
                  type="number"
                  className="border rounded p-2 w-full"
                  value={editData.basePrice || 0}
                  onChange={(e) =>
                    setEditData({ ...editData, basePrice: Number(e.target.value) })
                  }
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Discount (%)</label>
                <input
                  type="number"
                  className="border rounded p-2 w-full"
                  value={editData.discountPercent || 0}
                  onChange={(e) =>
                    setEditData({ ...editData, discountPercent: Number(e.target.value) })
                  }
                />
              </div>
            </div>

            {/* Text fields */}
            <div className="space-y-4 mb-6">
              <div>
                <label className="block font-medium mb-1">Description</label>
                <textarea
                  className="border rounded p-2 w-full"
                  rows={3}
                  value={editData.description || ""}
                  onChange={(e) =>
                    setEditData({ ...editData, description: e.target.value })
                  }
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Long Description</label>
                <textarea
                  className="border rounded p-2 w-full"
                  rows={4}
                  value={editData.longDescription || ""}
                  onChange={(e) =>
                    setEditData({ ...editData, longDescription: e.target.value })
                  }
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Overview</label>
                <textarea
                  className="border rounded p-2 w-full"
                  rows={3}
                  value={editData.overview || ""}
                  onChange={(e) =>
                    setEditData({ ...editData, overview: e.target.value })
                  }
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Warranty Info</label>
                <input
                  className="border rounded p-2 w-full"
                  value={editData.warrantyInfo || ""}
                  onChange={(e) =>
                    setEditData({ ...editData, warrantyInfo: e.target.value })
                  }
                />
              </div>
            </div>

{/* === IMAGES SECTION === */}
<div className="mb-6 border-t pt-6">
  <h3 className="font-semibold mb-3">Images</h3>
  
  {/* Existing Images */}
  {editImages.length > 0 && (
    <div className="mb-4">
      <p className="text-sm text-gray-600 mb-2">Current Images (drag to reorder)</p>
      <div className="flex gap-3 flex-wrap">
        {editImages.map((img, index) => {
          // Debug: xem img có gì
          console.log("📸 Image data:", img);
          
          // Lấy URL từ nhiều nguồn có thể
          const imageUrl = img.imageUrl || img.url || img.path;
          
          if (!imageUrl) {
            console.warn("⚠️ No image URL found for:", img);
            return null;
          }
          
          return (
            <div
              key={`edit-img-${img.id}-${index}`} // Fix: unique key
              draggable
              onDragStart={(e) => handleDragStart(e, index)}
              onDragOver={(e) => e.preventDefault()}
              onDrop={(e) => handleDrop(e, index)}
              className="relative cursor-move border-2 border-dashed border-gray-300 rounded p-1 hover:border-blue-400"
            >
              <img
                src={imageUrl}
                alt={`Product ${index + 1}`}
                className="w-28 h-28 object-cover rounded"
                onError={(e) => {
                  console.error("❌ Image load error:", imageUrl);
                  e.currentTarget.src = 'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" width="112" height="112"><rect width="112" height="112" fill="%23ddd"/><text x="50%" y="50%" text-anchor="middle" dy=".3em" fill="%23999">No Image</text></svg>';
                }}
              />
              
              {/* Primary Badge */}
              {img.isPrimary && (
                <span className="absolute top-2 left-2 bg-blue-600 text-white text-xs px-2 py-0.5 rounded">
                  Primary
                </span>
              )}
              
              {/* Action Buttons */}
              <div className="absolute top-2 right-2 flex gap-1">
                {!img.isPrimary && (
                  <button
                    type="button"
                    onClick={(e) => {
                      e.stopPropagation();
                      makePrimary(index);
                    }}
                    className="bg-green-600 text-white text-xs px-2 py-1 rounded hover:bg-green-700"
                    title="Set as primary"
                  >
                    ⭐
                  </button>
                )}
                
                <button
                  type="button"
                  onClick={async (e) => {
                    e.stopPropagation();
                    if (!confirm('Delete this image?')) return;
                    try {
                      await deleteImage(img.id);
                      setEditImages(editImages.filter((_, i) => i !== index));
                      alert('Image deleted');
                    } catch (err) {
                      console.error('Delete image failed:', err);
                      alert('Delete failed');
                    }
                  }}
                  className="bg-red-600 text-white text-xs px-2 py-1 rounded hover:bg-red-700"
                  title="Delete image"
                >
                  🗑️
                </button>
              </div>
              
              {/* Sort Order */}
              <span className="absolute bottom-2 right-2 bg-black bg-opacity-60 text-white text-xs px-1.5 py-0.5 rounded">
                {index + 1}
              </span>
            </div>
          );
        })}
      </div>
    </div>
  )}
  
  {/* Upload New Images */}
  <div className="mb-4">
    <label className="block font-medium mb-2">Upload New Images</label>
    <input
      type="file"
      multiple
      accept="image/*"
      onChange={(e) => {
        const files = Array.from(e.target.files || []);
        setNewImages([...newImages, ...files]);
      }}
      className="border rounded p-2 w-full"
    />
    
    {/* Preview New Images */}
    {newImages.length > 0 && (
      <div className="mt-3">
        <p className="text-sm text-gray-600 mb-2">New Images to Upload ({newImages.length})</p>
        <div className="flex gap-3 flex-wrap">
          {newImages.map((file, idx) => (
            <div key={`new-img-${idx}-${file.name}`} className="relative border rounded p-1">
              <img
                src={URL.createObjectURL(file)}
                alt={`New ${idx + 1}`}
                className="w-28 h-28 object-cover rounded"
              />
              
              {/* Remove button */}
              <button
                type="button"
                onClick={() => {
                  setNewImages(newImages.filter((_, i) => i !== idx));
                }}
                className="absolute top-2 right-2 bg-red-600 text-white text-xs px-2 py-1 rounded hover:bg-red-700"
              >
                ✕
              </button>
              
              {/* File name */}
              <p className="text-xs text-gray-600 mt-1 truncate w-28" title={file.name}>
                {file.name}
              </p>
            </div>
          ))}
        </div>
      </div>
    )}
  </div>
  
  {/* Help text */}
  <p className="text-xs text-gray-500">
    💡 Tip: Drag images to reorder. Click ⭐ to set as primary image.
  </p>
</div>

            {/* === SPECIFICATIONS SECTION === */}
            {editData.categoryId && (
              <div className="mb-6 border-t pt-6">
                <ProductSpecifications
                  productId={editData.id}
                  categoryId={Number(editData.categoryId)}
                  initialSpecs={specifications}
                  onChange={setSpecifications}
                />
              </div>
            )}

            {/* === VARIANTS SECTION === */}
            <div className="mb-6 border-t pt-6">
              <ProductVariants
                productId={editData.id}
                basePrice={editData.basePrice}
                initialVariants={variants}
                onChange={setVariants}
              />
            </div>

            {/* Buttons */}
            <div className="flex justify-end gap-3 mt-4">
              <button
                className="px-4 py-2 bg-gray-300 rounded"
                onClick={() => {
                  setOpenEdit(false);
                  setSpecifications([]);
                  setVariants([]);
                }}
              >
                Cancel
              </button>

              <button
                className="px-4 py-2 bg-blue-600 text-white rounded"
                onClick={handleEditSave}
              >
                Save Changes
              </button>
            </div>
          </div>
        </div>
      )}

      {/* === CREATE PRODUCT MODAL === */}
      {openCreate && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
          <div className="bg-white w-full max-w-4xl rounded-lg shadow-lg p-6 max-h-[90vh] overflow-y-auto">
            <h2 className="text-xl font-semibold mb-4">Create Product</h2>

            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <label className="block font-medium mb-1">Name *</label>
                <input
                  className="border rounded p-2 w-full"
                  value={createData.name}
                  onChange={(e) =>
                    setCreateData({ ...createData, name: e.target.value })
                  }
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Slug *</label>
                <input
                  className="border rounded p-2 w-full"
                  value={createData.slug}
                  onChange={(e) =>
                    setCreateData({ ...createData, slug: e.target.value })
                  }
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Category *</label>
                <select
                  className="border rounded p-2 w-full"
                  value={String(createData.categoryId)}
                  onChange={(e) =>
                    setCreateData({ ...createData, categoryId: Number(e.target.value) })
                  }
                >
                  <option value="">-- Select --</option>
                  {flatCategories.map((c: any) => (
                    <option key={c.id} value={c.id}>
                      {c.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block font-medium mb-1">Brand *</label>
                <select
                  className="border rounded p-2 w-full"
                  value={String(createData.brandId)}
                  onChange={(e) =>
                    setCreateData({ ...createData, brandId: e.target.value })
                  }
                >
                  <option value="">-- Select --</option>
                  {brands.map((b: any) => (
                    <option key={b.id} value={b.id}>
                      {b.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block font-medium mb-1">Base Price</label>
                <input
                  type="number"
                  className="border rounded p-2 w-full"
                  value={createData.basePrice}
                  onChange={(e) =>
                    setCreateData({ ...createData, basePrice: Number(e.target.value) })
                  }
                />
              </div>

              <div>
                <label className="block font-medium mb-1">Discount (%)</label>
                <input
                  type="number"
                  className="border rounded p-2 w-full"
                  value={createData.discountPercent}
                  onChange={(e) =>
                    setCreateData({
                      ...createData,
                      discountPercent: Number(e.target.value),
                    })
                  }
                />
              </div>
            </div>

            <div className="mt-4 mb-6">
              <label className="block font-medium mb-1">Overview</label>
              <textarea
                rows={3}
                className="border rounded p-2 w-full"
                value={createData.overview}
                onChange={(e) =>
                  setCreateData({ ...createData, overview: e.target.value })
                }
              />
            </div>

            {/* === SPECIFICATIONS SECTION === */}
            {createData.categoryId && (
              <div className="mb-6 border-t pt-6">
                <ProductSpecifications
                  categoryId={Number(createData.categoryId)}
                  initialSpecs={specifications}
                  onChange={setSpecifications}
                />
              </div>
            )}

            {/* === VARIANTS SECTION === */}
            <div className="mb-6 border-t pt-6">
              <ProductVariants
                basePrice={createData.basePrice}
                initialVariants={variants}
                onChange={setVariants}
              />
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <button
                className="px-4 py-2 bg-gray-300 rounded"
                onClick={() => {
                  setOpenCreate(false);
                  setCreateData({
                    name: "",
                    slug: "",
                    categoryId: "",
                    brandId: "",
                    basePrice: 0,
                    discountPercent: 0,
                    overview: "",
                    images: [],
                  });
                  setSpecifications([]);
                  setVariants([]);
                }}
              >
                Cancel
              </button>

              <button
                className="px-4 py-2 bg-blue-600 text-white rounded"
                onClick={handleCreate}
              >
                Create Product
              </button>
            </div>
          </div>
        </div>
      )}

      {/* === PRODUCT DETAIL MODAL === */}
      {openDetail && selectedProduct && (
        <div className="fixed inset-0 bg-black/60 flex justify-center items-center z-50">
          <div className="bg-white p-6 rounded-lg w-[900px] max-h-[90vh] overflow-y-auto shadow-xl">
            <div className="flex justify-between items-center mb-4">
            <h2 className="text-xl font-bold mb-4">
              Product Detail — {selectedProduct.name}
            </h2>
            <button
              onClick={() => setOpenDetail(false)}
              className="text-gray-500 hover:text-gray-700"
            >
              <X size={28} />
            </button>
            </div>

            {/* Basic Info */}
            <div className="grid grid-cols-2 gap-4 mb-6">
              <div>
                <p>
                  <strong>ID:</strong> {selectedProduct.id}
                </p>
                <p>
                  <strong>Name:</strong> {selectedProduct.name}
                </p>
                <p>
                  <strong>Slug:</strong> {selectedProduct.slug}
                </p>
                <p>
                  <strong>Brand:</strong> {selectedProduct.brandName}
                </p>
                <p>
                  <strong>Category:</strong> {selectedProduct.categoryName}
                </p>
                <p>
                  <strong>Base Price:</strong>{" "}
                  {selectedProduct.basePrice.toLocaleString()}đ
                </p>
                <p>
                  <strong>Discount:</strong> {selectedProduct.discountPercent ?? 0}%
                </p>
                <p>
                  <strong>Final Price:</strong>{" "}
                  {selectedProduct.finalPrice.toLocaleString()}đ
                </p>
                <p>
                  <strong>Total Sold:</strong> {selectedProduct.totalSold}
                </p>
              </div>
              <div>
                <p>
                  <strong>Overview:</strong>
                </p>
                <p className="text-gray-700">{selectedProduct.overview}</p>
                <p className="mt-2">
                  <strong>Description:</strong>
                </p>
                <p className="text-gray-700">{selectedProduct.description}</p>
              </div>
            </div>

            {/* Images */}
<div className="mt-4">
  <h3 className="font-semibold mb-2">Images</h3>
  {selectedProduct.images?.length > 0 ? (
    <div className="flex gap-3 flex-wrap">
      {selectedProduct.images.map((img: any, idx: number) => {
        const imageUrl = typeof img === 'string' ? img : img?.imageUrl;
        if (!imageUrl) return null;
        
        return (
          <img
            key={idx}
            src={imageUrl}
            alt={`${selectedProduct.name} ${idx + 1}`}
            className="w-28 h-28 object-cover rounded border"
            onError={(e) => e.currentTarget.style.display = 'none'}
          />
        );
      })}
    </div>
  ) : (
    <p className="text-gray-500 text-sm">No images</p>
  )}
</div>

{/* Specifications */}
      {selectedProduct.specs?.length > 0 && (
        <div className="mt-6">
          <h3 className="font-semibold mb-2">Specifications</h3>
          <div className="border rounded p-3 space-y-3">
            {selectedProduct.specs.map((s: any, idx: number) => (
              <div key={idx}>
                <p className="font-bold">{s.name}</p>
                <ul className="list-disc ml-5 text-gray-700">
                  {s.value.map((v: string, i: number) => (
                    <li key={i}>{v}</li>
                  ))}
                </ul>
              </div>
            ))}
          </div>
        </div>
      )}

            {/* Variants */}
            {selectedProduct.variants?.length > 0 && (
              <div className="mb-6">
                <h3 className="font-semibold mb-2">Variants</h3>
                <div className="space-y-3">
                  {selectedProduct.variants.map((v: any) => (
                    <div key={v.id} className="border rounded p-4 bg-white">
                      <div className="grid grid-cols-4 gap-4 mb-2">
                        <div>
                          <span className="text-xs text-gray-600">SKU:</span>
                          <p className="font-medium">{v.sku}</p>
                        </div>
                        <div>
                          <span className="text-xs text-gray-600">Price:</span>
                          <p className="font-medium">{v.price.toLocaleString()}đ</p>
                        </div>
                        <div>
                          <span className="text-xs text-gray-600">Stock:</span>
                          <p className="font-medium">{v.stock}</p>
                        </div>
                        <div>
                          <span className="text-xs text-gray-600">Status:</span>
                          <p className="font-medium">{v.status || "Avaiable"}</p>
                        </div>
                      </div>

                      <div className="mt-2">
                        <span className="text-xs text-gray-600">Attributes:</span>
                        <div className="flex flex-wrap gap-2 mt-1">
                          {v.attributes?.map((a: any, idx: number) => (
                            <span
                              key={idx}
                              className="px-2 py-1 bg-green-100 text-green-800 rounded text-sm"
                            >
                              <strong>{a.attributeName || a.name}:</strong>{" "}
                              {Array.isArray(a.value) ? a.value.join(", ") : a.value}
                            </span>
                          ))}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}