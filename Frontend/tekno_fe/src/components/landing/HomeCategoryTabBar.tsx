import { getCategoriesList } from "@/services/categories";
import { Category } from "@/type/categories";
import { log } from "console";
import Image from "next/image";
import Link from "next/link";
import React from "react";

export default async function HomeCategoryTabBar() {
  const categoriesList = await getCategoriesList();
  if (categoriesList && !categoriesList?.length) {
    return <div>Rỗng</div>;
  }

  return (
    <div className="gap-5 flex overflow-x-auto no-scrollbar scroll-smooth snap-x snap-mandatory ">
      {categoriesList.map((category) => (
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
