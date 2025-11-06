import { Search, SlidersHorizontal } from "lucide-react";
import React, { useEffect, useState } from "react";
import { Checkbox } from "../ui/checkbox";
import { Label } from "../ui/label";

import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Brand } from "@/type/brand";
import { getBrandList } from "@/services/brand";

export default function FilterCategories() {
  const [brandList, setbrandList] = useState<Brand[]>([]);

  useEffect(() => {
    async function fetchBrandList() {
      const data = await getBrandList();
      setbrandList(data.data);
    }
    fetchBrandList();
  }, []);
  return (
    <div>
      <aside className="lg:col-span-1">
        <div className="bg-white rounded-lg p-6 sticky top-24">
          <div className="flex items-center justify-between mb-6">
            <h3 className="flex items-center gap-2">
              <SlidersHorizontal className="w-5 h-5" />
              Filters
            </h3>
            <button className="text-sm text-secondary hover:underline">
              Clear All
            </button>
          </div>

          <Accordion
            type="multiple"
            defaultValue={[
              "brand",
              "color",
              "ram",
              "screen",
              "processor",
              "gpu",
              "drive",
            ]}
          >
            <AccordionItem value="brand">
              <AccordionTrigger>Brand</AccordionTrigger>
              <AccordionContent>
                {brandList &&
                  brandList.map((brand) => (
                    <div className="flex items-center space-x-2 space-y-2">
                      <Checkbox id={brand.id.toString()} />
                      <Label htmlFor={brand.id.toString()}>{brand.name}</Label>
                    </div>
                  ))}
              </AccordionContent>
            </AccordionItem>
          </Accordion>

          {/* Categories */}
          <div className="mb-6 pb-6 border-b border-gray-200">
            <h4 className="mb-3">Categories</h4>
            <div className="space-y-2">
              {/* {categories.map((category, index) => (
                <label
                  key={index}
                  className="flex items-center justify-between cursor-pointer group"
                >
                  <div className="flex items-center gap-2">
                    <input
                      type="checkbox"
                      className="rounded border-gray-300 text-secondary focus:ring-primary"
                      defaultChecked={index === 0}
                    />
                    <span className="text-sm group-hover:text-secondary transition-colors">
                      {category.name}
                    </span>
                  </div>
                  <span className="text-xs text-gray-500">
                    ({category.count})
                  </span>
                </label>
              ))} */}
            </div>
          </div>

          {/* Price Range */}
          <div className="mb-6 pb-6 border-b border-gray-200">
            <h4 className="mb-3">Price Range</h4>
            <div className="space-y-2">
              {/* {priceRanges.map((range, index) => (
                <label
                  key={index}
                  className="flex items-center gap-2 cursor-pointer group"
                >
                  <input
                    type="checkbox"
                    className="rounded border-gray-300 text-secondary focus:ring-primary"
                  />
                  <span className="text-sm group-hover:text-secondary transition-colors">
                    {range}
                  </span>
                </label>
              ))} */}
            </div>
          </div>

          {/* Brands */}
          <div className="mb-6 pb-6 border-b border-gray-200">
            <h4 className="mb-3">Brands</h4>
            <div className="space-y-2">
              <div className="flex items-center space-x-2">
                <Checkbox id="terms" />
                <Label htmlFor="terms">Accept terms and conditions</Label>
              </div>
              {/* {brands.map((brand, index) => (
                <label
                  key={index}
                  className="flex items-center gap-2 cursor-pointer group"
                >
                  <input
                    type="checkbox"
                    className="rounded border-gray-300 text-secondary focus:ring-primary"
                  />
                  <span className="text-sm group-hover:text-secondary transition-colors">
                    {brand}
                  </span>
                </label>
              ))} */}
            </div>
          </div>

          {/* Rating */}
          <div>
            <h4 className="mb-3">Rating</h4>
            <div className="space-y-2">
              {[5, 4, 3, 2, 1].map((rating) => (
                <label
                  key={rating}
                  className="flex items-center gap-2 cursor-pointer group"
                >
                  <input
                    type="checkbox"
                    className="rounded border-gray-300 text-secondary focus:ring-primary"
                  />
                  <div className="flex items-center gap-1">
                    {[...Array(rating)].map((_, i) => (
                      <span key={i} className="text-primary">
                        ★
                      </span>
                    ))}
                    {[...Array(5 - rating)].map((_, i) => (
                      <span key={i} className="text-gray-300">
                        ★
                      </span>
                    ))}
                  </div>
                  <span className="text-sm text-gray-500">& up</span>
                </label>
              ))}
            </div>
          </div>
        </div>
      </aside>
    </div>
  );
}
