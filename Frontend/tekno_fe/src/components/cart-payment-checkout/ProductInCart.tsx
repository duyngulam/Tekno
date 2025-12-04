import { Product } from "@/type/product";
import { Trash2 } from "lucide-react";
import React from "react";

export default function ProductInCart({ product }: { product?: any }) {
  return (
    <div className="flex gap-1 w-full">
      <div>Image</div>
      <div className="flex flex-col gap-2">
        <p>{product?.name}</p>
        <p>variant</p>
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <p>{product?.basePrice} </p>
            <p>{product?.finalPrice}</p>
          </div>
          <div>
            <Trash2 />
            <p>-1+</p>
          </div>
        </div>
      </div>
    </div>
  );
}
