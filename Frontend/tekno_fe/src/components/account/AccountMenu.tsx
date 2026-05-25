"use client";
import { useAuth } from "@/hook/useAuth";
import {
  User,
  CreditCard,
  ShoppingBag,
  Heart,
  Tag,
  Phone,
  LogOut,
} from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import React from "react";

// Add icon per item
export const AccountItemsData = [
  { href: "/account/personal-data", label: "Personal Data", icon: User },

  { href: "/account/orders", label: "Orders", icon: ShoppingBag },
  { href: "/account/wish-list", label: "Wish list", icon: Heart },
  { href: "/account/discount", label: "Discounts", icon: Tag },
  { href: "/contact-us", label: "Contact us", icon: Phone },
];

export default function AccountMenu() {
  const { user, logout } = useAuth();
  const pathname = usePathname();

  return (
    <div className="flex flex-col gap-4 bg-gray-50 rounded-sm">
      {/* avatar + name */}
      <div className="flex items-center gap-4 p-4 rounded-xl bg-white border">
        {/* Sample avatar (replace with user.avatarUrl if available) */}
        <div className="w-14 h-14 rounded-full bg-gradient-to-br from-blue-300 to-blue-500 text-white flex items-center justify-center">
          <User className="w-7 h-7" />
        </div>
        <div className="flex flex-col">
          <div className="text-sm font-semibold">
            {user?.role || user?.email || "Guest"}
          </div>
          <div className="text-xs text-gray-500">
            {user?.email || "guest@tekno.dev"}
          </div>
        </div>
      </div>

      {/* tabs */}
      {AccountItemsData.map((item) => {
        const Icon = item.icon;
        const active = pathname === item.href;
        return (
          <Link
            key={item.href}
            href={item.href}
            className={`hover:text-primary hoverEffect relative group ${
              active && "text-primary"
            }`}
          >
            <div className="flex items-center justify-start gap-4 pl-10 my-3 px-4">
              <Icon
                className={`h-5 w-5 ${active ? "text-primary" : "text-black"}`}
              />
              {item.label}
            </div>

            <span
              className={`absolute top-0 left-0 h-0 bg-primary group-hover:h-full group-hover:w-1 group-hover:left-0 hoverEffect ${
                active && "h-full w-1"
              }`}
            />
          </Link>
        );
      })}

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
