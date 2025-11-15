"use client";

import React, { useState, useEffect } from "react";
import { Switch } from "@/components/ui/switch";
import Link from "next/link";

export default function CatalogPage() {
const [settings, setSettings] = useState<Record<CatalogKey, boolean>>({
  discount: true,
  voucher: true,
  coupon: true,
});

// Load trạng thái từ localStorage lúc mở trang catalog
useEffect(() => {
  const saved = localStorage.getItem("catalogSettings");
  if (saved) {
    setSettings(JSON.parse(saved));
  }
}, []);

const handleToggle = (key: CatalogKey) => {
  setSettings((prev) => {
    const updated = { ...prev, [key]: !prev[key] };
    localStorage.setItem("catalogSettings", JSON.stringify(updated));
    return updated;
  });
};

type CatalogKey = "discount" | "voucher" | "coupon";

const options: { key: CatalogKey; title: string; description: string; link: string } [] = [
  {
    key: "discount",
    title: "Discount",
    description:
      "Allows managing and applying discounts by product or order value",
    link: "/dashboard/catalog/discount",
  },
  {
    key: "voucher",
    title: "Voucher",
    description:
      "Allows managing, issuing, and applying shopping vouchers.",
    link: "/dashboard/catalog/voucher",
  },
  {
    key: "coupon",
    title: "Coupon",
    description:
      "Allows managing, issuing, and applying discount codes.",
    link: "/dashboard/catalog/coupon",
  },
];

  return (
    <div className="bg-white rounded-lg shadow p-6 border border-gray-100">
      <div className="divide-y divide-gray-200">
        {options.map((item) => (
          <div
            key={item.key}
            className="flex items-center justify-between py-4"
          >
            <div>
              <h3 className="text-gray-800 font-medium">{item.title}</h3>
              <p className="text-gray-500 text-sm mt-1">{item.description}</p>
            </div>

            <div className="flex items-center gap-3">
            {settings[item.key] && (
              <Link
                href={item.link}
                className="text-secondary font-medium text-sm underline hover:text-primary transition"
              >
              Manage
              </Link>
            )}

              <Switch
                checked={settings[item.key]}
                onCheckedChange={() => handleToggle(item.key)}
                className="data-[state=checked]:bg-[#FFD500] data-[state=unchecked]:bg-gray-300"
              />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
