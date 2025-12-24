"use client";

import { getCategoriesList } from "@/services/categories";
import { Category } from "@/type/categories";
import Image from "next/image";
import Link from "next/link";
import React, { useEffect, useState } from "react";

export default function HomeCategoryTabBar() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        const res = await getCategoriesList();
        if (mounted) setCategories(res ?? []);
      } catch (e) {
        console.error("fetch categories error:", e);
      } finally {
        if (mounted) setLoading(false);
      }
    })();
    return () => {
      mounted = false;
    };
  }, []);

  if (loading)
    return <div className="py-4 text-sm text-gray-500">Loading…</div>;
  if (!categories.length)
    return <div className="py-4 text-sm text-gray-500">Rỗng</div>;

  return (
    <div className="gap-5 flex overflow-x-auto scrollbar scroll-smooth snap-x snap-mandatory">
      {categories.map((category) => (
        <div
          key={category.id}
          className="bg-white rounded-xl flex flex-col items-center gap-3 p-2"
        >
          {category?.imageUrl && (
            <div className="overflow-hidden hoverEffect w-30 h-30 p-2">
              <Link href={`/products?category=${category.slug}`}>
                <Image
                  src={category.imageUrl}
                  alt="categoryimage"
                  width={50}
                  height={50}
                  className="w-full h-full object-contain group-hover:scale-110 hoverEffect"
                />
              </Link>
            </div>
          )}
          <div className="text-center">{category.name}</div>
        </div>
      ))}
    </div>
  );
}
