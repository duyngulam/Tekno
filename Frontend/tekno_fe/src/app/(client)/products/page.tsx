"use client";
import React from "react";
import { BreadcrumbWithCustomSeparator } from "@/components/share/breadcumbCustom";
import { Grid3x3, List, ChevronDown } from "lucide-react";
import { FilterChips } from "@/components/product/FilterChips";
import { CategoryTabs } from "@/components/product/CategoryTabs";
import FilterCategories from "@/components/product/FilterCategories";
import ProductCard from "@/components/product/ProductCard";
import { Product } from "@/type/product";

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
    <div className="px-6 lg:px-12 py-8 bg-gray-50 min-h-screen">
      {/* Breadcrumb */}
      <div className="mb-6">
        <BreadcrumbWithCustomSeparator />
      </div>

      {/* Categories + Chips */}
      <CategoryTabs />
      <div className="mt-4 mb-8">
        <FilterChips />
      </div>

      <div className="grid lg:grid-cols-4 gap-8">
        {/* Sidebar */}
        <aside className="hidden lg:block">
          <FilterCategories />
        </aside>

        {/* Main Content */}
        <section className="lg:col-span-3">
          {/* Toolbar */}
          <div className="bg-white shadow-sm border border-gray-200 rounded-xl p-4 mb-6 flex items-center justify-between flex-wrap gap-4">
            <p className="text-gray-600 text-sm">
              Showing <span className="font-semibold text-gray-800">254</span>{" "}
              results
            </p>

            <div className="flex items-center gap-4">
              {/* View Toggle */}
              <div className="flex items-center gap-1 border border-gray-300 rounded-lg p-1 bg-gray-50">
                <button className="p-2 bg-primary text-white rounded transition hover:bg-primary/90">
                  <Grid3x3 className="w-4 h-4" />
                </button>
                <button className="p-2 text-gray-600 hover:bg-gray-100 rounded transition">
                  <List className="w-4 h-4" />
                </button>
              </div>

              {/* Sort */}
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

          {/* Products Grid */}
          <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
            {products.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>

          {/* Pagination */}
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
      </div>
    </div>
  );
}
