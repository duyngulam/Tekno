import { ShoppingBag } from "lucide-react";
import Link from "next/link";
import React from "react";

export default function () {
  return (
    <div>
      <Link href={"/cart"} className="group relative">
        <ShoppingBag className="w-5 h-5 hover:text-primary hoverEffect" />
        <span className="absolute -top-1 -right-1 bg-primary text-white rounded-full w-3.5 h-3.5 text-xs font-semibold flex items-center justify-center ">
          0
        </span>
      </Link>
    </div>
  );
}
