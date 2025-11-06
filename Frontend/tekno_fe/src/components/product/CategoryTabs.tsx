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

interface CategoryTabsProps {
  onCategoryChange?: (category: Category) => void;
}

export function CategoryTabs({ onCategoryChange }: CategoryTabsProps) {
  const [categories, setCategories] = useState<Category[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(
    null
  );

  useEffect(() => {
    async function loadCategories() {
      try {
        const data = await getCategoriesList();
        setCategories(data.data);
        // chọn mặc định category đầu tiên
        if (data.data.length > 0) {
          setSelectedCategory(data.data[0]);
          onCategoryChange?.(data.data[0]);
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
    <Tabs
      value={selectedCategory?.slug ?? ""}
      onValueChange={(val) => {
        const cat = categories.find((c) => c.slug === val);
        if (cat) handleCategorySelect(cat);
      }}
      className="w-full flex justify-center h-max-content p-6"
    >
      {/* ✅ TabsList bọc Carousel */}
      <TabsList className="w-full bg-transparent px-4">
        <div className="relative w-full">
          <Carousel className="w-full mx-5">
            <CarouselContent>
              {categories.map((cat) => (
                <CarouselItem
                  key={cat.id}
                  className="basis-1/5 md:basis-1/10 flex justify-center"
                >
                  <TabsTrigger
                    value={cat.slug}
                    className="
                      relative flex flex-col items-center justify-center 
                      text-center px-3 py-2 text-gray-600 
                      data-[state=active]:text-primary
                      data-[state=hover]:text-secondary 
                      w-20 break-words
                    "
                  >
                    <img
                      src={cat.iconPath}
                      alt={cat.slug}
                      className="w-7 h-7"
                    ></img>

                    <span className="text-[11px] font-medium leading-tight pt-3">
                      {cat.name}
                    </span>
                    <span
                      className="absolute bottom-0 left-0 w-full h-[3px] bg-primary 
                      scale-x-0 data-[state=active]:scale-x-100 
                      transition-transform origin-center rounded-full"
                    />
                  </TabsTrigger>
                </CarouselItem>
              ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
          </Carousel>
        </div>
      </TabsList>
    </Tabs>
  );
}
