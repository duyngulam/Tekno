"use client";
import React, { useEffect, useState } from "react";
import { Grid3x3, List, ChevronDown, Loader2 } from "lucide-react";
import { FilterChips } from "@/components/product/FilterChips";
import { CategoryTabs } from "@/components/product/CategoryTabs";
import Filter from "@/components/product/Filter";
import ProductCard from "@/components/product/ProductCard";
import { Product } from "@/type/product";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { Container } from "@/components/MainLayout/Container";
import { getProductsList } from "@/services/products";

import { AnimatePresence, motion } from "motion/react";

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
import NoProductAvailable from "@/components/product/NoProductAvailable";

export default function Products() {
  const [loading, setLoading] = useState(false);
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

  const params = { category: selectedCategory.toLocaleLowerCase() };
  useEffect(() => {
    const fecthProductList = async () => {
      setLoading(true);
      try {
        const res = await getProductsList(
          page,
          pageSize,
          selectedCategory,
          sortBy
        );
        console.log("respon:", res);
        setproductsList(res.data);
      } catch (error) {
        console.error("Product fetch error", error);
      } finally {
        setLoading(false);
      }
    };
    fecthProductList();
  }, [selectedCategory]);

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

  // useEffect(() => {
  //   async function fetchproductsList(
  //     page: number,
  //     pageSize: number,
  //     selectedCategory: string,
  //     sortBy: string
  //   ) {
  //     const data = await getProductsList(
  //       page,
  //       pageSize,
  //       selectedCategory,
  //       sortBy
  //     );
  //     console.log(data);
  //     setproductsList(data.data);
  //     setTotalRecords(data.totalRecords);
  //     setTotalPages(data.totalPages);
  //   }
  //   fetchproductsList(page, pageSize, selectedCategory, sortBy);
  // }, [page, pageSize, selectedCategory]);

  //console.log("productsList:", productsList);

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
        <div className="flex">
          <div className="hidden lg:block w-3/12">
            <Filter />
          </div>

          {/* Content chính */}
          <div className="w-full md:w-9/12 space-y-8">
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
            {loading ? (
              <div className="flex flex-col items-center justify-center py-10 min-h-80 gap-4 bg-gray-100 w-full mt-10">
                <div className="space-x-2 flex items-center">
                  <Loader2 className="w-5 h-5 animate-spin" />
                  <span>Product is loading...</span>
                </div>
              </div>
            ) : productsList?.length ? (
              <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-5 ">
                {productsList.map((product) => (
                  <AnimatePresence key={product?.id}>
                    <motion.div>
                      <ProductCard key={product.id} product={product} />
                    </motion.div>
                  </AnimatePresence>
                ))}
              </div>
            ) : (
              <NoProductAvailable selectedCategory={selectedCategory} />
            )}
            <div></div>

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
          </div>
        </div>
      </Container>
    </>
  );
}
