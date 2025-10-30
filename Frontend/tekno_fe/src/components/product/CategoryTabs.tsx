"use client";

import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  Smartphone,
  Laptop,
  Tablet,
  Headphones,
  Watch,
  Camera,
  Gamepad2,
  Network,
  Sun,
} from "lucide-react";

export function CategoryTabs() {
  return (
    <Tabs defaultValue="laptop" className="w-full">
      <TabsList className="flex gap-6 bg-transparent border-b border-gray-200">
        {[
          { id: "mobile", label: "Mobile", icon: Smartphone },
          { id: "laptop", label: "Laptop", icon: Laptop },
          { id: "tablet", label: "Tablet", icon: Tablet },
          { id: "audio", label: "Audio", icon: Headphones },
          { id: "wearable", label: "Wearable", icon: Watch },
          { id: "camera", label: "Camera", icon: Camera },
          { id: "gaming", label: "Gaming", icon: Gamepad2 },
          { id: "network", label: "Network", icon: Network },
          { id: "accessories", label: "Accessories", icon: Sun },
        ].map(({ id, label, icon: Icon }) => (
          <TabsTrigger
            key={id}
            value={id}
            className="relative flex flex-col items-center gap-1 px-3 py-2 text-gray-600 data-[state=active]:text-cyan-600"
          >
            <Icon className="w-5 h-5" />
            <span className="text-xs font-medium">{label}</span>
            {/* Gạch highlight vàng bên dưới */}
            <span className="absolute bottom-0 left-0 w-full h-[3px] bg-yellow-400 scale-x-0 data-[state=active]:scale-x-100 transition-transform origin-center rounded-full" />
          </TabsTrigger>
        ))}
      </TabsList>
    </Tabs>
  );
}
