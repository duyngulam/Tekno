"use client";

import React, { useEffect, useMemo, useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { get } from "@/lib/api";
import { ChevronDown, ChevronRight } from "lucide-react";
import Actions from "@/components/admin/Actions";
import Image from "next/image";
import { uploadImage, updateImageMeta, deleteImage, reorderImages, deleteVariant } from "@/lib/productsImageApi";

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
    images: [] as File[], // multiple files
  });

  const [editData, setEditData] = useState<any>(null); // will hold product fields + files preview

  // load products, categories, brands
useEffect(() => {
  loadAll();
}, []);

const loadAll = async () => {
  setLoading(true);

  await Promise.all([
    loadBrands(),
    loadCategories(),
    loadProducts(),
  ]);

  setLoading(false);
};

const loadBrands = async () => {
  try {
    const res = await get("http://localhost:5000/api/admin/brands");

    const list =
      res?.data?.data ||
      res?.data ||
      (Array.isArray(res) ? res : []) ||
      [];

    setBrands(list);
  } catch (err) {
    console.error("Brand load failed", err);
    setBrands([]);
  }
};

const loadCategories = async () => {
  try {
    const res = await get("http://localhost:5000/api/admin/categories/tree");
    let list = res?.data?.data || res?.data || res || [];
    list = assignParentIds(list);
    setTreeCategories(list);
  } catch (err) {
    console.error("Category load failed", err);
    setTreeCategories([]);
  }
};

const loadProducts = async () => {
  try {
    const res = await get("http://localhost:5000/api/admin/products");
    const list = res?.data?.data || res?.data || [];
    setProducts(list);
  } catch (err) {
    console.error("Product load failed", err);
    setProducts([]);
  }
};

const loadProductDetail = async (prod: any) => {
  try {
    const res = await get(`http://localhost:5000/api/admin/products/${prod.slug}`);
    setSelectedProduct(res?.data);
    setOpenDetail(true);
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

  // flatten categories for select
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

  // helpers
  const calcFinalPrice = (base: number = 0, disc: number = 0) => {
    const b = Number(base) || 0;
    const d = Number(disc) || 0;
    return Math.round(b - (b * d) / 100);
  };

  // Create
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

      // append images (key: Images) - many backends accept repeated key
      for (const f of createData.images) {
        fd.append("Images", f);
      }

      const res = await fetch("http://localhost:5000/api/admin/products/create", {
        method: "POST",
        body: fd,
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(`Create failed: ${text}`);
      }

      await loadAll();
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
      alert("Product created");
    } catch (err) {
      console.error(err);
      alert("Create failed");
    }
  };

  // Open edit modal
async function fetchProductDetail(prod: any) {
  try {
    const slug = prod.slug;
    if (!slug) return null;

    const res = await fetch(`http://localhost:5000/api/admin/products/${slug}`);
    if (!res.ok) return null;

    const json = await res.json();
    return json.data;        // backend trả data trong data
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

  // map brand
  const matchedBrand = brands.find(
    (b: any) => b.name.toLowerCase() === detail.brandName?.toLowerCase()
  );

  // map category
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

  // set existing images for reorder
setEditImages(
  detail.images.map((img: any) => ({
    id: img.id,
    imageUrl: img.imageUrl,
    isPrimary: img.isPrimary,
    sortOrder: img.sortOrder
  }))
);


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

  // cập nhật sortOrder mới
  const reordered = updated.map((img, i) => ({
    ...img,
    sortOrder: i,
  }));

  setEditImages(reordered);
};

  const setPrimary = (imgId: number) => {
  const updated = editImages.map((img) => ({
    ...img,
    isPrimary: img.id === imgId,
  }));

  setEditImages(updated);
};

const deleteExistingImage = async (id: number) => {
  if (!confirm("Delete this image?")) return;

  try {
    await deleteImage(id);
    setEditImages((prev) => prev.filter((img) => img.id !== id));
  } catch (err) {
    console.error("Failed to delete image:", err);
    alert("Failed to delete image");
  }
};
  const handleAddNewImages = (e: any) => {
  setNewImages([...newImages, ...Array.from(e.target.files as FileList)]);
};

const reorderEditImages = (dropIndex: number) => {
  if (draggingIndex === null) return;

  const updated = [...editImages];
  const moved = updated.splice(draggingIndex, 1)[0];
  updated.splice(dropIndex, 0, moved);

  // update sortOrder
  const reordered = updated.map((img, i) => ({
    ...img,
    sortOrder: i
  }));

  setEditImages(reordered);
  setDraggingIndex(null);
};

const makePrimary = (index: number) => {
  const updated = editImages.map((img, i) => ({
    ...img,
    isPrimary: i === index
  }));
  setEditImages(updated);
};


const handleEditSave = async () => {
  try {
    // Validate required fields
    if (!editData.name || !editData.slug || !editData.brandId || !editData.categoryId) {
      alert("Please fill required fields: Name, Slug, Brand, Category");
      return;
    }

    // ========================================
    // STEP 1: Update basic product info
    // ========================================
    const productForm = new FormData();
    // Note: Don't send Id in body, it's in URL path
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

    const productRes = await fetch(`http://localhost:5000/api/admin/products/${editData.id}`, {
      method: "PUT",
      body: productForm,
    });

    if (!productRes.ok) {
      const errorText = await productRes.text();
      console.error("Update product error:", errorText);
      throw new Error(`Failed to update product: ${errorText}`);
    }

    console.log("✅ Step 1: Product info updated");

// ========================================
// STEP 2: Update existing images metadata
// ========================================
for (const img of editImages) {
  try {
    const updatePayload = {
      imageId: img.id,
      isPrimary: !!img.isPrimary,
      sortOrder: img.sortOrder ?? 0,
    };

    const resp = await updateImageMeta(img.id, updatePayload);
    console.log(`✅ Updated image ${img.id}`, resp);
  } catch (e) {
    console.error(`Failed to update image ${img.id}:`, e);
  }
}
// ========================================
// STEP 3: Upload new images
// ========================================
const uploadedImageIds: number[] = [];

if (newImages.length > 0) {
  for (let i = 0; i < newImages.length; i++) {
    const file = newImages[i];
    const isPrimaryForThis = editImages.length === 0 && i === 0;

    try {
      const result = await uploadImage(file, editData.id, isPrimaryForThis);
      const newId = result?.id ?? result?.data?.id;
      if (newId) uploadedImageIds.push(newId);
      console.log(`✅ Uploaded image ${i + 1}:`, result);
    } catch (e) {
      console.error(`Failed to upload image ${i + 1}:`, e);
    }
  }

  console.log("✅ Step 3: New images uploaded");
}
// ========================================
// STEP 4: Reorder all images
// ========================================
const allImageIds = [...editImages.map((img) => img.id), ...uploadedImageIds];

if (allImageIds.length > 0) {
  try {
    const reorderResult = await reorderImages(editData.id, allImageIds);
    console.log("✅ Step 4: Images reordered", reorderResult);
  } catch (e) {
    console.error("Failed to reorder images:", e);
  }
}

    // ========================================
    // SUCCESS
    // ========================================
    alert("Product updated successfully!");
    await loadProducts();
    setOpenEdit(false);
    setEditImages([]);
    setNewImages([]);

  } catch (err) {
    console.error("Update failed:", err);
    alert(`Update failed: ${err instanceof Error ? err.message : "Unknown error"}`);
  }
};

  const deleteProduct = async (id: number) => {
    if (!confirm("Delete product?")) return;
    try {
      const res = await fetch(`http://localhost:5000/api/admin/products/${id}`, { method: "DELETE" });
      if (!res.ok) {
        const text = await res.text();
        throw new Error(`Delete failed: ${text}`);
      }
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
                  <td className="p-2">{p.finalPrice ?? calcFinalPrice(p.basePrice, p.discountPercent)}</td>
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


        {/* badge số lượng ảnh */}
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
{openEdit && (
  <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
    <div className="bg-white w-full max-w-4xl rounded-lg shadow-lg p-6 overflow-y-auto max-h-[90vh]">

      <h2 className="text-xl font-semibold mb-4">Edit Product</h2>

      {/* NAME + SLUG */}
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
      </div>

      {/* CATEGORY + BRAND */}
      <div className="grid grid-cols-2 gap-4 mb-4">
        <div>
          <label className="block font-medium mb-1">Category *</label>
          <select
            className="border rounded p-2 w-full"
            value={editData.categoryId || ""}
            onChange={(e) => setEditData({ ...editData, categoryId: Number(e.target.value) })}
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
            onChange={(e) => setEditData({ ...editData, brandId: Number(e.target.value) })}
          >
            <option value="">-- Select --</option>
            {brands.map((b: any) => (
              <option key={b.id} value={b.id}>
                {b.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* PRICE + DISCOUNT */}
      <div className="grid grid-cols-2 gap-4 mb-4">
        <div>
          <label className="block font-medium mb-1">Base Price</label>
          <input
            type="number"
            className="border rounded p-2 w-full"
            value={editData.basePrice || 0}
            onChange={(e) => setEditData({ ...editData, basePrice: Number(e.target.value) })}
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

      {/* DESCRIPTION */}
      <div className="mb-4">
        <label className="block font-medium mb-1">Description</label>
        <textarea
          className="border rounded p-2 w-full"
          rows={3}
          value={editData.description || ""}
          onChange={(e) => setEditData({ ...editData, description: e.target.value })}
        />
      </div>

      {/* LONG DESCRIPTION */}
      <div className="mb-4">
        <label className="block font-medium mb-1">Long Description</label>
        <textarea
          className="border rounded p-2 w-full"
          rows={4}
          value={editData.longDescription || ""}
          onChange={(e) => setEditData({ ...editData, longDescription: e.target.value })}
        />
      </div>

      {/* OVERVIEW */}
      <div className="mb-4">
        <label className="block font-medium mb-1">Overview</label>
        <textarea
          className="border rounded p-2 w-full"
          rows={3}
          value={editData.overview || ""}
          onChange={(e) => setEditData({ ...editData, overview: e.target.value })}
        />
      </div>

      {/* WARRANTY */}
      <div className="mb-4">
        <label className="block font-medium mb-1">Warranty Info</label>
        <input
          className="border rounded p-2 w-full"
          value={editData.warrantyInfo || ""}
          onChange={(e) => setEditData({ ...editData, warrantyInfo: e.target.value })}
        />
      </div>

      {/* ================= EXISTING IMAGES ================= */}
      <div className="mb-6">
        <h3 className="font-semibold mb-2">Existing Images (Reorder + Primary)</h3>

        {editImages.length === 0 && (
          <p className="text-gray-400 text-sm">No existing images</p>
        )}

        <div className="grid grid-cols-4 gap-2 mt-2">

  {editImages.map((img, index) => (
    <div
      key={`${img.id || "temp"}-${index}`}
      className="relative border rounded overflow-hidden cursor-move p-2"
      draggable
      onDragStart={(e) => handleDragStart(e, index)}
      onDragOver={(e) => e.preventDefault()}
      onDrop={(e) => handleDrop(e, index)}
    >
      {/* Image */}
<div className="relative w-full h-24">
  <Image
    src={img.imageUrl}
    alt="Product image"
    fill
    sizes="200px"
    className="object-cover rounded"
  />
</div>


      {/* Primary radio */}
      <label className="flex items-center gap-1 text-sm mt-1">
        <input
          type="radio"
          name="primaryImg"
          checked={!!img.isPrimary}   // 🔥 FIX
          onChange={() => makePrimary(index)}
        />
        Primary
      </label>

      {/* Sort order */}
      <div className="text-xs text-gray-600">Order: {img.sortOrder}</div>
    </div>
  ))}
</div>

      </div>

      {/* ================= NEW IMAGES ================= */}
      <div className="mb-6">
        <h3 className="font-semibold mb-2">Add New Images</h3>

        <input
          type="file"
          accept="image/*"
          multiple
          onChange={(e) => {
            const files = Array.from(e.target.files || []);
            setNewImages([...newImages, ...files]);
          }}
        />

        {/* Preview new images */}
        {newImages.length > 0 && (
          <div className="flex flex-wrap gap-4 mt-3">
            {newImages.map((file, idx) => (
              <div key={idx} className="border rounded p-2 bg-gray-50 w-32 text-center">
<div className="relative w-28 h-20 mb-2">
  <Image
    src={URL.createObjectURL(file)}
    alt="new img"
    fill
    className="object-cover rounded"
    sizes="120px"
/>
</div>
                <button
                  className="text-red-500 text-xs"
                  onClick={() =>
                    setNewImages(newImages.filter((_, i) => i !== idx))
                  }
                >
                  Remove
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* BUTTONS */}
      <div className="flex justify-end gap-3 mt-4">
        <button
          className="px-4 py-2 bg-gray-300 rounded"
          onClick={() => setOpenEdit(false)}
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

  {/* === PRODUCT DETAIL MODAL === */}
    
    {openDetail && selectedProduct && (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex justify-center items-center">
    <div className="bg-white p-6 rounded-lg w-[800px] max-h-[90vh] overflow-y-auto shadow-xl">

      <h2 className="text-xl font-bold mb-4">
        Product Detail — {selectedProduct.name}
      </h2>

      {/* Basic Info */}
      <div className="grid grid-cols-2 gap-4">
        <div>
          <p><strong>ID:</strong> {selectedProduct.id}</p>
          <p><strong>Name:</strong> {selectedProduct.name}</p>
          <p><strong>Slug:</strong> {selectedProduct.slug}</p>
          <p><strong>Brand:</strong> {selectedProduct.brandName}</p>
          <p><strong>Category:</strong> {selectedProduct.categoryName}</p>
          <p><strong>Base Price:</strong> {selectedProduct.basePrice.toLocaleString()}đ</p>
          <p><strong>Discount:</strong> {selectedProduct.discountPercent ?? 0}%</p>
          <p><strong>Final Price:</strong> {selectedProduct.finalPrice.toLocaleString()}đ</p>
          <p><strong>Total Sold:</strong> {selectedProduct.totalSold}</p>
        </div>
        <div>
          <p><strong>Overview:</strong></p>
          <p className="text-gray-700">{selectedProduct.overview}</p>
          <p className="mt-2"><strong>Description:</strong></p>
          <p className="text-gray-700">{selectedProduct.description}</p>
        </div>
      </div>

      {/* Images */}
      <div className="mt-4">
        <h3 className="font-semibold mb-2">Images</h3>
        <div className="flex gap-3 flex-wrap">
          {selectedProduct.images?.map((img: string, idx: number) => (
            <img
              key={idx}
              src={img}
              className="w-28 h-28 object-cover rounded border"
            />
          ))}
        </div>
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
        <div className="mt-6">
          <h3 className="font-semibold mb-2">Variants</h3>
          <div className="space-y-4">
            {selectedProduct.variants.map((v: any) => (
              <div key={v.id} className="border rounded p-3">
                <p><strong>SKU:</strong> {v.sku}</p>
                <p><strong>Price:</strong> {v.price.toLocaleString()}đ</p>
                <p><strong>Stock:</strong> {v.stock}</p>

                <div className="mt-2">
                  <strong>Attributes:</strong>
                  <ul className="list-disc ml-5 text-gray-700">
                    {v.attributes?.map((a: any, idx: number) => (
                      <li key={idx}>
                        <strong>{a.name}:</strong> {a.value.join(", ")}
                      </li>
                    ))}
                  </ul>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Close button */}
      <div className="mt-6 text-right">
        <button
          onClick={() => setOpenDetail(false)}
          className="px-4 py-2 bg-gray-600 text-white rounded"
        >
          Close
        </button>
      </div>
    </div>
  </div>
  )}
    
    </div>
  );
}
