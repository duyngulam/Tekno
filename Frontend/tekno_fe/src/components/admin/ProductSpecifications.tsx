"use client";

import { useEffect, useState } from "react";

type ProductSpecificationsProps = {
  productId?: number;
  categoryId?: number;
  initialSpecs?: ProductSpec[];
  onChange: (specs: ProductSpec[]) => void;
};

type ProductSpec = {
  attributeId: number;
  attributeName: string;
  values: string[];
};

export default function ProductSpecifications({
  productId,
  categoryId,
  initialSpecs = [],
  onChange,
}: ProductSpecificationsProps) {
  const [specifications, setSpecifications] = useState<ProductSpec[]>(initialSpecs);

  // Sync initial specs
  useEffect(() => {
    setSpecifications(initialSpecs);
  }, [initialSpecs]);

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h3 className="font-semibold text-lg">Product Specifications</h3>
      </div>

      {/* Specifications List - Read Only */}
      {specifications.length === 0 ? (
        <div className="p-8 border rounded bg-gray-50 text-center text-gray-500">
          No specifications.
        </div>
      ) : (
        <div className="space-y-4">
          {specifications.map((spec, idx) => (
            <div key={`spec-${idx}`} className="border rounded p-4 bg-white">
              <h4 className="font-semibold text-base mb-3">{spec.attributeName}</h4>
              
              <div>
                <label className="text-xs text-gray-600 block mb-2">Values:</label>
                {spec.values.length === 0 ? (
                  <p className="text-xs text-gray-400 text-center py-2">No values</p>
                ) : (
                  <div className="flex flex-wrap gap-2">
                    {spec.values.map((value, idx) => (
                      <div key={idx} className="px-3 py-1 bg-blue-50 border border-blue-200 rounded-full text-sm">
                        {value}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}