"use client";

import React, { useState, useMemo } from "react";
import Link from "next/link";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
interface Condition {
  purchaseFrom: string;
  discount: string;
  discountType: string;
}


export default function DiscountPage() {
  const [discounts, setDiscounts] = useState([
    {
      code: "KM000054",
      name: "Earth Hour",
      from: "2025-08-05T18:27:00",
      to: "2026-02-05T18:27:00",
      status: "Active",
      method: "Discount on items",
    },
    {
      code: "KM000055",
      name: "Summer Sale",
      from: "2025-07-01T08:00:00",
      to: "2025-07-31T23:59:00",
      status: "Unactive",
      method: "Discount on items",
    },
    {
      code: "KM000056",
      name: "New Year Promo",
      from: "2025-01-01T00:00:00",
      to: "2026-01-31T23:59:00",
      status: "Active",
      method: "Discount on items",
    },
  ]);

  const [selected, setSelected] = useState<any>(null);
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState(false);

  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [validityFilter, setValidityFilter] = useState("All");

  const [activeTab, setActiveTab] = useState("Information");
  
// Popup state
const [openNew, setOpenNew] = useState(false);
// Form values
const [newForm, setNewForm] = useState<{
  name: string;
  code: string;
  from: string;
  to: string;
  status: string;
  basedOn: string;
  discountType: string;
  limit: string;
  customerGroupType: string;
  customerGroup: string;
  notes: string;
}>({
  name: "",
  code: "",
  from: "",
  to: "",
  status: "Active",
  basedOn: "Product",
  discountType: "Product discount",
  limit: "multiple",
  customerGroupType: "all",
  customerGroup: "",
  notes: "",
});

const generateCode = () => {
  const num = Math.floor(100000 + Math.random() * 900000);
  return "KM" + num;
};

const validateForm = () => {
  if (!newForm.name || !newForm.name.trim()) return "Campaign name is required";
  if (!newForm.from) return "Start date is required";
  if (!newForm.to) return "End date is required";

  // Kiểm tra kiểu ngày: from < to (tùy ý)
  const fromTime = Date.parse(newForm.from);
  const toTime = Date.parse(newForm.to);
  if (isNaN(fromTime)) return "Start date is invalid";
  if (isNaN(toTime)) return "End date is invalid";
  if (fromTime >= toTime) return "Start date must be before end date";

  // Customer group
  if (
    newForm.customerGroupType === "specific" &&
    !newForm.customerGroup
  )
    return "Select customer group";

  return "";
};
const handleSaveNew = () => {
  const error = validateForm();
  if (error) {
    alert("⚠️ " + error);
    return;
  }

  const newCode = generateCode();

  setDiscounts((prev) => [
    ...prev,
    {
      code: newCode,
      name: newForm.name,
      from: newForm.from,
      to: newForm.to,
      status: newForm.status,
      method: "Discount on items",
    },
  ]);

  alert("✅ Campaign created successfully!");

  setOpenNew(false);

  // Reset form
  setNewForm({
    name: "",
    code: "",
    from: "",
    to: "",
    status: "Active",
    basedOn: "Product",
    discountType: "Product discount",
    limit: "multiple",
    customerGroupType: "all",
    customerGroup: "",
    notes: "",
  });
};

  // Lọc theo status & validity
  const filteredDiscounts = useMemo(() => {
    const now = new Date();
    return discounts.filter((d) => {
      const matchesSearch =
        d.name.toLowerCase().includes(search.toLowerCase()) ||
        d.code.toLowerCase().includes(search.toLowerCase());
      const matchesStatus = statusFilter === "All" || d.status === statusFilter;
      const isExpired = new Date(d.to) < now;
      const matchesValidity =
        validityFilter === "All" ||
        (validityFilter === "Active" && !isExpired) ||
        (validityFilter === "Expired" && isExpired);
      return matchesSearch && matchesStatus && matchesValidity;
    });
  }, [discounts, search, statusFilter, validityFilter]);

  const handleDelete = () => {
    if (window.confirm(`Are you sure to delete ${selected.name}?`)) {
      setDiscounts((prev) =>
        prev.filter((item) => item.code !== selected.code)
      );
      setOpen(false);
      setSelected(null);
    }
  };

  const handleUpdate = () => {
    setDiscounts((prev) =>
      prev.map((item) =>
        item.code === selected.code ? { ...selected } : item
      )
    );
    setEditing(false);
    alert("✅ Campaign updated successfully!");
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
            Discount
          </h2>
        </div>
        <button
          className="bg-[#FFD500] text-black px-4 py-2 rounded-md font-medium hover:opacity-90 transition"
          onClick={() => setOpenNew(true)}
          >
          New campaign
        </button>

      </div>

      <div className="flex gap-4">
        {/* Sidebar lọc */}
        <div className="w-1/4 bg-gray-50 p-4 rounded-lg space-y-4">

          {/* Status filter */}
          <div>
            <h4 className="font-medium text-gray-700 mb-2">Status</h4>
            <div className="flex flex-col gap-1 text-sm text-gray-600">
              {["All", "Active", "Unactive"].map((s) => (
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

          {/* Validity filter */}
          <div>
            <h4 className="font-medium text-gray-700 mb-2">Validity</h4>
            <div className="flex flex-col gap-1 text-sm text-gray-600">
              {["All", "Active", "Expired"].map((v) => (
                <label key={v} className="flex items-center gap-2">
                  <input
                    type="radio"
                    name="validity"
                    value={v}
                    checked={validityFilter === v}
                    onChange={(e) => setValidityFilter(e.target.value)}
                  />
                  {v}
                </label>
              ))}
            </div>
          </div>
        </div>

        {/* Bảng dữ liệu */}
        <div className="flex-1 bg-gray-50 rounded-lg p-4">
          <div className="flex justify-between items-center mb-4">
            <input
              type="text"
              placeholder="Search by code, campaign name"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-2/3 border border-gray-300 rounded-md p-2 text-sm focus:outline-none focus:ring-1 focus:ring-[#FFD500]"
            />
          </div>

          <table className="w-full text-sm text-gray-700">
            <thead>
              <tr className="bg-[#FFD500] text-gray-800 font-medium">
                <th className="text-left py-2 px-3">Code</th>
                <th className="text-left py-2 px-3">Name</th>
                <th className="text-left py-2 px-3">Period</th>
                <th className="text-left py-2 px-3">Status</th>
              </tr>
            </thead>
            <tbody>
              {filteredDiscounts.length === 0 ? (
                <tr>
                  <td
                    colSpan={4}
                    className="text-center py-6 text-gray-500 italic"
                  >
                    No discounts found
                  </td>
                </tr>
              ) : (
                filteredDiscounts.map((d, idx) => (
                  <tr
                    key={idx}
                    onClick={() => {
                      setSelected(d);
                      setOpen(true);
                      setEditing(false);
                    }}
                    className="border-b border-gray-200 hover:bg-gray-100 cursor-pointer transition"
                  >
                    <td className="py-2 px-3">{d.code}</td>
                    <td className="py-2 px-3">{d.name}</td>
                    <td className="py-2 px-3">
                      {new Date(d.from).toLocaleDateString("vi-VN")} -{" "}
                      {new Date(d.to).toLocaleDateString("vi-VN")}
                    </td>
                    <td
                      className={
                        d.status === "Active"
                          ? "py-2 px-3 text-green-700"
                          : "py-2 px-3 text-gray-500"
                      }
                    >
                      {d.status}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Popup chi tiết */}
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="max-w-3xl rounded-2xl">
          <DialogHeader>
            <DialogTitle className="text-secondary">
              Discount Information
            </DialogTitle>
          </DialogHeader>

          {selected && (
    <div className="mt-4 text-sm">
      {/* Tabs */}
      <div className="border-b border-gray-200 mb-4 flex gap-6 text-gray-700">
        {["Information", "Discount Method", "Discount History - Order"].map(
          (tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`pb-1 transition ${
                activeTab === tab
                  ? "border-b-2 border-[#FFD500] text-[#FFD500] font-medium"
                  : "hover:text-[#FFD500]"
              }`}
            >
              {tab}
            </button>
          )
        )}
      </div>

      {/* Tab: Information */}
      {activeTab === "Information" && (
        <div className="grid grid-cols-2 gap-4 mb-6">
          <div>
            <p>
              <strong>Campaign code:</strong> {selected.code}
            </p>
            <p className="mt-2">
              <strong>Period:</strong>{" "}
              {new Date(selected.from).toLocaleString("vi-VN")} -{" "}
              {new Date(selected.to).toLocaleString("vi-VN")}
            </p>
          </div>

          <div>
            <p>
              <strong>Campaign name:</strong>{" "}
              {editing ? (
                <input
                  type="text"
                  value={selected.name}
                  onChange={(e) =>
                    setSelected({ ...selected, name: e.target.value })
                  }
                  className="border border-gray-300 rounded-md p-1 ml-2"
                />
              ) : (
                selected.name
              )}
            </p>
            <p className="mt-2">
              <strong>Status:</strong>{" "}
              {editing ? (
                <select
                  value={selected.status}
                  onChange={(e) =>
                    setSelected({ ...selected, status: e.target.value })
                  }
                  className="border border-gray-300 rounded-md p-1 ml-2"
                >
                  <option>Active</option>
                  <option>Unactive</option>
                </select>
              ) : (
                <span
                  className={
                    selected.status === "Active"
                      ? "text-green-600"
                      : "text-gray-500"
                  }
                >
                  {selected.status}
                </span>
              )}
            </p>
          </div>
        </div>
      )}

      {/* Tab: Discount Method */}
      {activeTab === "Discount Method" && (
        <div>
          <p className="font-medium text-secondary">
            Product – Discount on items
          </p>
          <p className="text-gray-600 mb-4">
            The discounted quantity is multiplied by the purchased quantity.
          </p>

          <table className="w-full text-sm border-collapse">
            <thead>
              <tr className="bg-[#FFF5CC] font-medium text-gray-700">
                <th className="py-2 px-3 text-left">
                  Product/Category Purchased
                </th>
                <th className="py-2 px-3 text-left">Quantity Purchased</th>
                <th className="py-2 px-3 text-left">Discount</th>
                <th className="py-2 px-3 text-left">
                  Product/Category Discounted
                </th>
                <th className="py-2 px-3 text-left">Quantity Discount</th>
              </tr>
            </thead>
            <tbody>
              <tr className="border-t border-gray-200">
                <td className="py-2 px-3">All categories</td>
                <td className="py-2 px-3">2</td>
                <td className="py-2 px-3">5%</td>
                <td className="py-2 px-3">All categories</td>
                <td className="py-2 px-3">1</td>
              </tr>
            </tbody>
          </table>
        </div>
      )}

      {/* Tab: Discount History - Order */}
      {activeTab === "Discount History - Order" && (
        <div>
          <table className="w-full text-sm border-collapse">
            <thead>
              <tr className="bg-[#FFF5CC] font-medium text-gray-700">
                <th className="py-2 px-3 text-left">Order Code</th>
                <th className="py-2 px-3 text-left">Time</th>
                <th className="py-2 px-3 text-left">Seller</th>
                <th className="py-2 px-3 text-left">Customer</th>
                <th className="py-2 px-3 text-left">Total</th>
                <th className="py-2 px-3 text-left">Discount Value</th>
              </tr>
            </thead>
            <tbody>
              <tr className="bg-[#EAF1FF] border-t border-gray-200">
                <td className="py-2 px-3">DH000061</td>
                <td className="py-2 px-3">05/08/2021 18:46</td>
                <td className="py-2 px-3">Admin</td>
                <td className="py-2 px-3">An Nhien</td>
                <td className="py-2 px-3">290,000</td>
                <td className="py-2 px-3">5,500</td>
              </tr>
              <tr className="border-t border-gray-200">
                <td className="py-2 px-3">DH000062</td>
                <td className="py-2 px-3">06/08/2021 11:22</td>
                <td className="py-2 px-3">Admin</td>
                <td className="py-2 px-3">Anh Minh</td>
                <td className="py-2 px-3">450,000</td>
                <td className="py-2 px-3">8,000</td>
              </tr>
            </tbody>
          </table>
        </div>
      )}

      {/* Buttons */}
      {activeTab === "Information" && (
        <div className="flex justify-end gap-3 mt-6">
          {editing ? (
            <>
              <button
                onClick={handleUpdate}
                className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md"
              >
                Save
              </button>
              <button
                onClick={() => setEditing(false)}
                className="bg-gray-400 hover:bg-gray-500 text-white px-4 py-2 rounded-md"
              >
                Cancel
              </button>
            </>
          ) : (
            <>
              <button
                onClick={() => setEditing(true)}
                className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md"
              >
                Update
              </button>
              <button
                onClick={handleDelete}
                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md"
              >
                Delete
              </button>
            </>
          )}
        </div>
      )}
    </div>
  )}
        </DialogContent>
      </Dialog>

      {/* POPUP: NEW CAMPAIGN */}
<Dialog open={openNew} onOpenChange={setOpenNew}>
  <DialogContent
  className="
    max-w-4xl 
    max-h-[90vh] 
    overflow-y-auto 
    rounded-2xl 
    p-6 
    bg-white
  "
>
    <DialogHeader>
      <DialogTitle className="text-secondary text-xl font-semibold">
        New campaign
      </DialogTitle>
    </DialogHeader>

    {/* FORM START */}
    <div className="space-y-6 text-sm text-gray-700">

      {/* Campaign name + code */}
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="font-medium">Campaign name</label>
          <input
  type="text"
  placeholder="Campaign name"
  value={newForm.name}
  onChange={(e) =>
    setNewForm({ ...newForm, name: e.target.value })
  }
  className="w-full border border-gray-300 rounded-md p-2 mt-1"
/>

        </div>

        <div>
          <label className="font-medium">Campaign code</label>
          <input
            type="text"
            placeholder="Auto code"
            disabled
            className="w-full border border-gray-300 bg-gray-100 rounded-md p-2 mt-1"
          />
        </div>
      </div>

      {/* Validity */}
      <div className="border rounded-lg p-4">
        <p className="font-medium mb-3 text-secondary">Validity</p>

        <div className="grid grid-cols-2 gap-4">
  <div>
    <label className="block font-medium mb-1">Start date</label>
    <input
      type="datetime-local"
      value={newForm.from}
      onChange={(e) => setNewForm({ ...newForm, from: e.target.value })}
      className="w-full border border-gray-300 rounded-md p-2"
    />
  </div>

  <div>
    <label className="block font-medium mb-1">End date</label>
    <input
      type="datetime-local"
      value={newForm.to}
      onChange={(e) => setNewForm({ ...newForm, to: e.target.value })}
      className="w-full border border-gray-300 rounded-md p-2"
    />
  </div>
</div>


        <div className="flex items-center gap-6 mt-4">
          <label className="flex items-center gap-2">
            <input type="radio" name="status" defaultChecked /> Active
          </label>
          <label className="flex items-center gap-2">
            <input type="radio" name="status" /> Unactive
          </label>
        </div>
      </div>

      {/* Discount method */}
      <div className="border rounded-lg p-4">
          <p className="font-medium text-secondary mb-3">
          Discount Method
          </p>
          <div className="grid grid-cols-4 gap-3 items-end">
          <div>
            <label className="font-medium text-xs">Purchase from</label>
            <input className="border p-2 rounded-md w-full" />
          </div>

          <div>
            <label className="font-medium text-xs">Currency</label>
            <input disabled value="VND" className="border p-2 bg-gray-100 rounded-md w-full" />
          </div>

          <div>
            <label className="font-medium text-xs">Discount</label>
            <input className="border p-2 rounded-md w-full" />
          </div>

          <div>
            <label className="font-medium text-xs opacity-0">.</label>
            <button className="p-2 border rounded-md w-full">%</button>
          </div>
          </div>
      </div>

      {/* Number of times */}
      <div className="border rounded-lg p-4">
        <p className="font-medium text-secondary mb-3">
          Number of time discount can be applied per customer
        </p>

        <label className="flex items-center gap-2 mb-1">
          <input type="radio" name="limit" /> Allow once
        </label>

        <label className="flex items-center gap-2">
          <input type="radio" name="limit" defaultChecked /> Allow multiple times
        </label>
      </div>

      {/* Customer group */}
      <div className="border rounded-lg p-4">
  <p className="font-medium text-secondary mb-3">Customer Group</p>

  <label className="flex items-center gap-2 mb-1">
    <input
      type="radio"
      name="cg"
      checked={newForm.customerGroupType === "all"}
      onChange={() => setNewForm({ ...newForm, customerGroupType: "all" })}
    /> 
    All
  </label>

  <label className="flex items-center gap-2 mb-2">
    <input
      type="radio"
      name="cg"
      checked={newForm.customerGroupType === "specific"}
      onChange={() =>
        setNewForm({ ...newForm, customerGroupType: "specific" })
      }
    />
    Specific customer group
  </label>

  {newForm.customerGroupType === "specific" && (
    <select
      className="border p-2 rounded-md w-full"
      value={newForm.customerGroup}
      onChange={(e) =>
        setNewForm({ ...newForm, customerGroup: e.target.value })
      }
    >
      <option value="">Select customer group</option>
      <option value="VIP">VIP</option>
      <option value="Member">Member</option>
      <option value="New">New</option>
    </select>
  )}
</div>


      {/* Notes */}
      <div>
        <label className="font-medium">Notes</label>
        <textarea
          placeholder="Enter note"
          className="w-full border rounded-md p-2 mt-1"
          rows={3}
        />
      </div>

      {/* Buttons */}
      <div className="flex justify-end gap-3 pt-4">
        <button
          className="border border-gray-300 px-4 py-2 rounded-md"
          onClick={() => setOpenNew(false)}
        >
          Cancel
        </button>
        <button
  className="bg-[#FFD500] px-6 py-2 rounded-md font-medium"
  onClick={handleSaveNew}
>
  Save
</button>

      </div>
    </div>
  </DialogContent>
</Dialog>

    </div>
  );
}
