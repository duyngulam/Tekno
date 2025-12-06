"use client";

import React, { useEffect, useMemo, useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { get } from "@/lib/api";
import { ChevronDown, ChevronRight } from "lucide-react";
import Actions from "@/components/admin/Actions"; // adjust path if different

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

    let list: any[] = [];

    if (Array.isArray(res)) {
      list = res;
    }
    else if (Array.isArray(res?.data)) {
      list = res.data;
    }
    else if (Array.isArray(res?.data?.data)) {
      list = res.data.data;   // ⭐ TRƯỜNG HỢP CỦA BẠN
    }
    else {
      console.error("Unrecognized brand API format:", res);
    }

    setBrands(list);
  } catch (err) {
    console.error("Brand load failed", err);
    setBrands([]);
  }
};


const loadCategories = async () => {
  try {
    const cRes = await get("http://localhost:5000/api/admin/categories/tree");
    let list = cRes?.data?.data || cRes?.data || cRes || [];
    list = assignParentIds(list);
    setTreeCategories(list);
  } catch (err) {
    console.error("Category load failed", err);
    setTreeCategories([]);
  }
};

const loadProducts = async () => {
  try {
    const pRes = await get("http://localhost:5000/api/admin/products");
    const list = pRes?.data?.data || pRes?.data || [];
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
const openEditModal = (prod: any) => {
  // Map brand name → brandId
  const matchedBrand = brands.find(
    (b: any) =>
      b.name?.toLowerCase() === prod.brandName?.toLowerCase() ||
      b.brandName?.toLowerCase() === prod.brandName?.toLowerCase()
  );

  // Map category name → categoryId
  const matchedCategory = flatCategories.find(
    (c: any) =>
      c.name.replace(/—/g, "").trim().toLowerCase() ===
      prod.categoryName?.toLowerCase()
  );

  setEditData({
    id: prod.id,
    name: prod.name,
    slug: prod.slug,

    brandId: matchedBrand?.id ?? "",
    categoryId: matchedCategory?.id ?? "",

    basePrice: prod.basePrice,
    discountPercent: prod.discountPercent,
    overview: prod.overview ?? "",

    primaryImagePath: prod.primaryImagePath ?? null,
    images: [],
  });

  setOpenEdit(true);
};


  const handleEditSave = async () => {
    try {
      if (!editData || !editData.id) return;

      if (!editData.name || !editData.slug || !editData.categoryId || !editData.brandId) {
        alert("Please fill required fields: Name, Slug, Category, Brand");
        return;
      }

      const fd = new FormData();
      fd.append("Name", editData.name);
      fd.append("Slug", editData.slug);
      fd.append("CategoryId", String(editData.categoryId));
      fd.append("BrandId", String(editData.brandId));
      if (editData.basePrice !== undefined) fd.append("BasePrice", String(editData.basePrice));
      if (editData.discountPercent !== undefined) fd.append("DiscountPercent", String(editData.discountPercent));
      if (editData.overview) fd.append("Overview", editData.overview);

      // append new images
      for (const f of (editData.images || []) as File[]) {
        fd.append("Images", f);
      }

      const res = await fetch(`http://localhost:5000/api/admin/products/${editData.id}`, {
        method: "PUT",
        body: fd,
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(`Update failed: ${text}`);
      }

      await loadAll();
      setOpenEdit(false);
      setEditData(null);
      alert("Product updated");
    } catch (err) {
      console.error(err);
      alert("Update failed");
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
                    {p.primaryImagePath ? (
                      <img src={p.primaryImagePath} alt={p.name} className="w-20 h-12 object-cover rounded" />
                    ) : (
                      <div className="w-20 h-12 bg-gray-100 rounded flex items-center justify-center text-xs text-gray-500">No image</div>
                    )}
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

      {/* CREATE MODAL */}
      <Dialog open={openCreate} onOpenChange={setOpenCreate}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Create Product</DialogTitle>
          </DialogHeader>

          <div className="grid gap-3 mt-2">
            <div>
              <label className="block text-sm font-medium">Name *</label>
              <Input value={createData.name} onChange={(e) => setCreateData({ ...createData, name: e.target.value })} />
            </div>

            <div>
              <label className="block text-sm font-medium">Slug *</label>
              <Input value={createData.slug} onChange={(e) => setCreateData({ ...createData, slug: e.target.value })} />
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-sm font-medium">Brand *</label>
                <select className="border p-2 rounded w-full" value={String(createData.brandId)} onChange={(e) => setCreateData({ ...createData, brandId: e.target.value })}>
                  <option value="">-- Select Brand --</option>
                  {brands.map((b: any) => <option key={b.id} value={b.id}>{b.name ?? b.brandName ?? b.title}</option>)}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium">Category *</label>
                <select className="border p-2 rounded w-full" value={String(createData.categoryId)} onChange={(e) => setCreateData({ ...createData, categoryId: e.target.value })}>
                  <option value="">-- Select Category --</option>
                  {flatCategories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="block text-sm font-medium">Base Price</label>
                <Input type="number" value={String(createData.basePrice)} onChange={(e) => setCreateData({ ...createData, basePrice: Number(e.target.value) })} />
              </div>

              <div>
                <label className="block text-sm font-medium">Discount Percent</label>
                <Input type="number" value={String(createData.discountPercent)} onChange={(e) => setCreateData({ ...createData, discountPercent: Number(e.target.value) })} />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium">Overview</label>
              <textarea value={createData.overview} onChange={(e) => setCreateData({ ...createData, overview: e.target.value })} className="w-full border p-2 rounded" />
            </div>

            <div>
              <label className="block text-sm font-medium">Images (you can select multiple)</label>
              <input type="file" accept="image/*" multiple onChange={(e) => setCreateData({ ...createData, images: Array.from(e.target.files || []) })} />
            </div>

            <div>
              <div>Final price (preview): <strong>{calcFinalPrice(createData.basePrice, createData.discountPercent)}</strong></div>
            </div>

              <Button onClick={handleCreate}>Create Product</Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* EDIT MODAL */}
      <Dialog open={openEdit} onOpenChange={setOpenEdit}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Edit Product</DialogTitle>
          </DialogHeader>

          {editData && (
            <div className="grid gap-3 mt-2">
              <div>
                <label className="block text-sm font-medium">Name *</label>
                <Input value={editData.name} onChange={(e) => setEditData({ ...editData, name: e.target.value })} />
              </div>

              <div>
                <label className="block text-sm font-medium">Slug *</label>
                <Input value={editData.slug} onChange={(e) => setEditData({ ...editData, slug: e.target.value })} />
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium">Brand *</label>
                  <select className="border p-2 rounded w-full" value={String(editData.brandId || "")} onChange={(e) => setEditData({ ...editData, brandId: e.target.value })}>
                    <option value="">-- Select Brand --</option>
                    {brands.map((b: any) => <option key={b.id} value={b.id}>{b.name ?? b.brandName ?? b.title}</option>)}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium">Category *</label>
                  <select className="border p-2 rounded w-full" value={String(editData.categoryId || "")} onChange={(e) => setEditData({ ...editData, categoryId: e.target.value })}>
                    <option value="">-- Select Category --</option>
                    {flatCategories.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium">Base Price</label>
                  <Input type="number" value={String(editData.basePrice || 0)} onChange={(e) => setEditData({ ...editData, basePrice: Number(e.target.value) })} />
                </div>

                <div>
                  <label className="block text-sm font-medium">Discount Percent</label>
                  <Input type="number" value={String(editData.discountPercent || 0)} onChange={(e) => setEditData({ ...editData, discountPercent: Number(e.target.value) })} />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium">Overview</label>
                <textarea value={editData.overview || ""} onChange={(e) => setEditData({ ...editData, overview: e.target.value })} className="w-full border p-2 rounded" />
              </div>

              <div>
                <label className="block text-sm font-medium">Images (add new images to upload)</label>
                <input type="file" accept="image/*" multiple onChange={(e) => setEditData({ ...editData, images: Array.from(e.target.files || []) })} />
              </div>

              <div>
                <label className="block text-sm font-medium">Existing primary image</label>
                {editData.primaryImagePath ? (
                  <img src={editData.primaryImagePath} className="w-28 h-20 object-cover rounded border" />
                ) : (
                  <div className="w-28 h-20 bg-gray-100 rounded flex items-center justify-center text-xs text-gray-500">No image</div>
                )}
              </div>

              <div>
                <div>Final price (preview): <strong>{calcFinalPrice(editData.basePrice, editData.discountPercent)}</strong></div>
              </div>

                <Button onClick={handleEditSave}>Save</Button>
            </div>
          )}
        </DialogContent>
      </Dialog>
    
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
