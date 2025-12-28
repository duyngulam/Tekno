"use client";

import React, { useEffect, useState, useMemo } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { voucherApi } from "@/services/voucherApi";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Eye, Trash2, Edit2, Power, PowerOff, X } from "lucide-react";

export default function VoucherPage() {
  const [vouchers, setVouchers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [openCreate, setOpenCreate] = useState(false);
  const [openEdit, setOpenEdit] = useState(false);
  const [openDetail, setOpenDetail] = useState(false);
  const [selectedVoucher, setSelectedVoucher] = useState<any>(null);

  // Search + Filter
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");

  const [form, setForm] = useState({
    code: "",
    name: "",
    value: 0,
    quantity: 0,
    startDate: "",
    endDate: "",
    note: "",
  });

  // Fetch vouchers
  useEffect(() => {
    loadVouchers();
  }, []);

  const loadVouchers = async () => {
    try {
      setLoading(true);
      const json = await voucherApi.getAll();
      const list = Array.isArray(json?.data?.data)
        ? json.data.data
        : Array.isArray(json?.data)
        ? json.data
        : [];
      setVouchers(list);
    } catch (err) {
      console.error("Fetch error:", err);
      setVouchers([]);
    } finally {
      setLoading(false);
    }
  };

  // Handle Create
  const handleCreate = async () => {
    try {
      if (!form.code || !form.name || !form.startDate || !form.endDate) {
        alert("Please fill all required fields");
        return;
      }

      if (form.value <= 0) {
        alert("Value must be greater than 0");
        return;
      }

      if (form.quantity <= 0) {
        alert("Quantity must be greater than 0");
        return;
      }

      const payload = {
        code: form.code.toUpperCase(),
        name: form.name,
        type: "FixedAmount",
        value: Number(form.value),
        quantity: Number(form.quantity),
        note: form.note || "",
        startDate: new Date(form.startDate).toISOString(),
        endDate: new Date(form.endDate).toISOString(),
        applicableCategoryIds: [],
        applicableProductIds: [],
      };

      await voucherApi.create(payload);
      alert("Voucher created successfully!");
      
      await loadVouchers();
      setOpenCreate(false);
      resetForm();
    } catch (e) {
      console.error("Create error", e);
      alert("Failed to create voucher");
    }
  };

  // Handle Edit
  const openEditModal = async (voucher: any) => {
    try {
      const detail = await voucherApi.getById(voucher.id.toString());
      const data = detail?.data || detail;
      
      setSelectedVoucher(data);
      setForm({
        code: data.code,
        name: data.name,
        value: data.value,
        quantity: data.quantity,
        startDate: new Date(data.startDate).toISOString().slice(0, 16),
        endDate: new Date(data.endDate).toISOString().slice(0, 16),
        note: data.note || "",
      });
      setOpenEdit(true);
    } catch (err) {
      console.error("Failed to load voucher detail:", err);
      alert("Failed to load voucher details");
    }
  };

  const handleUpdate = async () => {
    try {
      if (!selectedVoucher) return;

      if (!form.code || !form.name || !form.startDate || !form.endDate) {
        alert("Please fill all required fields");
        return;
      }

      const payload = {
        code: form.code.toUpperCase(),
        name: form.name,
        type: "FixedAmount",
        value: Number(form.value),
        quantity: Number(form.quantity),
        note: form.note || "",
        startDate: new Date(form.startDate).toISOString(),
        endDate: new Date(form.endDate).toISOString(),
        applicableCategoryIds: [],
        applicableProductIds: [],
      };

      await voucherApi.update(selectedVoucher.id.toString(), payload);
      alert("Voucher updated successfully!");
      
      await loadVouchers();
      setOpenEdit(false);
      resetForm();
    } catch (err) {
      console.error("Update error:", err);
      alert("Failed to update voucher");
    }
  };

  // Handle Delete
  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this voucher?")) return;

    try {
      await voucherApi.delete(id);
      alert("Voucher deleted successfully!");
      await loadVouchers();
    } catch (err) {
      console.error("Delete error:", err);
      alert("Failed to delete voucher");
    }
  };

  // Handle Activate
  const handleActivate = async (id: string) => {
    try {
      await voucherApi.activate(id);
      alert("Voucher activated successfully!");
      await loadVouchers();
    } catch (err) {
      console.error("Activate error:", err);
      alert("Failed to activate voucher");
    }
  };

  // Handle Deactivate
  const handleDeactivate = async (id: string) => {
    try {
      await voucherApi.deactivate(id);
      alert("Voucher deactivated successfully!");
      await loadVouchers();
    } catch (err) {
      console.error("Deactivate error:", err);
      alert("Failed to deactivate voucher");
    }
  };

  // View Detail
  const handleViewDetail = async (voucher: any) => {
    try {
      const detail = await voucherApi.getById(voucher.id.toString());
      const data = detail?.data || detail;
      setSelectedVoucher(data);
      setOpenDetail(true);
    } catch (err) {
      console.error("Failed to load detail:", err);
      setSelectedVoucher(voucher);
      setOpenDetail(true);
    }
  };

  const resetForm = () => {
    setForm({
      code: "",
      name: "",
      value: 0,
      quantity: 0,
      startDate: "",
      endDate: "",
      note: "",
    });
    setSelectedVoucher(null);
  };

  // Filter + Search Logic
  const filteredVouchers = useMemo(() => {
    const today = new Date();

    return vouchers
      .map((v) => {
        const start = new Date(v.startDate);
        const end = new Date(v.endDate);

        let status = v.status || "Active";
        if (start > today) status = "Scheduled";
        else if (end < today) status = "Expired";
        else if (v.status === "Inactive") status = "Inactive";

        return { ...v, status };
      })
      .filter((v) => {
        const matchSearch =
          v.code?.toLowerCase().includes(search.toLowerCase()) ||
          v.name?.toLowerCase().includes(search.toLowerCase());

        const matchStatus = statusFilter === "All" || v.status === statusFilter;

        return matchSearch && matchStatus;
      });
  }, [vouchers, search, statusFilter]);

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center gap-5">
          <h2 className="text-xl font-semibold">Voucher Management</h2>
        </div>
        <Button onClick={() => setOpenCreate(true)}>+ Create Voucher</Button>
      </div>

      {/* Search + Filter UI */}
      <div className="flex items-center gap-4 mb-4">
        <input
          type="text"
          placeholder="Search by code or name..."
          className="border p-2 rounded w-64"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />

        <select
          className="border p-2 rounded"
          value={statusFilter}
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
        <div className="flex justify-center items-center py-12">
          <p className="text-gray-500">Loading vouchers...</p>
        </div>
      ) : vouchers.length === 0 ? (
        <p className="text-gray-500">No vouchers found.</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-sm bg-white shadow rounded">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2">Code</th>
                <th>Name</th>
                <th>Value</th>
                <th>Quantity</th>
                <th>Start</th>
                <th>End</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredVouchers.map((v) => (
                <tr className="border-b hover:bg-gray-50" key={v.id}>
                  <td className="p-2 font-mono font-medium text-blue-600">
                    {v.code}
                  </td>
                  <td>{v.name}</td>
                  <td className="font-medium">{v.value.toLocaleString()}đ</td>
                  <td>{v.quantity}</td>
                  <td className="text-xs">
                    {new Date(v.startDate).toLocaleDateString("vi-VN")}
                  </td>
                  <td className="text-xs">
                    {new Date(v.endDate).toLocaleDateString("vi-VN")}
                  </td>
                  <td>
                    <span
                      className={`px-2 py-1 rounded text-xs font-medium ${
                        v.status === "Active"
                          ? "bg-green-100 text-green-700"
                          : v.status === "Scheduled"
                          ? "bg-blue-100 text-blue-700"
                          : v.status === "Expired"
                          ? "bg-gray-100 text-gray-700"
                          : "bg-red-100 text-red-700"
                      }`}
                    >
                      {v.status}
                    </span>
                  </td>
                  <td className="p-2">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleViewDetail(v)}
                        className="p-1 text-blue-600 hover:bg-blue-50 rounded"
                        title="View Details"
                      >
                        <Eye size={16} />
                      </button>

                      <button
                        onClick={() => openEditModal(v)}
                        className="p-1 text-green-600 hover:bg-green-50 rounded"
                        title="Edit"
                      >
                        <Edit2 size={16} />
                      </button>

                      {v.status === "Active" || v.status === "Scheduled" ? (
                        <button
                          onClick={() => handleDeactivate(v.id.toString())}
                          className="p-1 text-orange-600 hover:bg-orange-50 rounded"
                          title="Deactivate"
                        >
                          <PowerOff size={16} />
                        </button>
                      ) : (
                        <button
                          onClick={() => handleActivate(v.id.toString())}
                          className="p-1 text-green-600 hover:bg-green-50 rounded"
                          title="Activate"
                        >
                          <Power size={16} />
                        </button>
                      )}

                      <button
                        onClick={() => handleDelete(v.id.toString())}
                        className="p-1 text-red-600 hover:bg-red-50 rounded"
                        title="Delete"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Create Voucher Modal */}
      <Dialog open={openCreate} onOpenChange={setOpenCreate}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Create Voucher</DialogTitle>
          </DialogHeader>

          <div className="grid gap-3 mt-2">
            <div>
              <label className="block text-sm font-medium mb-1">
                Code <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="e.g., SUMMER2025"
                value={form.code}
                onChange={(e) =>
                  setForm({ ...form, code: e.target.value.toUpperCase() })
                }
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Name <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="Voucher name"
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Value (đ) <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="Discount amount"
                type="number"
                min={0}
                value={form.value}
                onChange={(e) => setForm({ ...form, value: Number(e.target.value) })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Quantity <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="Number of vouchers"
                type="number"
                min={1}
                value={form.quantity}
                onChange={(e) =>
                  setForm({ ...form, quantity: Number(e.target.value) })
                }
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Start Date <span className="text-red-500">*</span>
              </label>
              <Input
                type="datetime-local"
                value={form.startDate}
                onChange={(e) => setForm({ ...form, startDate: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                End Date <span className="text-red-500">*</span>
              </label>
              <Input
                type="datetime-local"
                value={form.endDate}
                onChange={(e) => setForm({ ...form, endDate: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Note</label>
              <Input
                placeholder="Optional note"
                value={form.note}
                onChange={(e) => setForm({ ...form, note: e.target.value })}
              />
            </div>

            <Button onClick={handleCreate} className="mt-3">
              Create Voucher
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Edit Voucher Modal */}
      <Dialog open={openEdit} onOpenChange={setOpenEdit}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Edit Voucher</DialogTitle>
          </DialogHeader>

          <div className="grid gap-3 mt-2">
            <div>
              <label className="block text-sm font-medium mb-1">
                Code <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="e.g., SUMMER2025"
                value={form.code}
                onChange={(e) =>
                  setForm({ ...form, code: e.target.value.toUpperCase() })
                }
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Name <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="Voucher name"
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Value (đ) <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="Discount amount"
                type="number"
                min={0}
                value={form.value}
                onChange={(e) => setForm({ ...form, value: Number(e.target.value) })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Quantity <span className="text-red-500">*</span>
              </label>
              <Input
                placeholder="Number of vouchers"
                type="number"
                min={1}
                value={form.quantity}
                onChange={(e) =>
                  setForm({ ...form, quantity: Number(e.target.value) })
                }
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                Start Date <span className="text-red-500">*</span>
              </label>
              <Input
                type="datetime-local"
                value={form.startDate}
                onChange={(e) => setForm({ ...form, startDate: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">
                End Date <span className="text-red-500">*</span>
              </label>
              <Input
                type="datetime-local"
                value={form.endDate}
                onChange={(e) => setForm({ ...form, endDate: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Note</label>
              <Input
                placeholder="Optional note"
                value={form.note}
                onChange={(e) => setForm({ ...form, note: e.target.value })}
              />
            </div>

            <div className="flex gap-3 mt-3">
              <Button
                variant="outline"
                onClick={() => {
                  setOpenEdit(false);
                  resetForm();
                }}
                className="flex-1"
              >
                Cancel
              </Button>
              <Button onClick={handleUpdate} className="flex-1">
                Update Voucher
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>

      {/* Detail Modal */}
      {openDetail && selectedVoucher && (
        <Dialog open={openDetail} onOpenChange={setOpenDetail}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Voucher Details</DialogTitle>
            </DialogHeader>

            <div className="space-y-4 mt-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-600">
                    Code
                  </label>
                  <p className="text-lg font-mono font-bold text-blue-600">
                    {selectedVoucher.code}
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-600">
                    Status
                  </label>
                  <span
                    className={`inline-block px-3 py-1 rounded text-sm font-medium ${
                      selectedVoucher.status === "Active"
                        ? "bg-green-100 text-green-700"
                        : selectedVoucher.status === "Scheduled"
                        ? "bg-blue-100 text-blue-700"
                        : selectedVoucher.status === "Expired"
                        ? "bg-gray-100 text-gray-700"
                        : "bg-red-100 text-red-700"
                    }`}
                  >
                    {selectedVoucher.status || "Active"}
                  </span>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-600">
                    Name
                  </label>
                  <p className="text-sm font-medium">{selectedVoucher.name}</p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-600">
                    Discount Value
                  </label>
                  <p className="text-lg font-bold text-green-600">
                    {selectedVoucher.value.toLocaleString()}đ
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-600">
                    Quantity
                  </label>
                  <p className="text-sm">{selectedVoucher.quantity}</p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-600">
                    Type
                  </label>
                  <p className="text-sm">{selectedVoucher.type || "FixedAmount"}</p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-600">
                    Start Date
                  </label>
                  <p className="text-sm">
                    {new Date(selectedVoucher.startDate).toLocaleString("vi-VN")}
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-600">
                    End Date
                  </label>
                  <p className="text-sm">
                    {new Date(selectedVoucher.endDate).toLocaleString("vi-VN")}
                  </p>
                </div>
              </div>

              {selectedVoucher.note && (
                <div>
                  <label className="block text-sm font-medium text-gray-600 mb-1">
                    Note
                  </label>
                  <p className="text-sm bg-yellow-50 border border-yellow-200 rounded p-3">
                    {selectedVoucher.note}
                  </p>
                </div>
              )}

              <div className="flex justify-end gap-3 pt-4 border-t">
                <Button variant="outline" onClick={() => setOpenDetail(false)}>
                  Close
                </Button>
                <Button
                  onClick={() => {
                    setOpenDetail(false);
                    openEditModal(selectedVoucher);
                  }}
                  className="bg-green-600 hover:bg-green-700"
                >
                  <Edit2 className="w-4 h-4 mr-2" />
                  Edit
                </Button>
                <Button
                  variant="outline"
                  onClick={() => {
                    handleDelete(selectedVoucher.id.toString());
                    setOpenDetail(false);
                  }}
                  className="text-red-600 border-red-600"
                >
                  <Trash2 className="w-4 h-4 mr-2" />
                  Delete
                </Button>
              </div>
            </div>
          </DialogContent>
        </Dialog>
      )}
    </div>
  );
}