"use client";
import { cn } from "@/lib/utils";
import { Product } from "@/type/product";
import { Heart } from "lucide-react";
import React, { useEffect, useState } from "react";
import { useStore } from "../../../store";

export default function AddToFavorButton({
  product,
  className,
}: {
  product: Product;
  className?: string;
}) {
  const { favorProducts, addToFavor, removeFavor } = useStore();
  const [existingProduct, setExistingProduct] = useState<Product | null>(null);
  useEffect(() => {
    const availableItem = favorProducts.find((item) => item.id === product.id);
    setExistingProduct(availableItem || null);
  }, [product, favorProducts]);
  const handleFavor = (e: React.MouseEvent<HTMLSpanElement>) => {
    e.preventDefault();
    if (product?.id) {
      addToFavor(product);
    }
  };
  return (
    <div className={cn("", className)}>
      <button
        className="p-2.5 rounded-full hover:bg-primary hover:text-white hoverEffect text-primary"
        onClick={handleFavor}
      >
        {existingProduct ? (
          <Heart
            fill="red"
            size={20}
            className="text-primary/80 group-hover:text-white hoverEffect"
          />
        ) : (
          <Heart
            size={20}
            className="text-primary/80 group-hover:text-white hoverEffect"
          />
        )}
      </button>
    </div>
  );
}
