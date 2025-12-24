"use client";

import { cn } from "@/lib/utils";
import { Product } from "@/type/product";
import { Heart } from "lucide-react";
import React, { useEffect, useState } from "react";
import useFavor from "@/hook/useFavor";

export default function AddToFavorButton({
  productId,
  className,
}: {
  productId: number;
  className?: string;
}) {
  console.log(productId);

  const { items, addToFavor, removeFavor, checkFavor } = useFavor();

  // Kiểm tra sản phẩm có trong danh sách yêu thích hay chưa
  const exists = items.some((item) => item.id === productId);
  // const [exists, setExists] = useState(false);
  // useEffect(() => {
  //   const check = async () => {
  //     if (!productId) return;

  //     const exists = await checkFavor(productId);
  //     setExists(exists);
  //   };

  //   check();
  // }, [productId]);

  const handleFavor = async (e: React.MouseEvent<HTMLButtonElement>) => {
    e.preventDefault();
    if (!productId) return;
    console.log(productId);

    if (exists) {
      await removeFavor(productId); // Đã tồn tại -> remove
    } else {
      await addToFavor(productId); // Chưa tồn tại -> add
    }
  };

  return (
    <div className={cn("", className)}>
      <button
        className="p-2.5 rounded-full hover:bg-primary hover:text-white hoverEffect text-primary"
        onClick={handleFavor}
      >
        {exists ? (
          <Heart fill="red" size={20} className="hoverEffect" />
        ) : (
          <Heart size={20} className="text-primary/80 hoverEffect" />
        )}
      </button>
    </div>
  );
}
