import React from "react";

import Filter from "@/components/product/Filter";
import Categories from "@/components/product/Categories";
import ProductCard from "@/components/product/ProductCard";
import type { Product } from "@/type/product";

export default function Products() {
  const res = {
    success: true,
    data: [
      {
        id: 1,
        name: "Dell XPS 13",
        slug: "dell-xps-13",
        basePrice: 1699.0,
        overview: "Premium ultrabook with compact design.",
        brandName: "Dell",
        primaryImageUrl:
          "https://cdn2.fptshop.com.vn/unsafe/meme_loopy_2_114d7eb714.jpg",
      },
      {
        id: 2,
        name: "MacBook Air",
        slug: "macbook-air",
        basePrice: 1199.0,
        overview: "Ultra-thin and lightweight laptop by Apple.",
        brandName: "Apple",
        primaryImageUrl:
          "https://cdn2.fptshop.com.vn/unsafe/meme_loopy_2_114d7eb714.jpg",
      },
      {
        id: 3,
        name: "Asus ZenBook",
        slug: "asus-zenbook",
        basePrice: 1450.0,
        overview: "Portable productivity ultrabook.",
        brandName: "Asus",
        primaryImageUrl:
          "https://cdn2.fptshop.com.vn/unsafe/meme_loopy_2_114d7eb714.jpg",
      },
    ],
    pagination: {
      page: 1,
      pageSize: 3,
      totalItems: 5,
      totalPages: 2,
    },
  };

  const products: Product[] = res.data;

  return (
    <div>
      <div className="breadcrumbs text-sm ">
        <ul>
          <li>
            <a>Home</a>
          </li>
          <li>
            <a className="text-primary border-d-2 border-primary">Products</a>
          </li>
        </ul>
      </div>

      <div className="flex flex-wrap gap-4">
        <Categories />
        <Categories />
        <Categories />
        <Categories />
      </div>

      {/* filter chip */}
      <div></div>

      <div className="flex gap-4 w-7xl bg-amber-200 p-4">
        <div>
          <Filter />
        </div>

        <div className="flex flex-wrap gap-4">
          {/* ✅ Lặp qua mảng products */}
          {products.map((p) => (
            <ProductCard key={p.id} product={p} />
          ))}
        </div>
      </div>
    </div>
  );
}
