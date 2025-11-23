"use client";
import React from "react";
import { Button } from "../ui/button";
import { Product } from "@/type/product";
import { useStore } from "../../../store";

export default function AddToCartButton({
  product,
  className,
}: {
  product: Product;
  className?: string;
}) {
  const { addItem } = useStore();
  const isOutOfStock = false;
  const handleAddToCart = () => {
    console.log(product);
    if (!isOutOfStock) {
      addItem(product);
      alert(`${product.name} added succesfull`);
    }
  };
  return (
    <div className="w-full h-12 flex items-center">
      <Button
        variant="outline"
        onClick={handleAddToCart}
        disabled={isOutOfStock}
        className="w-full"
      >
        Add to cart
      </Button>
    </div>
  );
}
