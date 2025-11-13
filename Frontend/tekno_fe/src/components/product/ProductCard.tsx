import Link from "next/link";
import Image from "next/image";
import { Product } from "@/type/product";
import { HeartIcon, ShoppingCartIcon } from "@heroicons/react/24/outline";

interface ProductCardProps {
  product: Product;
}

export default function ProductCard({ product }: ProductCardProps) {
  return (
    <div className="relative bg-white rounded-2xl shadow-md hover:shadow-lg transition flex flex-col w-70 h-fit group">
      {/* --- Badge giảm giá --- */}
      <div className="absolute z-10 top-3 bg-blue-100 text-blue-600 text-sm font-semibold px-2 py-1 rounded-r-lg">
        -12%
      </div>

      <Link href={`/products/${product.slug}`} className="m-4 block">
        {/* --- Ảnh sản phẩm --- */}
        <div className="relative w-full h-44 mt-5 overflow-hidden rounded-lg flex items-center justify-center">
          <img
            src={product.primaryImagePath}
            alt={product.name}
            width={256}
            height={190}
            className="object-cover w-full h-full"
          />

          {/* --- Màu sắc bên phải --- */}
          <div className="absolute right-2 top-1/2 -translate-y-1/2 flex flex-col items-center gap-2">
            <div className="w-4 h-4 bg-black rounded-full border border-gray-300 cursor-pointer"></div>
            <div className="w-4 h-4 bg-white rounded-full border border-gray-300 cursor-pointer"></div>
            <div className="w-4 h-4 bg-gray-400 rounded-full border border-gray-300 cursor-pointer"></div>
            <button className="text-lg font-semibold text-gray-500">+</button>
          </div>
        </div>

        {/* --- Tên sản phẩm --- */}
        <p className="text-gray-900 text-sm font-medium truncate w-full mt-3">
          {product.name}
        </p>
      </Link>

      {/* --- Giá và Rating --- */}
      <div className="relative flex items-center justify-between w-full px-4 pb-4">
        {/* --- Layer Hover (Add to cart + Heart) --- */}
        <div className="absolute inset-0 flex items-center justify-center gap-5 bg-white opacity-0 transition-all duration-100 group-hover:opacity-100 rounded-md">
          <button className="flex items-center gap-2 border border-yellow-400 text-yellow-500 px-4 py-2 rounded-md hover:bg-yellow-400 hover:text-white transition-all">
            <ShoppingCartIcon width={24} />
            Add to cart
          </button>
          <HeartIcon
            width={24}
            className="text-yellow-500 cursor-pointer hover:fill-yellow-500"
          />
        </div>

        {/* --- Layer Default (Giá + Rating) --- */}
        <div className="flex items-center justify-between w-full mt-2">
          <div className="flex flex-col">
            <p className="text-gray-400 text-sm line-through">
              ${(product.basePrice + 111.87).toFixed(2)}
            </p>
            <p className="text-lg font-semibold text-black">
              ${product.basePrice.toFixed(2)}
            </p>
          </div>
          <div className="flex items-center text-yellow-500">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="currentColor"
              viewBox="0 0 24 24"
              className="w-5 h-5"
            >
              <path d="M12 .587l3.668 7.568 8.332 1.151-6.064 5.871L19.8 24 12 19.897 4.2 24l1.864-8.823L0 9.306l8.332-1.151z" />
            </svg>
            <span className="ml-1 font-medium text-base">4.9</span>
          </div>
        </div>
      </div>
    </div>
  );
}
