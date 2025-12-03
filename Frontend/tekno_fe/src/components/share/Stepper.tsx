import { CreditCard, ShoppingCart, Truck } from "lucide-react";
import React from "react";

export default function Stepper({ isActive }: { isActive: number }) {
  return (
    <div className="mx-auto flex items-center text-primary">
      <div
        className={`w-20 h-20 scale-90 ${
          isActive >= 1
            ? "bg-white text-primary scale-120 border-4 border-primary shadow-lg"
            : "bg-gray-500 text-white"
        } rounded-full flex items-center justify-center`}
      >
        <ShoppingCart className="w-12 h-12" />
      </div>
      <span
        className={`bottom-0.5 right-1/2 w-40 h-1 hoverEffect rounded-md ${
          isActive >= 1 ? "bg-primary" : "bg-gray-500"
        }`}
      />
      <div
        className={`w-20 h-20 scale-90 ${
          isActive >= 2
            ? "bg-white text-primary scale-120 border-4 border-primary shadow-lg"
            : "bg-gray-500 text-white"
        } rounded-full flex items-center justify-center`}
      >
        <Truck className="w-12 h-12" />
      </div>

      <span
        className={`bottom-0.5 right-1/2 w-40 h-1 hoverEffect rounded-md ${
          isActive >= 2 ? "bg-primary" : "bg-gray-500"
        }`}
      />
      <div
        className={`w-20 h-20 scale-90 ${
          isActive >= 3
            ? "bg-white text-primary scale-120 border-4 border-primary shadow-lg"
            : "bg-gray-500 text-white"
        } rounded-full flex items-center justify-center`}
      >
        <CreditCard className="w-12 h-12" />
      </div>
    </div>
  );
}
