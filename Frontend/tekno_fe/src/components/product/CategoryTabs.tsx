"use client";
import { useEffect, useState } from "react";
import { getCategoriesList } from "@/services/categories";
import { Category } from "@/type/categories";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import Image from "next/image";

export function CategoryTabs({ queryCategory }: { queryCategory: string }) {
  const [categories, setCategories] = useState<Category[]>([]);
  //const searchParams = useSearchParams();
  //const queryCategory = searchParams.get("category") || "";

  useEffect(() => {
    async function loadCategories() {
      try {
        const data = await getCategoriesList();
        setCategories(data);
      } catch (error) {
        console.error("❌ Lỗi khi lấy categories:", error);
      }
    }
    loadCategories();
  }, []);

  return (
    <div className="flex overflow-x-auto scroll-smooth no-scrollbar gap-2 mx-30">
      {categories.map((category) => (
        <Link
          href={`/products?category=${category.slug}`}
          key={category.id}
          className="flex flex-col items-center gap-2 min-w-30 relative group"
          // onClick={() => handleCategorySelect(category)}
        >
          {category.iconPath && (
            <Image
              src={category.iconPath}
              alt={category.slug}
              width={28}
              height={28}
              className="w-7 h-7 "
            ></Image>
          )}

          <div className="text-[14px] text-center font-medium leading-tight pt-3">
            {category.name}
          </div>
          <span
            className={`absolute -bottom-0 left-0 w-0 h-0.5 bg-primary group-hover:w-full hoverEffect ${
              category.slug == queryCategory && "w-full"
            } 
              `}
          />
        </Link>
      ))}
    </div>
  );
}
