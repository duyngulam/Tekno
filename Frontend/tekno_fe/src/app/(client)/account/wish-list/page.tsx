"use client";
import TitleAccount from "@/components/account/TitleAccount";
import ProductCard from "@/components/product/ProductCard";
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationPrevious,
} from "@/components/ui/pagination";
import useFavor from "@/hook/useFavor";
import React from "react";

export default function page() {
  const { items } = useFavor();
  console.log("favor item", items);

  return (
    <div className="flex flex-col gap-4">
      <TitleAccount title="Wish list" des="See your favorites list here" />
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-10">
        {items.map((p) => (
          <ProductCard product={p} />
        ))}
      </div>
    </div>
  );
}
