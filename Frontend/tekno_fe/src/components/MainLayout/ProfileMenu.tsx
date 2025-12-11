"use client";

import { useAuth } from "@/hook/useAuth";
import { Heart, LogOut, ShoppingBag, UserRound } from "lucide-react";
import Link from "next/link";
import React from "react";

export default function ProfileMenu() {
  const { logout, user } = useAuth();

  return (
    <div className="bg-white shadow-lg rounded-lg p-4 w-fit space-y-2">
      <Link
        href="/account/personal-data"
        className="flex items-center gap-2 py-1 hover:text-blue-500 font-normal text-lg"
      >
        <UserRound size={24} /> {user?.email}
      </Link>
      <Link
        href="/cart"
        className="flex items-center gap-2 py-1 hover:text-blue-500 font-normal text-lg"
      >
        <ShoppingBag size={24} /> Orders
      </Link>

      <Link
        href="/account/wishlist"
        className="flex items-center gap-2 py-1 hover:text-blue-500 font-normal text-lg"
      >
        <Heart size={24} /> Wish list
      </Link>

      <button
        onClick={logout}
        className="flex items-center gap-2 py-1 font-normal text-lg hover:text-red-500"
      >
        <LogOut />
        Logout
      </button>
    </div>
  );
}
