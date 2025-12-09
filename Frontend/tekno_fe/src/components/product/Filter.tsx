import { SlidersHorizontal } from "lucide-react";
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
import { getCategoryAttributes } from "@/services/categories";
import { CategoryAttribute } from "@/type/categories";

export default function Filter({
  selectedBrand,
  minPrice,
  maxPrice,
  onBrandChange,
  onMinPriceChange,
  onMaxPriceChange,
  categoryId, // optional: id category để load attributes
  onAttributesChange, // optional callback nhận filters hiện tại
}: {
  selectedBrand?: string;
  minPrice?: number;
  maxPrice?: number;
  onBrandChange: (value: string) => void;
  onMinPriceChange: (value: number) => void;
  onMaxPriceChange: (value: number) => void;
  categoryId?: number;
  onAttributesChange?: (filters: Record<string, string[]>) => void;
}) {
  const [brandList, setbrandList] = useState<Brand[]>([]);
  const [priceRange, setPriceRange] = useState<[number, number]>([500, 2000]);

  // DYNAMIC ATTRIBUTES
  const [attributes, setAttributes] = useState<CategoryAttribute[]>([]);
  const [selectedAttributes, setSelectedAttributes] = useState<
    Record<string, string[]>
  >({});

  useEffect(() => {
    async function fetchBrandList() {
      const data = await getBrandList();
      setbrandList(data.data);
    }
    fetchBrandList();
  }, []);

  // fetch attributes khi categoryId thay đổi
  useEffect(() => {
    if (!categoryId) {
      setAttributes([]);
      return;
    }
    let mounted = true;
    (async () => {
      try {
        const attrs = await getCategoryAttributes(categoryId);
        if (mounted) setAttributes(attrs || []);
      } catch (err) {
        console.error("Failed to load category attributes", err);
        if (mounted) setAttributes([]);
      }
    })();
    return () => {
      mounted = false;
    };
  }, [categoryId]);

  // toggle giá trị attribute
  const toggleAttributeValue = (
    attrName: string,
    value: string,
    checked: boolean
  ) => {
    setSelectedAttributes((prev) => {
      const cur = new Set(prev[attrName] || []);
      if (checked) cur.add(value);
      else cur.delete(value);

      const next = { ...prev, [attrName]: Array.from(cur) };
      onAttributesChange?.(next); // notify parent
      return next;
    });
  };

  const clearAll = () => {
    setSelectedAttributes({});
    setPriceRange([0, 0]);
    onBrandChange("");
    onMinPriceChange(0);
    onMaxPriceChange(0);
    onAttributesChange?.({});
  };

  return (
    <div>
      <aside className="lg:col-span-1">
        <div className="bg-white rounded-lg p-6 sticky top-24">
          <div className="flex items-center justify-between">
            <h3 className="flex items-center gap-2">
              <SlidersHorizontal className="w-5 h-5" />
              Filters
            </h3>
            <button
              className="text-sm text-secondary hover:underline"
              onClick={clearAll}
            >
              Clear All
            </button>
          </div>

          <Accordion type="multiple" defaultValue={["brand", "price"]}>
            {/* brand */}
            <AccordionItem value="brand">
              <AccordionTrigger>Brand</AccordionTrigger>
              <AccordionContent>
                {brandList.map((brand) => (
                  <div
                    key={brand.id}
                    className="flex items-center space-x-2 space-y-2"
                  >
                    <Checkbox
                      id={brand.id.toString()}
                      checked={selectedBrand === brand.slug}
                      onCheckedChange={(checked) =>
                        onBrandChange(checked ? brand.slug : "")
                      }
                    />
                    <Label htmlFor={brand.id.toString()}>{brand.name}</Label>
                  </div>
                ))}
              </AccordionContent>
            </AccordionItem>

            {/* price */}
            <AccordionItem value="price">
              <AccordionTrigger>Price</AccordionTrigger>
              <AccordionContent>
                <div className="flex justify-center gap-2 pb-4 ">
                  <input
                    type="number"
                    value={priceRange[0]}
                    onChange={(e) => {
                      const v = +e.target.value;
                      setPriceRange([v, priceRange[1]]);
                      onMinPriceChange(v);
                    }}
                    className="w-20 border rounded-md px-2 py-1 text-lg text-center"
                    placeholder="min"
                  />
                  <input
                    type="number"
                    value={priceRange[1]}
                    onChange={(e) => {
                      const v = +e.target.value;
                      setPriceRange([priceRange[0], v]);
                      onMaxPriceChange(v);
                    }}
                    className="w-20 border rounded-md px-2 py-1 text-lg text-center"
                    placeholder="max"
                  />
                </div>

                <Slider
                  min={0}
                  max={10000000000}
                  step={100}
                  value={priceRange}
                  onValueChange={(v) => {
                    setPriceRange(v as [number, number]);
                    onMinPriceChange(v[0]);
                    onMaxPriceChange(v[1]);
                  }}
                  className="[&>[data-orientation=horizontal]]:bg-yellow-200"
                />
              </AccordionContent>
            </AccordionItem>

            {/* dynamic attributes */}
            {attributes.map((attr) => (
              <AccordionItem key={attr.name} value={attr.name}>
                <AccordionTrigger>{attr.name}</AccordionTrigger>
                <AccordionContent>
                  <div className="flex flex-col gap-2">
                    {attr.value.map((val) => (
                      <div key={val} className="flex items-center gap-2">
                        <Checkbox
                          id={`${attr.name}-${val}`}
                          checked={(
                            selectedAttributes[attr.name] || []
                          ).includes(val)}
                          onCheckedChange={(checked) =>
                            toggleAttributeValue(
                              attr.name,
                              val,
                              Boolean(checked)
                            )
                          }
                        />
                        <Label htmlFor={`${attr.name}-${val}`}>{val}</Label>
                      </div>
                    ))}
                  </div>
                </AccordionContent>
              </AccordionItem>
            ))}
          </Accordion>
        </div>
      </aside>
    </div>
  );
}
