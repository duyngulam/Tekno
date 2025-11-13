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
import { Slider } from "../ui/slider";
import { RangeSlider } from "../ui/range-slide";
import { Switch } from "../ui/switch";

export default function Filter() {
  const [brandList, setbrandList] = useState<Brand[]>([]);
  const [priceRange, setPriceRange] = useState<[number, number]>([500, 2000]);

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
          <div className="flex items-center justify-between">
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
              "discount",
              "price",
              "ram",
              "screen",
              "processor",
              "gpu",
              "drive",
            ]}
          >
            {/* brand */}
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

            {/* color */}
            <AccordionItem value="color">
              <AccordionTrigger>Color</AccordionTrigger>
              <AccordionContent></AccordionContent>
            </AccordionItem>
            {/* discount */}
            <AccordionItem value="discount">
              <div className="flex items-center justify-between py-4 text-base font-semibold text-black hover:text-yellow-400 transition-all">
                <span>Discount</span>
                <Switch />
              </div>
            </AccordionItem>

            {/* price */}
            <AccordionItem value="price">
              <AccordionTrigger>Price</AccordionTrigger>
              <AccordionContent>
                <div className="flex justify-center gap-2 pb-4 ">
                  <input
                    type="number"
                    value={priceRange[0]}
                    onChange={(e) =>
                      setPriceRange([+e.target.value, priceRange[1]])
                    }
                    className="w-20 border rounded-md px-2 py-1 text-lg text-center"
                    placeholder="min"
                  />
                  <input
                    type="number"
                    value={priceRange[1]}
                    onChange={(e) =>
                      setPriceRange([priceRange[0], +e.target.value])
                    }
                    className="w-20 border rounded-md px-2 py-1 text-lg text-center"
                    placeholder="max"
                  />
                </div>
                <Slider
                  min={0}
                  max={5000}
                  step={100}
                  defaultValue={[20, 80]}
                  value={priceRange}
                  onValueChange={(v) => setPriceRange(v as [number, number])}
                  className="[&>[data-orientation=horizontal]]:bg-yellow-200"
                />
              </AccordionContent>
            </AccordionItem>

            {/* ram */}
            <AccordionItem value="ram">
              <AccordionTrigger>Ram</AccordionTrigger>
              <AccordionContent></AccordionContent>
            </AccordionItem>

            {/* screen size */}
            <AccordionItem value="screen">
              <AccordionTrigger>Screen Size</AccordionTrigger>
              <AccordionContent></AccordionContent>
            </AccordionItem>

            {/* Processor */}
            <AccordionItem value="processor">
              <AccordionTrigger>Processor</AccordionTrigger>
              <AccordionContent></AccordionContent>
            </AccordionItem>

            {/* GPU */}
            <AccordionItem value="GPU">
              <AccordionTrigger>GPU brand</AccordionTrigger>
              <AccordionContent></AccordionContent>
            </AccordionItem>

            {/* Drive Size */}
            <AccordionItem value="drive">
              <AccordionTrigger>Drive Size</AccordionTrigger>
              <AccordionContent></AccordionContent>
            </AccordionItem>
          </Accordion>
        </div>
      </aside>
    </div>
  );
}
