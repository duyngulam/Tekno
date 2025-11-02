"use client";
import React from "react";
import { Grid3x3, List, ChevronDown } from "lucide-react";
import { FilterChips } from "@/components/product/FilterChips";
import { CategoryTabs } from "@/components/product/CategoryTabs";
import FilterCategories from "@/components/product/FilterCategories";
import ProductCard from "@/components/product/ProductCard";
import { Product } from "@/type/product";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { Container } from "@/components/MainLayout/Container";

export default function Products() {
  const products: Product[] = [
    {
      id: 1,
      name: 'MacBook Pro 16" M2 Max',
      slug: "macbook-pro-16-m2-max",
      basePrice: 2499,
      overview:
        "Powerful laptop with Apple M2 Max chip and Liquid Retina XDR display.",
      brandName: "Apple",
      primaryImageUrl:
        "https://images.unsplash.com/photo-1754928864131-21917af96dfd?w=400",
    },
    {
      id: 2,
      name: 'iPad Pro 12.9" 256GB',
      slug: "ipad-pro-12-9-256gb",
      basePrice: 1099,
      overview: "High-performance tablet with M2 chip and ProMotion display.",
      brandName: "Apple",
      primaryImageUrl:
        "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400",
    },
    {
      id: 3,
      name: "Sony WH-1000XM5 Headphones",
      slug: "sony-wh-1000xm5-headphones",
      basePrice: 349,
      overview:
        "Noise-cancelling wireless headphones with exceptional sound quality.",
      brandName: "Sony",
      primaryImageUrl:
        "https://images.unsplash.com/photo-1660391532247-4a8ad1060817?w=400",
    },
    {
      id: 4,
      name: "Logitech MX Master 3S",
      slug: "logitech-mx-master-3s",
      basePrice: 99,
      overview:
        "Ergonomic wireless mouse with advanced precision and quiet clicks.",
      brandName: "Logitech",
      primaryImageUrl:
        "https://images.unsplash.com/photo-1660491083562-d91a64d6ea9c?w=400",
    },
    {
      id: 5,
      name: "Samsung Galaxy S24 Ultra",
      slug: "samsung-galaxy-s24-ultra",
      basePrice: 1199,
      overview:
        "Flagship smartphone with pro-grade camera and AI-powered performance.",
      brandName: "Samsung",
      primaryImageUrl:
        "https://images.unsplash.com/photo-1675953935267-e039f13ddd79?w=400",
    },
    {
      id: 6,
      name: 'Dell UltraSharp 27" 4K',
      slug: "dell-ultrasharp-27-4k",
      basePrice: 599,
      overview: "27-inch 4K monitor with ultra-thin bezels and color accuracy.",
      brandName: "Dell",
      primaryImageUrl:
        "https://images.unsplash.com/photo-1593833210845-d9935371664e?w=400",
    },
  ];

  return (
    <>
      {/* Breadcrumb nằm ngoài grid để full width nếu muốn */}
      {/* <div className="bg-gray-50 py-4 mb-6">
        <Container></Container>
      </div> */}

      {/* Container chính */}
      <Container>
        <div className="col-span-12">
          <Breadcrumb />
        </div>
        {/* Sidebar */}
        <aside className="hidden lg:block col-span-3">
          <FilterCategories />
        </aside>

        {/* Content chính */}
        <section className="col-span-12 lg:col-span-9 space-y-8">
          {/* Bộ lọc */}
          <div>
            <CategoryTabs />
            <FilterChips />
          </div>

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
