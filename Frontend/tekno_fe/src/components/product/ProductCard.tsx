import Link from "next/link";
import Image from "next/image";
import { Product } from "@/type/product";
import { HeartIcon, ShoppingCartIcon } from "@heroicons/react/24/outline";
import AddToFavorButton from "./AddToFavorButton";
import { Star } from "lucide-react";

interface ProductCardProps {
  product: Product;
}

export default function ProductCard({ product }: ProductCardProps) {
  return (
    <Link href={`/products/${product?.slug}`}>
      <div className=" relative text-sm bg-white border-[1px] border-secondary/20 rounded-md group shadow">
        {/* --- Ảnh sản phẩm --- */}
        <div className="relative group overflow-hidden bg-gray-50 m-1 pb-1 border-b border-secondary/50 hover:border-secondary hoverEffect">
          {product?.primaryImagePath && (
            <Image
              src={product.primaryImagePath}
              alt={product.name}
              loading="lazy"
              width={400}
              height={400}
              className="object-center"
            />
          )}
        </div>
        <AddToFavorButton
          className="absolute top-2 right-2 z-10"
          product={product}
        />
        {/* {product?.discountPercent && product.discountPercent > 0 && (
            <p className="absolute z-10 top-2 left-0 bg-blue-100 text-blue-600 text-sm font-semibold px-2 py-1 rounded-r-lg border border-blue-500/50 group-hover:border-blue-700 hoverEffect">
              {product?.discountPercent}
            </p>
          )} */}
        <p className="absolute z-10 top-2 left-0 bg-blue-100 text-blue-600 text-sm font-semibold px-2 py-1 rounded-r-lg ">
          15
        </p>

        {/* --- Tên sản phẩm --- */}
        <div className="p-3 flex flex-col gap-2">
          <p className="text-gray-900 text-sm font-medium line-clamp-1">
            {product.name}
          </p>
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-500 line-through text-sm font-normal">
                {product.basePrice}
              </p>
              <p className="text-gray-900 font-medium text-xl">
                {product.finalPrice}
              </p>
            </div>
            {/* sao */}
            <div className="flex gap-1 items-center text-primary">
              <Star fill="bg-primary" />
              <span className="ml-1 font-normal text-base">4.9</span>
            </div>
          </div>
        </div>
      </div>
    </Link>
  );
}
