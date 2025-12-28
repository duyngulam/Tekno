"use client";
import React, { useEffect, useState } from "react";
import Image from "next/image";
import { Brand } from "@/type/brand";
import { getBrandList } from "@/services/brand";

export default function TopBrand() {
  const [brands, setBrands] = useState<Brand[]>([]);

  useEffect(() => {
    const fetchBrands = async () => {
      try {
        const res = await getBrandList();
        setBrands(res.data ?? []);
      } catch (error) {
        console.log("error in fetching brand", error);
      }
    };
    fetchBrands();
  }, []);

  return (
    <div className="flex flex-col gap-5">
      <div className="border-b border-gray-200 flex items-center justify-between pb-2">
        <div className="font-semibold text-2xl">Top Brands</div>
      </div>

      {/* scrollable brand list */}
      <div className="flex items-center gap-4 overflow-x-auto scroll-smooth snap-x snap-mandatory py-2">
        {brands.map((brand) => (
          <div
            key={brand.id}
            className="snap-start shrink-0 flex items-center justify-center rounded-xl bg-white px-4 pb-3"
            style={{ minWidth: 120 }}
          >
            <div className="relative w-24 h-12 md:w-32 md:h-14">
              <Image
                alt={brand.name ?? "brand image"}
                src={brand.logoPath}
                fill
                sizes="(max-width: 640px) 96px, (max-width: 1024px) 128px, 160px"
                className="object-contain"
                priority={false}
              />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
