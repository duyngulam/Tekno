"use client";

import React, { useEffect, useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Link from "next/link";
import  Actions  from "@/components/admin/Actions";
import { getBrandList, createBrand, updateBrand, deleteBrand } from "@/services/brand";

export default function BrandPage() {
  const [brands, setBrands] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [openCreate, setOpenCreate] = useState(false);
  const [search, setSearch] = useState("");
  
  const [openEdit, setOpenEdit] = useState(false);
  const [editingBrand, setEditingBrand] = useState<any>(null);


  const [form, setForm] = useState({
    name: "",
    slug: "",
    country: "",
    image: null as File | null,
  });

const fetchBrands = async () => {
  try {
    const res = await getBrandList();

    const list = Array.isArray(res?.data)
      ? res.data
      : Array.isArray(res?.data?.data)
      ? res.data.data
      : [];

    setBrands(list);
  } catch (err) {
    console.error(err);
    setBrands([]);
  } finally {
    setLoading(false);
  }
};
  useEffect(() => {
    fetchBrands();
  }, []);

const handleCreate = async () => {
  try {
    if (!form.name || !form.slug) {
      alert("Name and Slug are required");
      return;
    }

    const fd = new FormData();
    fd.append("Name", form.name);
    fd.append("Slug", form.slug);
    fd.append("Country", form.country);
    if (form.image) fd.append("image", form.image);

    await createBrand(fd);

    await fetchBrands(); // refresh list

    setOpenCreate(false);
    setForm({ name: "", slug: "", country: "", image: null });
  } catch (e: any) {
    alert(e.message || "Create brand failed");
  }
};

  const filtered = brands.filter((b) =>
    (b.name || "").toLowerCase().includes(search.toLowerCase())
  );

const handleEdit = (brand: any) => {
  setEditingBrand(brand);
  setForm({
    name: brand.name || "",
    slug: brand.slug || "",
    country: brand.country || "",
    image: null,
  });
  setOpenEdit(true);
};

const handleUpdate = async () => {
  if (!editingBrand?.id) return;

  if (!form.name || !form.slug) {
    alert("Name and Slug are required");
    return;
  }

  try {
    const fd = new FormData();
    fd.append("Name", form.name);
    fd.append("Slug", form.slug);
    fd.append("Country", form.country);
    if (form.image) fd.append("image", form.image);

    await updateBrand(editingBrand.id, fd);

    await fetchBrands();

    setOpenEdit(false);
    setEditingBrand(null);
    setForm({ name: "", slug: "", country: "", image: null });
  } catch (e: any) {
    alert(e.message || "Update brand failed");
  }
};


const handleDelete = async (id: string) => {
  if (!confirm("Are you sure you want to delete this brand?")) return;

  try {
    await deleteBrand(id);
    setBrands((prev) => prev.filter((b) => b.id !== id));
  } catch (e: any) {
    alert(e.message || "Delete brand failed");
  }
};


  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center gap-5">
          <h2 className="text-xl font-semibold">Brands</h2>
        </div>
        <Button
          onClick={() => {
          setForm({ name: "", slug: "", country: "", image: null });
          setEditingBrand(null);
          setOpenCreate(true);
          }}
          >
          + Create Brand
        </Button>

      </div>

      <div className="flex items-center gap-4 mb-4">
        <input
          type="text"
          placeholder="Search brands..."
          className="border p-2 rounded w-80"
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : filtered.length === 0 ? (
        <p className="text-gray-500">No brands found.</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-sm bg-white shadow rounded">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2">ID</th>
                <th>Logo</th>
                <th>Name</th>
                <th>Country</th>
                <th>  </th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((b) => (
                <tr className="border-b hover:bg-gray-50" key={b.id}>
                  <td className="p-2">{b.id}</td>
                  <td>
                    {b.logoPath && (
                      <img src={b.logoPath} alt={b.name || 'Brand'} className="h-12 w-auto object-contain" />
                    )}
                  </td>
                  <td>{b.name}</td>
                  <td>{b.country}</td>
                  <td>
                    <Actions
                      onEdit={() => handleEdit(b)}
                      onDelete={() => handleDelete(b.id)}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

{/*EDIT DIALOG*/}
<Dialog open={openEdit} onOpenChange={setOpenEdit}>
  <DialogContent className="max-w-md">
    <DialogHeader>
      <DialogTitle>Edit Brand</DialogTitle>
    </DialogHeader>

    <div className="grid gap-3 mt-2">
      <Input
        placeholder="Brand name*"
        value={form.name}
        onChange={(e) => setForm({ ...form, name: e.target.value })}
      />

      <Input
        placeholder="Slug*"
        value={form.slug}
        onChange={(e) => setForm({ ...form, slug: e.target.value })}
      />

      <Input
        placeholder="Country"
        value={form.country}
        onChange={(e) => setForm({ ...form, country: e.target.value })}
      />

      <Input
        type="file"
        accept="image/*"
        onChange={(e) => setForm({ ...form, image: e.target.files?.[0] || null })}
      />

      <Button onClick={handleUpdate}>Update Brand</Button>
    </div>
  </DialogContent>
</Dialog>

      {/*CREATE DIALOG*/}
      <Dialog open={openCreate} onOpenChange={setOpenCreate}>
        <DialogContent className="max-w-md max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Create Brand</DialogTitle>
          </DialogHeader>

          <div className="grid gap-3 mt-2">
            <div>
              <label className="block text-sm font-medium mb-1">Name*</label>
              <Input placeholder="Brand name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Slug*</label>
              <Input placeholder="e.g., brand-slug" value={form.slug} onChange={(e) => setForm({ ...form, slug: e.target.value })} />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Country</label>
              <Input placeholder="e.g., USA, Vietnam" value={form.country} onChange={(e) => setForm({ ...form, country: e.target.value })} />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Image</label>
              <Input type="file" accept="image/*" onChange={(e) => setForm({ ...form, image: e.target.files?.[0] || null })} />
              {form.image && <p className="text-xs text-gray-600 mt-1">Selected: {form.image.name}</p>}
            </div>

            <Button onClick={handleCreate} className="mt-3">Create Brand</Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}