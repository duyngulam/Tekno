"use client";

import React, { useEffect, useState, useMemo } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Link from "next/link";
import { get, postForm } from "@/lib/api";

export default function AdvertisementPage() {
  const [advertisements, setAdvertisements] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [openCreate, setOpenCreate] = useState(false);

  // Search + Filter
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");

  const [form, setForm] = useState({
    productId: "",
    position: "",
    priority: 100,
    startDate: "",
    endDate: "",
    isActive: true,
    image: null as File | null,
  });

  // Fetch advertisements
  useEffect(() => {
    const fetchAdvertisements = async () => {
      try {
        const json = await get("http://localhost:5000/api/admin/advertisements", { cache: "no-store" });
        console.log("API Response:", json);

        const list = Array.isArray(json?.data?.data)
          ? json.data.data
          : Array.isArray(json?.data)
          ? json.data
          : [];

        setAdvertisements(list);
      } catch (err) {
        console.error("Fetch error:", err);
        setAdvertisements([]);
      } finally {
        setLoading(false);
      }
    };

    fetchAdvertisements();
  }, []);

  // Handle Create with FormData (for image upload)
  const handleCreate = async () => {
    try {
      if (!form.image) {
        alert("Please select an image");
        return;
      }

      const formData = new FormData();
      formData.append("image", form.image);
      formData.append("ProductId", form.productId);
      formData.append("Position", form.position);
      formData.append("Priority", String(form.priority));
      formData.append("StartDate", new Date(form.startDate).toISOString());
      formData.append("EndDate", new Date(form.endDate).toISOString());
      formData.append("IsActive", String(form.isActive));

      // Use helper that automatically attaches Authorization header when token present
      const createJson = await postForm("http://localhost:5000/api/admin/advertisements", formData);

      // Refresh list
      const refreshJson = await get("http://localhost:5000/api/admin/advertisements");
      const list = Array.isArray(refreshJson?.data?.data) ? refreshJson.data.data : [];
      setAdvertisements(list);

      setOpenCreate(false);
      setForm({
        productId: "",
        position: "",
        priority: 100,
        startDate: "",
        endDate: "",
        isActive: true,
        image: null,
      });

      alert("Advertisement created successfully!");
    } catch (e) {
      console.error("Create error", e);
      alert("Failed to create advertisement");
    }
  };

  // Filter + Search Logic
  const filteredAdvertisements = useMemo(() => {
    const today = new Date();

    return advertisements
      .map((ad) => {
        const start = new Date(ad.startDate);
        const end = new Date(ad.endDate);

        let status = "Active";
        if (!ad.isActive) status = "Inactive";
        else if (start > today) status = "Scheduled";
        else if (end < today) status = "Expired";

        return { ...ad, status };
      })
      .filter((ad) => {
        const matchSearch =
          ad.productName?.toLowerCase().includes(search.toLowerCase()) ||
          ad.position?.toLowerCase().includes(search.toLowerCase()) ||
          ad.productId?.toString().includes(search);

        const matchStatus =
          statusFilter === "All" || ad.status === statusFilter;

        return matchSearch && matchStatus;
      });
  }, [advertisements, search, statusFilter]);

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center gap-5">
          <h2 className="text-xl font-semibold">Advertisement</h2>
        </div>
        <Button onClick={() => setOpenCreate(true)}>+ Create Advertisement</Button>
      </div>

      {/* Search + Filter UI */}
      <div className="flex items-center gap-4 mb-4">
        <input
          type="text"
          placeholder="Search by product name, position, or ID..."
          className="border p-2 rounded w-80"
          onChange={(e) => setSearch(e.target.value)}
        />

        <select
          className="border p-2 rounded"
          onChange={(e) => setStatusFilter(e.target.value)}
        >
          <option value="All">All Status</option>
          <option value="Active">Active</option>
          <option value="Inactive">Inactive</option>
          <option value="Scheduled">Scheduled</option>
          <option value="Expired">Expired</option>
        </select>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : advertisements.length === 0 ? (
        <p className="text-gray-500">No advertisements found.</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-sm bg-white shadow rounded">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2">ID</th>
                <th>Image</th>
                <th>Product</th>
                <th>Position</th>
                <th>Priority</th>
                <th>Start</th>
                <th>End</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {filteredAdvertisements.map((ad) => (
                <tr className="border-b hover:bg-gray-50" key={ad.id}>
                  <td className="p-2">{ad.id}</td>
                  <td>
                    {ad.imageUrl && (
                      <img 
                        src={ad.imageUrl} 
                        alt={ad.productName || "Ad"} 
                        className="w-20 h-12 object-cover rounded"
                      />
                    )}
                  </td>
                  <td>
                    <div className="font-medium">{ad.productName || `Product #${ad.productId}`}</div>
                    <div className="text-xs text-gray-500">ID: {ad.productId}</div>
                  </td>
                  <td>{ad.position}</td>
                  <td>{ad.priority}</td>
                  <td>{new Date(ad.startDate).toLocaleDateString()}</td>
                  <td>{new Date(ad.endDate).toLocaleDateString()}</td>
                  <td>
                    <span className={`px-2 py-1 rounded text-xs font-medium ${
                      ad.status === 'Active' ? 'bg-green-100 text-green-700' :
                      ad.status === 'Scheduled' ? 'bg-blue-100 text-blue-700' :
                      ad.status === 'Expired' ? 'bg-gray-100 text-gray-700' :
                      'bg-red-100 text-red-700'
                    }`}>
                      {ad.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Create Advertisement Modal */}
      <Dialog open={openCreate} onOpenChange={setOpenCreate}>
        <DialogContent className="max-w-md max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Create Advertisement</DialogTitle>
          </DialogHeader>

          <div className="grid gap-3 mt-2">
            <div>
              <label className="block text-sm font-medium mb-1">Product ID *</label>
              <Input
                placeholder="Enter product ID"
                type="number"
                value={form.productId}
                onChange={(e) => setForm({ ...form, productId: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Position *</label>
              <Input
                placeholder="e.g., Homepage Banner, Sidebar"
                value={form.position}
                onChange={(e) => setForm({ ...form, position: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Priority</label>
              <Input
                placeholder="Priority (higher = more important)"
                type="number"
                value={form.priority}
                onChange={(e) => setForm({ ...form, priority: Number(e.target.value) })}
              />
              <p className="text-xs text-gray-500 mt-1">Higher priority shows first</p>
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Image *</label>
              <Input
                type="file"
                accept="image/*"
                onChange={(e) => {
                  const file = e.target.files?.[0] || null;
                  setForm({ ...form, image: file });
                }}
              />
              {form.image && (
                <p className="text-xs text-gray-600 mt-1">Selected: {form.image.name}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Start Date *</label>
              <Input
                type="datetime-local"
                value={form.startDate}
                onChange={(e) => setForm({ ...form, startDate: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">End Date *</label>
              <Input
                type="datetime-local"
                value={form.endDate}
                onChange={(e) => setForm({ ...form, endDate: e.target.value })}
              />
            </div>

            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="isActive"
                checked={form.isActive}
                onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
                className="w-4 h-4"
              />
              <label htmlFor="isActive" className="text-sm font-medium">
                Active immediately
              </label>
            </div>

            <Button onClick={handleCreate} className="mt-3">
              Create Advertisement
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}