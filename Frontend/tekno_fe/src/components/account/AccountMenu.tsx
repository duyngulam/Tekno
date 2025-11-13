"use client";
import { useAuth } from "@/hook/useAuth";
import { AirVentIcon, icons, LogOut } from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import React from "react";
export const AccountItemsData = [
  { href: "/account/personal-data", label: "Personal Data" },
  { href: "/account/payment", label: " Payment & Instalments " },
  { href: "/account/orders", label: "Orders" },
  { href: "/account/wish-list", label: "Wish list" },
  { href: "/account/discount", label: "Discounts" },
  { href: "/account/notification", label: "Notification" },
  { href: "/contact-us", label: "Contact us" },
];

export default function AccountMenu() {
  const { user, logout } = useAuth();
  const pathname = usePathname();

  return (
    <div className="flex flex-col gap-4 bg-gray-50 rounded-sm">
      {/* avt + name */}
      <div className="flex items-center justify-center gap-4 border border-amber-400">
        <div>avt</div>
        <div>{user?.email}</div>
      </div>
      {/* tab */}
      {AccountItemsData.map((item) => (
        <Link
          key={item?.href}
          href={item?.href}
          className={`hover:text-primary hoverEffect relative group ${
            pathname === item.href && "text-primary"
          }`}
        >
          <div className="flex items-center justify-start gap-4 pl-10 my-3 px-4">
            <AirVentIcon className="text-black" />
            {item.label}
          </div>

          <span
            className={`absolute top-0 left-0 h-0 bg-primary group-hover:h-full group-hover:w-1 group-hover:left-0 hoverEffect ${
              pathname === item.href && "h-full w-1"
            }`}
          />
        </Link>
      ))}
      <div className="text-red-500 hoverEffect relative group" onClick={logout}>
        <div className="flex items-center justify-start gap-4 pl-10 my-3 px-4">
          <LogOut className="text-black" />
          Log out
        </div>
        <span className="absolute top-0 left-0 h-0 bg-primary group-hover:h-full group-hover:w-1 group-hover:left-0 hoverEffect hover:h-full hover:w-1" />
      </div>
    </div>
  );
}
