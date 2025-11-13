"use client";

import React, { useState, useMemo } from "react";
import Link from "next/link";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

export default function VoucherPage() {
  const [vouchers, setVouchers] = useState([
    {
      code: "PHVC000003",
      name: "Return",
      start: "2025-08-23",
      end: "2026-02-23",
      quantity: 10,
      value: 300000,

    },
    {
      code: "PHVC000002",
      name: "Summer",
      start: "2026-04-23",
      end: "2026-08-23",
      quantity: 10,
      value: 300000,

    },
    {
      code: "PHVC000001",
      name: "Holiday",
      start: "2021-08-23",
      end: "2022-02-23",
      quantity: 10,
      value: 300000,

    },
  ]);

  const [selected, setSelected] = useState<any>(null);
  const [activeTab, setActiveTab] = useState("Information");
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("Active");

const filteredVouchers = useMemo(() => {
  const now = new Date();

  return vouchers
    .map((v) => {
      const start = new Date(v.start);
      const end = new Date(v.end);

      let status = "Active";
      if (start > now) status = "Unactive";
      else if (end < now) status = "Expired";

      return { ...v, status };
    })
    .filter((v) => {
      const matchesSearch =
        v.name.toLowerCase().includes(search.toLowerCase()) ||
        v.code.toLowerCase().includes(search.toLowerCase());
      const matchesStatus =
        statusFilter === "All" || v.status === statusFilter;
      return matchesSearch && matchesStatus;
    });
}, [vouchers, search, statusFilter]);

  const handleDelete = () => {
    if (window.confirm(`Delete ${selected.name}?`)) {
      setVouchers((prev) => prev.filter((i) => i.code !== selected.code));
      setOpen(false);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow border border-gray-100 p-6">
      {/* Header */}
        <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-2">
          <Link
            href="/dashboard/catalog"
            className="text-secondary font-medium hover:underline"
          >
            ← Back
          </Link>
          <h2 className="text-xl font-semibold text-secondary ml-4">
            Voucher
          </h2>
        </div>
        <button
          className="bg-[#FFD500] text-black px-4 py-2 rounded-md font-medium hover:opacity-90 transition"
          >
          New release
        </button>
        </div>

      <div className="flex gap-4">
        {/* Sidebar */}
        <div className="w-1/4 bg-white rounded-lg p-4 shadow">
          <h4 className="font-medium text-gray-700 mb-2">Status</h4>
            <div className="flex flex-col gap-3 text-sm text-gray-600">
              {["All", "Active", "Unactive", "Expired"].map((s) => (
                <label key={s} className="flex items-center gap-2">
                  <input
                    type="radio"
                    name="status"
                    value={s}
                    checked={statusFilter === s}
                    onChange={(e) => setStatusFilter(e.target.value)}
                  />
                  {s}
                </label>
              ))}
            </div>
        </div>

        {/* Table */}
        <div className="flex-1 bg-gray-50 rounded-lg p-4">
          <div className="flex justify-between items-center mb-4">
            <input
              type="text"
              placeholder="Search by code, release name"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-2/3 border border-gray-300 rounded-md p-2 text-sm focus:outline-none focus:ring-1 focus:ring-[#FFD500]"
            />
          </div>

          <table className="w-full text-sm text-gray-700">
            <thead>
              <tr className="bg-[#FFD500] text-gray-800 font-medium">
                <th className="py-2 px-3 text-left">Release code</th>
                <th className="py-2 px-3 text-left">Release name</th>
                <th className="py-2 px-3 text-left">Start date</th>
                <th className="py-2 px-3 text-left">End date</th>
                <th className="py-2 px-3 text-left">Quantity</th>
                <th className="py-2 px-3 text-left">Value</th>
                <th className="py-2 px-3 text-left">Status</th>
              </tr>
            </thead>
            <tbody>
              {filteredVouchers.map((v, i) => (
                <tr
                  key={i}
                  onClick={() => {
                    setSelected(v);
                    setOpen(true);
                    setActiveTab("Information");
                  }}
                  className="border-b border-gray-200 hover:bg-gray-100 cursor-pointer transition"
                >
                <td className="py-2 px-3">{v.code}</td>
                <td className="py-2 px-3">{v.name}</td>
                <td className="py-2 px-3">
                    {new Date(v.start).toLocaleDateString("vi-VN")}
                </td>
                <td className="py-2 px-3">
                    {new Date(v.end).toLocaleDateString("vi-VN")}
                </td>
                <td className="py-2 px-3">{v.quantity}</td>
                <td className="py-2 px-3">{v.value.toLocaleString()}</td>
                <td
                    className={
                        v.status === "Active"
                            ? "py-2 px-3 text-green-700"
                            : v.status === "Unactive"
                            ? "py-2 px-3 text-gray-500"
                            : "py-2 px-3 text-red-600"
                        }
                    >
                    {v.status}
                </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Detail Dialog */}
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-3xl rounded-2xl">
          <DialogHeader>
            <DialogTitle className="text-secondary">Voucher detail</DialogTitle>
          </DialogHeader>

          {selected && (
            <div className="mt-4 text-sm">
              {/* Tabs */}
              <div className="border-b border-gray-200 mb-4 flex gap-6 text-gray-700">
                {["Information", "Voucher list", "Usage history"].map((t) => (
                  <button
                    key={t}
                    onClick={() => setActiveTab(t)}
                    className={`pb-1 ${
                      activeTab === t
                        ? "border-b-2 border-[#FFD500] text-[#FFD500] font-medium"
                        : "hover:text-[#FFD500]"
                    }`}
                  >
                    {t}
                  </button>
                ))}
              </div>

              {/* Tab content */}
              {activeTab === "Information" && (
                <div className="grid grid-cols-2 gap-4">
                  <p>
                    <strong>Release Code:</strong> {selected.code}
                  </p>
                  <p>
                    <strong>Release Name:</strong> {selected.name}
                  </p>
                  <p>
                    <strong>Period:</strong>{" "}
                    {new Date(selected.start).toLocaleDateString("vi-VN")} -{" "}
                    {new Date(selected.end).toLocaleDateString("vi-VN")}
                  </p>
                  <p>
                    <strong>Value:</strong>{" "}
                    {selected.value.toLocaleString("vi-VN")}
                  </p>
                  <p>
                    <strong>Applicable Product/Category:</strong> All categories
                  </p>
                  <p>
                    <strong>Minimum Purchase Amount:</strong> 0
                  </p>
                  <p>
                    <strong>Status:</strong>{" "}
                    <span className="text-green-700">{selected.status}</span>
                  </p>
                  <p>
                    <strong>Note:</strong>
                  </p>
                </div>
              )}

              {activeTab === "Voucher list" && (
                <p className="text-gray-600 italic">
                  Voucher list details will be displayed here.
                </p>
              )}

              {activeTab === "Usage history" && (
                <p className="text-gray-600 italic">
                  Usage history will be displayed here.
                </p>
              )}

              <div className="flex justify-end gap-3 mt-6">
                <button className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md">
                  Update
                </button>
                <button
                  onClick={handleDelete}
                  className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md"
                >
                  Delete
                </button>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}
