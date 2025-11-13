"use client";

import { useEffect, useState } from "react";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/components/ui/carousel";
import { getCategoriesList } from "@/services/categories";
import { Category } from "@/type/categories";
import { Laptop } from "lucide-react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";

interface CategoryTabsProps {
  onCategoryChange?: (category: Category) => void;
}

export function CategoryTabs({ onCategoryChange }: CategoryTabsProps) {
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(
    null
  );

  const searchParams = useSearchParams();
  const queryCategory = searchParams.get("category") || "";

  useEffect(() => {
    async function loadCategories() {
      try {
        const data = await getCategoriesList();
        setCategories(data.data);
        // chọn mặc định category đầu tiên
        if (data.data.length > 0) {
          //setSelectedCategory(data.data[0]);
          //onCategoryChange?.(data.data[0]);
        }
      } catch (error) {
        console.error("❌ Lỗi khi lấy categories:", error);
      }
    }
    loadCategories();
  }, []);

  const handleCategorySelect = (cat: Category) => {
    setSelectedCategory(cat);
    onCategoryChange?.(cat);
  };

  return (
    <div className="flex overflow-x-auto scroll-smooth no-scrollbar gap-2 mx-30">
      {categories.map((category) => (
        <Link
          href={`/products?category=${category.slug}`}
          key={category.id}
          className="flex flex-col items-center gap-2 min-w-30 relative group"
          // onClick={() => handleCategorySelect(category)}
        >
          <img
            src={category.iconPath}
            alt={category.slug}
            className="w-7 h-7 "
          ></img>

          <div className="text-[14px] text-center font-medium leading-tight pt-3">
            {category.name}
          </div>
          <span
            className={`absolute -bottom-0 left-0 w-0 h-0.5 bg-primary group-hover:w-full hoverEffect ${
              category.slug == queryCategory && "w-full"
            } 
              `}
          />
          {/* <span
            className="absolute bottom-0 left-0 w-full h-1 bg-primary 
                      scale-x-0 data-[state=active]:scale-x-100 hover:scale-x-100
                      transition-transform origin-center rounded-full"
          /> */}
        </Link>
      ))}
    </div>
  );
}
