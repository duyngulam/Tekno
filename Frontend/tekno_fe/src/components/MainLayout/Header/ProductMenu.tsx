"use client";

import { useEffect, useState } from "react";
import { getCategoriesList } from "@/services/categories";
import { Category } from "@/type/categories";
import Image from "next/image";
import Link from "next/link";

export default function ProductMenu() {
  const [categories, setCategories] = useState<Category[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      const data = await getCategoriesList();
      setCategories(data);
    };
    fetchData();
  }, []);

  return (
    <div
      className="
        fixed left-1/2 -translate-x-1/2 top-[80px]
        bg-white shadow-xl rounded-lg 
        w-[900px] h-[420px] p-6 z-[999]
      "
    >
      <div className="flex h-full">
        {/* LEFT: Category list */}
        <div
          className="flex flex-col flex-1 border-r pr-4 
            max-h-64 overflow-y-auto scrollbar-thin scrollbar-thumb-gray-400"
        >
          {categories.map((cat) => (
            <Link
              href={`/products?category=${cat.slug}`}
              key={cat.id}
              className="flex items-center gap-5 py-2 cursor-pointer hover:text-primary hover:bg-gray-50 px-2 rounded"
            >
              <Image
                src={cat.iconPath}
                height={200}
                width={200}
                alt="icon"
                sizes="icon"
                className="w-10 h-10"
              />

              <p> {cat.name} </p>
            </Link>
          ))}
        </div>

        {/* RIGHT: Content preview */}
        <div className="flex-1 pl-4">
          <p className="text-gray-500">
            Chọn danh mục để xem sản phẩm / hình ảnh / nội dung preview.
          </p>
        </div>
      </div>
    </div>
  );
}
