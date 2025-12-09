"use client";
import { ShoppingBag } from "lucide-react";
import Link from "next/link";
import React from "react";
import { useStore } from "../../../../store";
import { useCart } from "@/hook/useCart";
import { get } from "http";

export default function () {
  const { getTotalItems, items } = useCart();
  const n = items.length;
  console.log("cart item count:", items.length);

  return (
    <div>
      <Link href={"/cart"} className="group relative">
        <ShoppingBag className="w-5 h-5 hover:text-primary hoverEffect" />
        <span className="absolute -top-1 -right-1 bg-primary text-white rounded-full w-3.5 h-3.5 text-xs font-semibold flex items-center justify-center ">
          {n ? n : 0}
        </span>
      </Link>
    </div>
  );
}
