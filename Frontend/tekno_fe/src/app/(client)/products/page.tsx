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
import { getProductList } from "@/services/products";
import Link from "next/link";

export default function Products() {
  const [products, setProducts] = useState<Product[]>([]);

  useEffect(() => {
    async function fetchProducts() {
      const data = await getProductList();
      //const data = await res.json();
      setProducts(data);
    }
    fetchProducts();
  }, []);

  return (
    <>
      {/* Container chính */}
      <Container>
        <div className="col-span-12">
          <Breadcrumb />
        </div>

        <div className="col-span-12">
          <CategoryTabs />
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
              Showing <span className="font-semibold text-gray-800">254</span>{" "}
              results
            </p>

            <div className="flex items-center gap-4">
              <div className="flex items-center gap-1 border border-gray-300 rounded-lg p-1 bg-gray-50">
                <button className="p-2 bg-primary text-white rounded hover:bg-primary/90 transition">
                  <Grid3x3 className="w-4 h-4" />
                </button>
                <button className="p-2 text-gray-600 hover:bg-gray-100 rounded transition">
                  <List className="w-4 h-4" />
                </button>
              </div>

              <div className="relative">
                <select className="appearance-none px-4 py-2 pr-10 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer bg-white">
                  <option>Sort by: Featured</option>
                  <option>Price: Low to High</option>
                  <option>Price: High to Low</option>
                  <option>Newest First</option>
                  <option>Best Rating</option>
                </select>
                <ChevronDown className="w-4 h-4 text-gray-400 absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none" />
              </div>
            </div>
          </div>

          {/* Danh sách sản phẩm */}
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
            {products.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>

          {/* Phân trang */}
          <div className="flex justify-center items-center gap-2 mt-10">
            <button className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 text-sm transition">
              Previous
            </button>
            <button className="px-4 py-2 bg-primary text-white rounded-lg shadow-sm">
              1
            </button>
            <button className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 text-sm transition">
              2
            </button>
            <button className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 text-sm transition">
              3
            </button>
            <button className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 text-sm transition">
              Next
            </button>
          </div>
        </section>
      </Container>
    </>
  );
}
