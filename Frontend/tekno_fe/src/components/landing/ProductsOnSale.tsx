import { ArrowBigRight, ArrowRightCircle } from "lucide-react";
import React, { useEffect, useState } from "react";
import ProductCard from "../product/ProductCard";
import { getProductsOnSale } from "@/services/products";
import { Product } from "@/type/product";
import { count } from "console";
import ViewAllButton from "../share/ViewAllButton";

export default function ProductsOnSale() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        const res = await getProductsOnSale({ count: 4 });
        if (mounted) setProducts(res.data ?? []);
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
    <div className="bg-primary rounded-xl flex py-10 ">
      <div className="flex flex-col items-center justify-center w-1/5">
        <div className="flex flex-col items-center justify-center">
          <div className="text-secondary font-bold px-5">Products On Sale</div>
          <button>Shop now</button>
        </div>
        <ViewAllButton />
        {/* <button className="flex gap-2 hoverEffect group">
          View all{" "}
          <ArrowRightCircle className="w-5 h-5 hidden group-hover:inline-flex hoverEffect " />
        </button> */}
      </div>

      {/* hien thi 4 sp moi nhat */}
      <div className="grid grid-col-2 md:grid-cols-4 gap-4">
        {products?.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </div>
  );
}
