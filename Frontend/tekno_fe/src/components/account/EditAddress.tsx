"use client";

import React, { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";
import { ProfileAddress } from "@/type/address";
import { Province, District, Ward } from "@/type/location";
import { getProvinces, getDistricts, getWards } from "@/services/location";
import {
  AddressPayload,
  updateProfileAddress,
  setDefaultProfileAddress,
} from "@/services/profile";

type Props = {
  address: ProfileAddress;
  onUpdated?: () => void; // callback khi cập nhật xong
  onClose?: () => void; // đóng modal nếu có
};

export default function EditAddress({ address, onUpdated, onClose }: Props) {
  // form state
  const [recipientName, setRecipientName] = useState(
    address.recipientName ?? ""
  );
  const [phoneNumber, setPhoneNumber] = useState(address.phoneNumber ?? "");
  const [addressLine, setAddressLine] = useState(
    // hỗ trợ cả addressLine và addressLine1
    (address as any).addressLine ?? (address as any).addressLine1 ?? ""
  );
  const [isDefault, setIsDefault] = useState<boolean>(!!address.isDefault);

  // location selections (use Code thay vì id)
  const [provinceCode, setProvinceCode] = useState<number | undefined>(
    (address as any).provinceCode ?? (address as any).provinceId
  );
  const [districtCode, setDistrictCode] = useState<number | undefined>(
    (address as any).districtCode ?? (address as any).districtId
  );
  const [wardCode, setWardCode] = useState<number | undefined>(
    (address as any).wardCode ?? (address as any).wardId
  );

  // options
  const [provinces, setProvinces] = useState<Province[]>([]);
  const [districts, setDistricts] = useState<District[]>([]);
  const [wards, setWards] = useState<Ward[]>([]);

  // ui state
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  // load provinces
  useEffect(() => {
    (async () => {
      try {
        const ps = await getProvinces();
        setProvinces(ps);
      } catch (e: any) {
        toast.error(e?.message || "Không tải được danh sách tỉnh/thành");
      }
    })();
  }, []);

  // load districts when province changes (or initial)
  useEffect(() => {
    if (!provinceCode) {
      setDistricts([]);
      setDistrictCode(undefined);
      setWards([]);
      setWardCode(undefined);
      return;
    }
    (async () => {
      try {
        setLoading(true);
        const ds = await getDistricts(provinceCode);
        setDistricts(ds);
        // nếu district hiện tại không thuộc tỉnh mới -> reset
        if (districtCode && !ds.some((d) => d.code === districtCode)) {
          setDistrictCode(undefined);
          setWards([]);
          setWardCode(undefined);
        }
      } catch (e: any) {
        toast.error(e?.message || "Không tải được danh sách quận/huyện");
      } finally {
        setLoading(false);
      }
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [provinceCode]);

  // load wards when district changes (or initial)
  useEffect(() => {
    if (!districtCode) {
      setWards([]);
      setWardCode(undefined);
      return;
    }
    (async () => {
      try {
        setLoading(true);
        const ws = await getWards(districtCode);
        setWards(ws);
        if (wardCode && !ws.some((w) => w.code === wardCode)) {
          setWardCode(undefined);
        }
      } catch (e: any) {
        toast.error(e?.message || "Không tải được danh sách phường/xã");
      } finally {
        setLoading(false);
      }
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [districtCode]);

  const canSubmit = useMemo(
    () =>
      recipientName.trim() &&
      phoneNumber.trim() &&
      addressLine.trim() &&
      provinceCode &&
      districtCode &&
      wardCode,
    [
      recipientName,
      phoneNumber,
      addressLine,
      provinceCode,
      districtCode,
      wardCode,
    ]
  );

  // PATCH set default (đặt ở đầu tiên)
  const handleSetDefault = async () => {
    try {
      const token = localStorage.getItem("token") || "";
      if (!token) throw new Error("Thiếu token đăng nhập");
      await setDefaultProfileAddress(token, Number(address.id));
      setIsDefault(true);
      toast.success("Đã đặt địa chỉ làm mặc định");
      onUpdated?.();
    } catch (e: any) {
      toast.error(e?.message || "Không thể đặt mặc định");
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canSubmit || submitting) return;

    try {
      setSubmitting(true);
      const token = localStorage.getItem("token") || "";
      if (!token) throw new Error("Thiếu token đăng nhập");

      const provinceName =
        provinces.find((p) => p.code === provinceCode)?.name ??
        (address as any).provinceName ??
        "";
      const districtName =
        districts.find((d) => d.code === districtCode)?.name ??
        (address as any).districtName ??
        "";
      const wardName =
        wards.find((w) => w.code === wardCode)?.name ??
        (address as any).wardName ??
        "";

      const payload: AddressPayload = {
        recipientName: recipientName.trim(),
        phoneNumber: phoneNumber.trim(),
        addressLine: addressLine.trim(),
        provinceCode: provinceCode!,
        provinceName,
        districtCode: districtCode!,
        districtName,
        wardCode: wardCode!,
        wardName,
        isDefault,
      };

      await updateProfileAddress(
        localStorage.getItem("token") || "",
        Number(address.id),
        payload
      );

      toast.success("Cập nhật địa chỉ thành công");
      onUpdated?.();
      onClose?.();
    } catch (e: any) {
      toast.error(e?.message || "Cập nhật địa chỉ thất bại");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {/* Set default đặt ở đầu tiên */}
      <div className="flex items-center justify-between rounded-md border px-3 py-2 bg-white">
        <div className="text-sm">
          <div className="font-medium">Đặt làm mặc định</div>
          <div className="text-gray-500">
            Địa chỉ mặc định sẽ dùng cho đặt hàng và thanh toán.
          </div>
        </div>
        <button
          type="button"
          onClick={handleSetDefault}
          disabled={!!isDefault}
          className={`px-3 py-2 rounded-md ${
            isDefault
              ? "bg-gray-200 text-gray-500"
              : "bg-yellow-400 text-white hover:bg-yellow-500"
          }`}
        >
          {isDefault ? "Mặc định" : "Set default"}
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="flex flex-col gap-2">
          <label className="text-sm text-gray-600">Họ tên</label>
          <input
            className="border rounded-md px-3 py-2"
            value={recipientName}
            onChange={(e) => setRecipientName(e.target.value)}
            placeholder="Họ và tên"
          />
        </div>

        <div className="flex flex-col gap-2">
          <label className="text-sm text-gray-600">Số điện thoại</label>
          <input
            className="border rounded-md px-3 py-2"
            value={phoneNumber}
            onChange={(e) => setPhoneNumber(e.target.value)}
            placeholder="VD: 0912345678"
          />
        </div>

        <div className="md:col-span-2 flex flex-col gap-2">
          <label className="text-sm text-gray-600">Địa chỉ</label>
          <input
            className="border rounded-md px-3 py-2"
            value={addressLine}
            onChange={(e) => setAddressLine(e.target.value)}
            placeholder="Số nhà, đường, tòa nhà..."
          />
        </div>

        <div className="flex flex-col gap-2">
          <label className="text-sm text-gray-600">Tỉnh/Thành phố</label>
          <select
            className="border rounded-md px-3 py-2"
            value={provinceCode ?? ""}
            onChange={(e) =>
              setProvinceCode(
                e.target.value ? Number(e.target.value) : undefined
              )
            }
          >
            <option value="">Chọn tỉnh/thành</option>
            {provinces.map((p) => (
              <option key={p.code as any} value={p.code as any}>
                {p.name}
              </option>
            ))}
          </select>
        </div>

        <div className="flex flex-col gap-2">
          <label className="text-sm text-gray-600">Quận/Huyện</label>
          <select
            className="border rounded-md px-3 py-2"
            value={districtCode ?? ""}
            onChange={(e) =>
              setDistrictCode(
                e.target.value ? Number(e.target.value) : undefined
              )
            }
            disabled={!provinceCode || loading}
          >
            <option value="">Chọn quận/huyện</option>
            {districts.map((d) => (
              <option key={d.code as any} value={d.code as any}>
                {d.name}
              </option>
            ))}
          </select>
        </div>

        <div className="flex flex-col gap-2">
          <label className="text-sm text-gray-600">Phường/Xã</label>
          <select
            className="border rounded-md px-3 py-2"
            value={wardCode ?? ""}
            onChange={(e) =>
              setWardCode(e.target.value ? Number(e.target.value) : undefined)
            }
            disabled={!districtCode || loading}
          >
            <option value="">Chọn phường/xã</option>
            {wards.map((w) => (
              <option key={w.code as any} value={w.code as any}>
                {w.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="flex gap-3">
        <button
          type="submit"
          disabled={!canSubmit || submitting}
          className="px-4 py-2 rounded-md bg-primary text-white disabled:bg-gray-300"
        >
          {submitting ? "Đang lưu..." : "Lưu thay đổi"}
        </button>
        <button
          type="button"
          onClick={onClose}
          className="px-4 py-2 rounded-md border"
        >
          Hủy
        </button>
      </div>
    </form>
  );
}
