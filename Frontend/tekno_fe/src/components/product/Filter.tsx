"use client";

import { useState } from "react";

const FilterSidebar = () => {
  const [openSection, setOpenSection] = useState<string | null>(null);
  const [discount, setDiscount] = useState(true);
  const [price, setPrice] = useState({ min: "", max: "" });
  const [selected, setSelected] = useState<{ [key: string]: string[] }>({
    brand: [],
    ram: [],
    screen: [],
    processor: [],
    gpu: [],
    drive: [],
  });

  const toggleOption = (key: string, value: string) => {
    setSelected((prev) => {
      const arr = prev[key] || [];
      return {
        ...prev,
        [key]: arr.includes(value)
          ? arr.filter((v) => v !== value)
          : [...arr, value],
      };
    });
  };

  const toggleSection = (name: string) => {
    setOpenSection(openSection === name ? null : name);
  };

  const sections = [
    {
      title: "Brand",
      key: "brand",
      options: ["Asus", "Acer", "Apple", "Dell"],
    },
    {
      title: "RAM",
      key: "ram",
      options: ["32 GB", "16 GB", "12 GB", "8 GB"],
    },
    {
      title: "Screen Size",
      key: "screen",
      options: ['13" - 13.9"', '14" - 14.9"', '15" - 15.9"', '16" - 16.9"'],
    },
    {
      title: "Processor",
      key: "processor",
      options: [
        "Intel Core i5",
        "Intel Core i7",
        "Intel Core i9",
        "AMD Ryzen 9",
      ],
    },
    {
      title: "GPU Brand",
      key: "gpu",
      options: ["NVIDIA", "Intel", "AMD", "Apple"],
    },
    {
      title: "Drive Size",
      key: "drive",
      options: ["512GB", "256GB", "64GB", "128GB"],
    },
  ];

  return (
    <aside className="w-64 bg-white border rounded-2xl p-4 shadow-sm space-y-4">
      {/* Header */}
      <div className="flex items-center justify-between">
        <h2 className="font-semibold text-gray-700">Filters</h2>
        <button
          onClick={() => {
            setSelected({
              brand: [],
              ram: [],
              screen: [],
              processor: [],
              gpu: [],
              drive: [],
            });
            setPrice({ min: "", max: "" });
            setDiscount(false);
          }}
          className="text-sm text-yellow-500 hover:underline"
        >
          Clear all
        </button>
      </div>

      {/* Discount Toggle */}
      <div className="flex items-center justify-between border-t pt-3">
        <span className="text-gray-700 font-medium">Discount</span>
        <label className="relative inline-flex items-center cursor-pointer">
          <input
            type="checkbox"
            checked={discount}
            onChange={(e) => setDiscount(e.target.checked)}
            className="sr-only peer"
          />
          <div className="w-11 h-6 bg-gray-300 peer-focus:outline-none rounded-full peer peer-checked:bg-yellow-400 transition-colors"></div>
          <div className="absolute left-1 top-1 bg-white w-4 h-4 rounded-full transition-transform peer-checked:translate-x-5"></div>
        </label>
      </div>

      {/* Price Section */}
      <div className="border-t pt-3">
        <div
          className="flex items-center justify-between cursor-pointer"
          onClick={() => toggleSection("price")}
        >
          <span className="font-medium text-gray-700">Price</span>
          {/* {openSection === "price" ? (
            <ChevronUp size={16} />
          ) : (
            <ChevronDown size={16} />
          )} */}
        </div>
        {openSection === "price" && (
          <div className="pt-3 flex space-x-2">
            <input
              type="number"
              placeholder="min"
              value={price.min}
              onChange={(e) => setPrice({ ...price, min: e.target.value })}
              className="w-20 border rounded-md p-1 text-sm focus:outline-none focus:ring-1 focus:ring-yellow-400"
            />
            <input
              type="number"
              placeholder="max"
              value={price.max}
              onChange={(e) => setPrice({ ...price, max: e.target.value })}
              className="w-20 border rounded-md p-1 text-sm focus:outline-none focus:ring-1 focus:ring-yellow-400"
            />
          </div>
        )}
      </div>

      {/* Dynamic Filter Sections */}
      {sections.map((sec) => (
        <div key={sec.key} className="border-t pt-3">
          <div
            className="flex items-center justify-between cursor-pointer"
            onClick={() => toggleSection(sec.key)}
          >
            <span className="font-medium text-gray-700">{sec.title}</span>
            {/* {openSection === sec.key ? (
              <ChevronUp size={16} />
            ) : (
              <ChevronDown size={16} />
            )} */}
          </div>
          {openSection === sec.key && (
            <div className="pt-2 space-y-1">
              {sec.options.map((opt) => (
                <label
                  key={opt}
                  className="flex items-center space-x-2 cursor-pointer"
                >
                  <input
                    type="checkbox"
                    className="accent-yellow-400"
                    checked={selected[sec.key]?.includes(opt)}
                    onChange={() => toggleOption(sec.key, opt)}
                  />
                  <span className="text-sm text-gray-600">{opt}</span>
                </label>
              ))}
            </div>
          )}
        </div>
      ))}
    </aside>
  );
};

export default FilterSidebar;
