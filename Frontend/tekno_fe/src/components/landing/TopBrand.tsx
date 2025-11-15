"use client";
import { getProductsList } from "@/services/products";
import { Product } from "@/type/product";
import { ChevronRight } from "lucide-react";
import React, { useEffect, useState } from "react";
import ProductCard from "../product/ProductCard";
import { Brand } from "@/type/brand";
import { getBrandList } from "@/services/brand";
import Image from "next/image";

export default function TopBrand() {
  const [brands, setBrands] = useState<Brand[]>([]);
  useEffect(() => {
    const fetchBrands = async () => {
      try {
        const res = await getBrandList();
        console.log(res);
        setBrands(res.data);
      } catch (error) {
        console.log("error in fetching brand", error);
      }
    };
    fetchBrands();
  }, []);
  return (
    <div className="flex flex-col gap-5">
      <div className="border-b border-gray-500 flex items-center justify-between pb-2">
        <div className="font-semibold text-2xl">Top Brands</div>
        <button className="flex items-center gap-2 hoverEffect mx-10">
          View all <ChevronRight className="w-5 h-5" />
        </button>
      </div>
      <div className="flex items-center gap-10 overflow-x-scroll">
        {brands &&
          brands.map((brand) => (
            <div key={brand.id}>
              <img alt="brand image" src={brand.logoPath} className="w-fix" />
            </div>
          ))}
      </div>
    </div>
  );
}
