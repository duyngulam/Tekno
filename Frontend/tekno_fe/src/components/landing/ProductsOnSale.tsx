import { ArrowBigRight, ArrowRightCircle } from "lucide-react";
import React from "react";
import ProductCard from "../product/ProductCard";

export default function ProductsOnSale() {
  return (
    <div className="bg-primary rounded-xl flex py-10 ">
      <div className="flex flex-col items-center justify-center w-1/5">
        <div className="flex flex-col items-center justify-center">
          <div className="text-secondary font-bold px-5">Products On Sale</div>
          <button>Shop now</button>
        </div>
        <button className="flex gap-2 hoverEffect group">
          View all{" "}
          <ArrowRightCircle className="w-5 h-5 hidden group-hover:inline-flex hoverEffect " />
        </button>
      </div>

      {/* hien thi 4 sp moi nhat */}
      <div>Dnah sach sp</div>
    </div>
  );
}
