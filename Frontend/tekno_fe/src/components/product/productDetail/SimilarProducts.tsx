import { getProductsList } from "@/services/products";
import React from "react";
import ProductsListWithCarousel from "./ProductsListWithCarousel";

export default async function SimilarProducts() {
  const data = await getProductsList({ category: "laptop" });

  const products = data.data;
  console.log(products);
  return (
    <div className="flex flex-col gap-4">
      {/* title */}
      <div className="font-bold text-xl">Similar Products</div>
      <ProductsListWithCarousel products={products} />
    </div>
  );
}
