"use client";

import { useEffect, useState } from "react";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { getCategoriesList } from "@/services/categories";
import {
  Smartphone,
  Laptop,
  Tablet,
  Headphones,
  Watch,
  Camera,
  Gamepad2,
  Network,
  Sun,
} from "lucide-react";
import { Category } from "@/type/categories";

export function CategoryTabs() {
  const [categories, setCategories] = useState<Category[]>([]);

  useEffect(() => {
    async function loadCategories() {
      try {
        const data = await getCategoriesList();
        setCategories(data.data);
      } catch (error) {
        console.error("❌ Lỗi khi lấy categories:", error);
      }
    }
    loadCategories();
  }, []);

  return (
    <Tabs
      defaultValue="laptop"
      className="w-full flex justify-center h-max-content"
    >
      {/* ✅ Căn giữa TabsList */}
      <TabsList
        className="
          flex flex-wrap justify-center items-center 
          gap-4 bg-transparent border-b border-gray-200 
          max-w-[600px] mx-auto h-max
        "
      >
        {categories.length > 0 &&
          categories.map((cat) => (
            <TabsTrigger
              key={cat.id}
              value={cat.name.toLowerCase()}
              className="
                relative flex flex-col items-center justify-center 
                text-center px-3 py-2 text-gray-600 
                data-[state=active]:text-cyan-600 
                w-20 break-words
              "
            >
              <Laptop className="w-5 h-5 shrink-0" />
              <span className="text-[11px] font-medium leading-tight">
                {cat.name}
              </span>
              <span
                className="absolute bottom-0 left-0 w-full h-[3px] bg-yellow-400 
                scale-x-0 data-[state=active]:scale-x-100 
                transition-transform origin-center rounded-full"
              />
            </TabsTrigger>
          ))}
      </TabsList>
    </Tabs>
  );
}
