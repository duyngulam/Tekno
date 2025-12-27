import { getCategoriesList } from "@/services/categories";
import { ArrowRightCircle, ChevronRight } from "lucide-react";
import Link from "next/link";
import React from "react";
const data = await getCategoriesList();
const category = data[0].slug;
export default function ViewAllButton() {
  return (
    <Link
      href={`/products?category=${category}`}
      className="flex items-center gap-2 hoverEffect mx-10 hover:cursor-pointer"
    >
      View all <ChevronRight className="w-5 h-5" />
    </Link>
  );
}
