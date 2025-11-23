"use client";
import { ShoppingBag } from "lucide-react";
import Link from "next/link";
import React from "react";
import { useStore } from "../../../../store";

export default function () {
  const { items } = useStore();
  return (
    <div>
      <Link href={"/cart"} className="group relative">
        <ShoppingBag className="w-5 h-5 hover:text-primary hoverEffect" />
        <span className="absolute -top-1 -right-1 bg-primary text-white rounded-full w-3.5 h-3.5 text-xs font-semibold flex items-center justify-center ">
          {items?.length ? items?.length : 0}
        </span>
      </Link>
    </div>
  );
}
