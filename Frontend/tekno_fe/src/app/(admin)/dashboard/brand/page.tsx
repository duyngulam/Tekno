"use client";

import React, { useEffect, useState } from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Link from "next/link";
import { get, postForm } from "@/lib/api";

export default function BrandPage() {
  const [brands, setBrands] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [openCreate, setOpenCreate] = useState(false);
  const [search, setSearch] = useState("");

  const [form, setForm] = useState({
    name: "",
    country: "",
    isActive: true,
    image: null as File | null,
  });

  useEffect(() => {
    const fetchBrands = async () => {
      try {
        const json = await get("http://localhost:5000/api/admin/brands/list", { cache: "no-store" });
        const list = Array.isArray(json?.data?.data)
          ? json.data.data
          : Array.isArray(json?.data)
          ? json.data
          : [];
        setBrands(list);
      } catch (err) {
        console.error("Fetch brands error:", err);
        setBrands([]);
      } finally {
        setLoading(false);
      }
    };

    fetchBrands();
  }, []);

  const handleCreate = async () => {
    try {
      if (!form.name) {
        alert("Please enter brand name");
        return;
      }

      const fd = new FormData();
      if (form.image) fd.append("image", form.image);
      fd.append("Name", form.name);
      fd.append("Country", form.country);
      fd.append("IsActive", String(form.isActive));

      await postForm("http://localhost:5000/api/admin/brands", fd);

      // refresh
      const json = await get("http://localhost:5000/api/admin/brands/list", { cache: "no-store" });
      const list = Array.isArray(json?.data?.data) ? json.data.data : Array.isArray(json?.data) ? json.data : [];
      setBrands(list);

      setOpenCreate(false);
      setForm({ name: "", country: "", isActive: true, image: null });
      alert("Brand created successfully");
    } catch (e) {
      console.error("Create brand error", e);
      alert("Failed to create brand");
    }
  };

  const filtered = brands.filter((b) =>
    (b.name || "").toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center gap-5">
          <h2 className="text-xl font-semibold">Brands</h2>
        </div>
        <Button onClick={() => setOpenCreate(true)}>+ Create Brand</Button>
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
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <Dialog open={openCreate} onOpenChange={setOpenCreate}>
        <DialogContent className="max-w-md max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Create Brand</DialogTitle>
          </DialogHeader>

          <div className="grid gap-3 mt-2">
            <div>
              <label className="block text-sm font-medium mb-1">Name *</label>
              <Input placeholder="Brand name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
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

            <div className="flex items-center gap-2">
              <input type="checkbox" id="isActive" checked={form.isActive} onChange={(e) => setForm({ ...form, isActive: e.target.checked })} className="w-4 h-4" />
              <label htmlFor="isActive" className="text-sm font-medium">Active</label>
            </div>

            <Button onClick={handleCreate} className="mt-3">Create Brand</Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}