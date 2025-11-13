import { Phone } from "lucide-react";
import React from "react";

interface ServiceData {
  text: string;
  icon: React.ReactNode;
}

const data: ServiceData[] = [
  {
    text: "Latest and Greatest Tech",
    icon: (
      <Phone className="h-6 w-6 text-gray-700 group-hover:text-primary transition-color" />
    ),
  },
  {
    text: "Guarantee",
    icon: (
      <Phone className="h-6 w-6 text-gray-700 group-hover:text-primary transition-color" />
    ),
  },
  {
    text: "Free Shipping over 1000$",
    icon: (
      <Phone className="h-6 w-6 text-gray-700 group-hover:text-primary transition-color" />
    ),
  },
  {
    text: "24/7 Support",
    icon: (
      <Phone className="h-6 w-6 text-gray-700 group-hover:text-primary transition-color" />
    ),
  },
];
export default function FooterTop() {
  return (
    <div className="grid grid-cols-2 md:grid-cols-4 gap-8 bg-white">
      {data?.map((item, index) => (
        <div
          key={index}
          className="flex items-center gap-3 group hover:bg-gray-50 py-5 transition-color"
        >
          <div>{item.icon}</div>
          {item?.text}
        </div>
      ))}
    </div>
  );
}
