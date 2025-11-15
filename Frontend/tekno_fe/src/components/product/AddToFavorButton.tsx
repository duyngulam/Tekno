import { cn } from "@/lib/utils";
import { Product } from "@/type/product";
import { Heart } from "lucide-react";
import React from "react";

export default function AddToFavorButton({
  product,
  className,
}: {
  product: Product;
  className?: string;
}) {
  return (
    <div className={cn("absolute top-2 right-2 z-10", className)}>
      <button className="p-2.5 rounded-full hover:bg-primary hover:text-white hoverEffect text-primary">
        <Heart size={20} />
      </button>
    </div>
  );
}
