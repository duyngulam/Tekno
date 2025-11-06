"use client";
import React, { useEffect, useState } from "react";
import { Grid3x3, List, ChevronDown } from "lucide-react";
import { FilterChips } from "@/components/product/FilterChips";
import { CategoryTabs } from "@/components/product/CategoryTabs";
import FilterCategories from "@/components/product/FilterCategories";
import ProductCard from "@/components/product/ProductCard";
import { Product } from "@/type/product";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { Container } from "@/components/MainLayout/Container";
import { getProductsList } from "@/services/products";

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import { Category } from "@/type/categories";

export default function Products() {
  const [productsList, setproductsList] = useState<Product[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>("");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(15);
  const [totalRecords, setTotalRecords] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);

  const handleCategoryChange = (category: Category) => {
    console.log("✅ Category được chọn:", category);
    setSelectedCategory(category.slug);
  };

  useEffect(() => {
    async function fetchproductsList(
      page: number,
      pageSize: number,
      selectedCategory: string
    ) {
      const data = await getProductsList(page, pageSize, selectedCategory);
      console.log(data);
      setproductsList(data.data);
      setTotalRecords(data.totalRecords);
      setTotalPages(data.totalPages);
    }
    fetchproductsList(page, pageSize, selectedCategory);
  }, [page, pageSize, selectedCategory]);

  console.log("productsList:", productsList);

  return (
    <>
      {/* Container chính */}
      <Container>
        <div className="col-span-12">
          <Breadcrumb />
        </div>

        <div className="col-span-12">
          <CategoryTabs onCategoryChange={handleCategoryChange} />
          <FilterChips />
        </div>
        {/* Sidebar */}
        <aside className="hidden lg:block col-span-3">
          <FilterCategories />
        </aside>

        {/* Content chính */}
        <section className="col-span-12 lg:col-span-9 space-y-8">
          {/* Bộ lọc */}

          {/* Thanh công cụ */}
          <div className="bg-white border border-gray-200 rounded-xl p-4 flex flex-wrap justify-between items-center gap-4 shadow-sm">
            <p className="text-gray-600 text-sm">
              Showing{" "}
              <span className="font-semibold text-gray-800">
                {totalRecords}
              </span>{" "}
              results
            </p>

            <div className="flex items-center gap-4">
              {/* <div className="flex items-center gap-1 border border-gray-300 rounded-lg p-1 bg-gray-50">
                <button className="p-2 bg-primary text-white rounded hover:bg-primary/90 transition">
                  <Grid3x3 className="w-4 h-4" />
                </button>
                <button className="p-2 text-gray-600 hover:bg-gray-100 rounded transition">
                  <List className="w-4 h-4" />
                </button>
              </div> */}

              {/* Sort */}
              <Select>
                <SelectTrigger className="w-[180px]">
                  <SelectValue placeholder="Sort" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="light">Newest First</SelectItem>
                  <SelectItem value="dark">Price: Low to High</SelectItem>
                  <SelectItem value="system">Price: High to Low</SelectItem>
                  <SelectItem value="best-rating">Best Rating</SelectItem>
                </SelectContent>
              </Select>

              {/* <div className="relative">
                <select className="appearance-none px-4 py-2 pr-10 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer bg-white">
                  <option>Newest First</option>
                  <option>Price: Low to High</option>
                  <option>Price: High to Low</option>
                  <option>Best Rating</option>
                </select>
                <ChevronDown className="w-4 h-4 text-gray-400 absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none" />
              </div> */}
            </div>
          </div>

          {/* Danh sách sản phẩm */}
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
            {productsList.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>

          {/* Phân trang */}
          <Pagination>
            <PaginationContent>
              {/* Nút Previous */}
              <PaginationItem>
                <PaginationPrevious
                  href="#"
                  onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
                />
              </PaginationItem>

              {/* Các trang */}
              {Array.from({ length: totalPages }, (_, i) => (
                <PaginationItem key={i}>
                  <PaginationLink
                    href="#"
                    isActive={page === i + 1}
                    onClick={() => setPage(i + 1)}
                  >
                    {i + 1}
                  </PaginationLink>
                </PaginationItem>
              ))}

              {/* Nút Next */}
              <PaginationItem>
                <PaginationNext
                  href="#"
                  onClick={() =>
                    setPage((prev) => Math.min(prev + 1, totalPages))
                  }
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </section>
      </Container>
    </>
  );
}
