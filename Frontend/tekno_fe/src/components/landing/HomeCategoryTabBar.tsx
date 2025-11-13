"use client";
import { getCategoriesList } from "@/services/categories";
import { Category } from "@/type/categories";
import { log } from "console";
import Image from "next/image";
import Link from "next/link";
import React, { useEffect, useState } from "react";

export default function HomeCategoryTabBar() {
  const [categoriesList, setCategoriesList] = useState<Category[]>([]);
  const [selectedTab, setSelectedTab] = useState<string>(
    categoriesList[0]?.name || ""
  );

  useEffect(() => {
    const fetchCategoriesList = async () => {
      try {
        const res = await getCategoriesList();
        console.log(res);
        setCategoriesList(res.data);
      } catch (error) {
        console.log("error in fetching category:", error);
      }
    };
    fetchCategoriesList();
  }, [selectedTab]);

  if (categoriesList && !categoriesList?.length) {
    return <div>Rỗng</div>;
  }

  return (
    <div className="border border-primary gap-5 flex overflow-x-auto no-scrollbar scroll-smooth snap-x snap-mandatory ">
      {categoriesList.map((category) => (
        <div
          key={category.id}
          className="bg-white rounded-xl flex flex-col items-center gap-3 p-2"
        >
          {category?.iconPath && (
            <div className="overflow-hidden border border-secondary/50 hover:border-secondary hoverEffect w-30 h-30 p-2">
              <Link href={`/products?category=${category.slug}`}>
                <Image
                  src={category.iconPath}
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
