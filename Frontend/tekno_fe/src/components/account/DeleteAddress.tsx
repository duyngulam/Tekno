"use client";

import React, { useState } from "react";
import { toast } from "sonner";
import { ProfileAddress } from "@/type/address";
import { deleteProfileAddress } from "@/services/profile";
import { Trash2 } from "lucide-react";

type Props = {
  address: ProfileAddress;
  onDeleted?: () => void;
  onClose?: () => void;
};

export default function DeleteAddress({ address, onDeleted, onClose }: Props) {
  const [deleting, setDeleting] = useState(false);

  const handleDelete = async () => {
    try {
      setDeleting(true);
      const token = localStorage.getItem("token") || "";
      if (!token) throw new Error("Missing token");

      await deleteProfileAddress(token, Number(address.id));

      toast.success("Đã xóa địa chỉ thành công");
      onDeleted?.();
      onClose?.();
    } catch (e: any) {
      toast.error(e?.message || "Xóa địa chỉ thất bại");
    } finally {
      setDeleting(false);
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-center">
        <div className="w-16 h-16 rounded-full bg-red-100 flex items-center justify-center">
          <Trash2 className="w-8 h-8 text-red-500" />
        </div>
      </div>

      <div className="text-center space-y-2">
        <h3 className="text-lg font-semibold text-gray-800">Delete Address</h3>
        <p className="text-sm text-gray-600">
          Are you sure you want to delete this address? This action cannot be
          undone.
        </p>
      </div>

      <div className="bg-gray-50 p-3 rounded-md">
        <div className="text-sm">
          <div className="font-medium">{address.recipientName}</div>
          <div className="text-gray-600">{address.phoneNumber}</div>
          <div className="text-gray-600">{address.addressLine}</div>
          <div className="text-gray-500">
            {address.wardName}, {address.districtName}, {address.provinceName}
          </div>
        </div>
      </div>

      <div className="flex gap-3 pt-2">
        <button
          type="button"
          onClick={onClose}
          className="flex-1 px-4 py-2 rounded-md border border-gray-300 text-gray-700 hover:bg-gray-50"
          disabled={deleting}
        >
          Cancel
        </button>
        <button
          type="button"
          onClick={handleDelete}
          disabled={deleting}
          className="flex-1 px-4 py-2 rounded-md bg-red-500 text-white hover:bg-red-600 disabled:opacity-50"
        >
          {deleting ? "Deleting..." : "Delete Address"}
        </button>
      </div>
    </div>
  );
}
