"use client";
import { cn } from "@/lib/utils";
import { Product } from "@/type/product";
import { Heart } from "lucide-react";
import React, { useEffect, useState } from "react";
import useFavor from "@/hook/useFavor";

export default function AddToFavorButton({
  product,
  className,
}: {
  product: Product;
  className?: string;
}) {
  const { items, addToFavor, removeFavor } = useFavor();
  const [existingProduct, setExistingProduct] = useState<Product | null>(null);

  useEffect(() => {
    const availableItem = items.find((item) => item.id === product.id);
    setExistingProduct(availableItem || null);
  }, [product, items]);

  const handleFavor = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.preventDefault();
    if (!product?.id) return;

    if (existingProduct) {
      removeFavor(product.id); // ❌ có rồi thì remove
    } else {
      addToFavor(product.id); // ✔️ chưa có thì add
    }
  };

  return (
    <div className={cn("", className)}>
      <button
        className="p-2.5 rounded-full hover:bg-primary hover:text-white hoverEffect text-primary"
        onClick={handleFavor}
      >
        {existingProduct ? (
          <Heart fill="red" size={20} className="hoverEffect" />
        ) : (
          <Heart size={20} className="text-primary/80 hoverEffect" />
        )}
      </button>
    </div>
  );
}
