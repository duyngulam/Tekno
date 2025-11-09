"use client";
import React, { useEffect, useState } from "react";
import { Grid3x3, List, ChevronDown } from "lucide-react";
import { FilterChips } from "@/components/product/FilterChips";
import { CategoryTabs } from "@/components/product/CategoryTabs";
import Filter from "@/components/product/Filter";
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
  const [selectedBrands, setSelectedBrands] = useState<string[]>([]);
  const [filters, setFilters] = useState<string[]>([
    "Silver",
    "Intel Core i9",
    "Apple",
    "12 GB",
  ]);
  const [sortBy, setSortBy] = useState("newest");
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(15);
  const [totalRecords, setTotalRecords] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(1);

  const HandleAddFilter = (f: string) => {
    setFilters((prev) => {
      if (prev.includes(f)) return prev;
      return [...prev, f];
    });
  };

  const HandleRemoveFilter = (f: string) => {
    setFilters((prev) => prev.filter((item) => item !== f));
  };

  const handleCategoryChange = (category: Category) => {
    console.log("✅ Category được chọn:", category);
    setSelectedCategory(category.slug);
  };

  useEffect(() => {
    async function fetchproductsList(
      page: number,
      pageSize: number,
      selectedCategory: string,
      sortBy: string
    ) {
      const data = await getProductsList(
        page,
        pageSize,
        selectedCategory,
        sortBy
      );
      console.log(data);
      setproductsList(data.data);
      setTotalRecords(data.totalRecords);
      setTotalPages(data.totalPages);
    }
    fetchproductsList(page, pageSize, selectedCategory, sortBy);
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
        </div>
        <div className="col-span-12">
          <FilterChips
            filters={filters}
            HandleRemoveFilter={HandleRemoveFilter}
          />
        </div>
        {/* Sidebar */}
        {/* tutu tinh */}
        <aside className="hidden lg:block col-span-3">
          <Filter />
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
              <Select value={sortBy} onValueChange={setSortBy}>
                <SelectTrigger className="w-[180px]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="newest">Newest First</SelectItem>
                  <SelectItem value="asc">Price: Low to High</SelectItem>
                  <SelectItem value="dasc">Price: High to Low</SelectItem>
                  <SelectItem value="best">Best Rating</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Danh sách sản phẩm */}
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-4 ">
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
