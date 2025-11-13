"use client";
import { getCategoriesList } from "@/services/categories";
import { Category } from "@/type/categories";
import { log } from "console";
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
    <div className="border border-primary gap-5 flex overflow-x-auto scroll-smooth snap-x snap-mandatory ">
      {categoriesList.map((category) => (
        <div
          key={category.id}
          className="bg-white min-w-[200px] rounded-xl border border-primary flex flex-col items-center justify-center gap-5 p-5"
        >
          <div>Image</div>
          <div>{category.name}</div>
        </div>
      ))}
    </div>
  );
}
