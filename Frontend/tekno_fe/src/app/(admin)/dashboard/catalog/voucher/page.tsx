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
import Link from "next/link";

export default function VoucherPage() {
  const [vouchers, setVouchers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [openCreate, setOpenCreate] = useState(false);

  // ⭐ NEW: Search + Filter
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
    const fetchVouchers = async () => {
      try {
        const res = await fetch("http://localhost:5000/api/admin/coupons", {
          method: "GET",
          cache: "no-store",
          headers: { "Content-Type": "application/json" },
        });

        const json = await res.json();
        const list = Array.isArray(json?.data?.data) ? json.data.data : [];
        setVouchers(list);
      } catch (err) {
        console.error("Fetch error:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchVouchers();
  }, []);

  // Handle Create
  const handleCreate = async () => {
    try {
      const payload = {
        code: form.code,
        name: form.name,
        type: "FixedAmount",
        value: Number(form.value),
        quantity: Number(form.quantity),
        note: form.note,
        startDate: new Date(form.startDate).toISOString(),
        endDate: new Date(form.endDate).toISOString(),
        applicableCategoryIds: [],
        applicableProductIds: [],
      };

      const res = await voucherApi.create(payload);

      setVouchers((prev) => [...prev, res.data]);
      setOpenCreate(false);
      setForm({
        code: "",
        name: "",
        value: 0,
        quantity: 0,
        startDate: "",
        endDate: "",
        note: "",
      });
    } catch (e) {
      console.error("Create error", e);
    }
  };

  // ⭐ NEW: Filter + Search Logic
  const filteredVouchers = useMemo(() => {
    const today = new Date();

    return vouchers
      .map((v) => {
        const start = new Date(v.startDate);
        const end = new Date(v.endDate);

        let status = v.status;
        if (start > today) status = "Unactive";
        else if (end < today) status = "Expired";

        return { ...v, status };
      })
      .filter((v) => {
        const matchSearch =
          v.code.toLowerCase().includes(search.toLowerCase()) ||
          v.name.toLowerCase().includes(search.toLowerCase());

        const matchStatus =
          statusFilter === "All" || v.status === statusFilter;

        return matchSearch && matchStatus;
      });
  }, [vouchers, search, statusFilter]);

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center gap-5">
        <Link
            href="/dashboard/catalog"
            className="text-secondary font-medium hover:underline"
          >
            ← Back
          </Link>
        <h2 className="text-xl font-semibold">Voucher</h2>
        </div>
        <Button onClick={() => setOpenCreate(true)}>+ Create Voucher</Button>
      </div>

      {/* ⭐ NEW: Search + Filter UI */}
      <div className="flex items-center gap-4 mb-4">
        <input
          type="text"
          placeholder="Search by code or name..."
          className="border p-2 rounded w-64"
          onChange={(e) => setSearch(e.target.value)}
        />

        <select
          className="border p-2 rounded"
          onChange={(e) => setStatusFilter(e.target.value)}
        >
          <option value="All">All Status</option>
          <option value="Active">Active</option>
          <option value="Unactive">Unactive</option>
          <option value="Expired">Expired</option>
        </select>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : (
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
            </tr>
          </thead>
          <tbody>
            {filteredVouchers.map((v) => (
              <tr className="border-b" key={v.id}>
                <td className="p-2">{v.code}</td>
                <td>{v.name}</td>
                <td>{v.value.toLocaleString()}</td>
                <td>{v.quantity}</td>
                <td>{new Date(v.startDate).toLocaleDateString()}</td>
                <td>{new Date(v.endDate).toLocaleDateString()}</td>
                <td>{v.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {/* Create Voucher Modal */}
      <Dialog open={openCreate} onOpenChange={setOpenCreate}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Create Voucher</DialogTitle>
          </DialogHeader>

          <div className="grid gap-3 mt-2">
            <Input
              placeholder="Code"
              value={form.code}
              onChange={(e) => setForm({ ...form, code: e.target.value })}
            />
            <Input
              placeholder="Name"
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
            />
            <label>Value</label>
            <Input
              placeholder="Value"
              type="number"
              value={form.value}
              onChange={(e) => setForm({ ...form, value: Number(e.target.value) })}
            />
            <label>Quantity</label>
            <Input
              placeholder="Quantity"
              type="number"
              value={form.quantity}
              onChange={(e) =>
                setForm({ ...form, quantity: Number(e.target.value) })
              }
            />
            <label>Start Date</label>
            <Input
              type="datetime-local"
              value={form.startDate}
              onChange={(e) => setForm({ ...form, startDate: e.target.value })}
            />

            <label>End Date</label>
            <Input
              type="datetime-local"
              value={form.endDate}
              onChange={(e) => setForm({ ...form, endDate: e.target.value })}
            />

            <Input
              placeholder="Note"
              value={form.note}
              onChange={(e) => setForm({ ...form, note: e.target.value })}
            />

            <Button onClick={handleCreate} className="mt-3">
              Create
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
