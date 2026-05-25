import { ArrowBigRight, ArrowRightCircle } from "lucide-react";
import React, { useEffect, useState } from "react";
import ProductCard from "../product/ProductCard";
import {
  getProductRecommendation,
  getProductsOnSale,
} from "@/services/products";
import { Product } from "@/type/product";
import { count } from "console";
import ViewAllButton from "../share/ViewAllButton";
import { useAuth } from "@/hook/useAuth";

export default function ProductsOnSale() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();

  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        const res = await getProductRecommendation(user?.id ?? 2, 10);
        if (mounted) setProducts(res ?? []);
      } catch (e) {
        console.error("error in fetching new products", e);
      } finally {
        if (mounted) setLoading(false);
      }
    })();
    return () => {
      mounted = false;
    };
  }, []);
  return (
    <div className="relative rounded-2xl overflow-hidden p-8 bg-gradient-to-r from-pink-200 via-blue-200 to-purple-200">
      {/* Gradient overlay */}
      <div className="absolute inset-0 bg-gradient-to-br from-pink-300/30 via-blue-300/30 to-purple-300/30"></div>

      {/* Content */}
      <div className="relative z-10">
        {/* Header Section */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center gap-2 bg-white/20 backdrop-blur-sm rounded-full px-6 py-2 mb-4">
            <span className="text-2xl">✨</span>
            <h2 className="text-lg font-bold text-gray-800">
              DÀNH CHO BẠN
            </h2>
          </div>
        </div>

        {/* Products Horizontal Scroll */}
<div className="relative">
  <div
    className="
      flex
      gap-4
      overflow-x-auto
      overflow-y-hidden
      pb-4
      scroll-smooth
      snap-x
      snap-mandatory
      touch-pan-x
      [-ms-overflow-style:none]
      [scrollbar-width:none]
      [&::-webkit-scrollbar]:hidden
    "
  >
    {products?.map((product) => (
      <div
        key={product.id}
        className="
          min-w-[280px]
          sm:min-w-[300px]
          md:min-w-[320px]
          flex-none
          snap-start
        "
      >
        <ProductCard product={product} />
      </div>
    ))}
  </div>

  {/* Scroll button */}
  {products.length > 0 && (
    <button
      className="
        absolute
        right-2
        top-1/2
        -translate-y-1/2
        bg-white/80
        hover:bg-white
        rounded-full
        p-2
        shadow-lg
        z-20
      "
    >
      <ArrowRightCircle className="w-5 h-5 text-gray-600" />
    </button>
  )}
</div>

        </div>
      </div>

  );
}
