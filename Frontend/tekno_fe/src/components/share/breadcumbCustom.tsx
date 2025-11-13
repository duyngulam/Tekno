"use client";

import { usePathname } from "next/navigation";
import Link from "next/link";
import { ChevronRight } from "lucide-react";

export function Breadcrumb() {
  const pathname = usePathname(); // ví dụ: "/products/laptop/macbook-pro"
  const segments = pathname.split("/").filter(Boolean); // ["products", "laptop", "macbook-pro"]

  // Tạo danh sách breadcrumb
  const breadcrumbItems = segments.map((segment, index) => {
    const href = "/" + segments.slice(0, index + 1).join("/");
    return {
      label: decodeURIComponent(segment.replace(/-/g, " ")), // đổi "macbook-pro" → "macbook pro"
      href,
    };
  });

  return (
    <nav className="flex items-center text-sm text-gray-500 space-x-1">
      {/* Trang chủ */}
      <Link href="/" className="hover:text-blue-600 transition">
        Home
      </Link>

      {breadcrumbItems.map((item, index) => (
        <div key={item.href} className="flex items-center space-x-1">
          <ChevronRight className="w-4 h-4 text-gray-400" />
          {index === breadcrumbItems.length - 1 ? (
            <span className="text-gray-800 font-semibold capitalize">
              {item.label}
            </span>
          ) : (
            <Link
              href={item.href}
              className="hover:text-blue-600 transition capitalize"
            >
              {item.label}
            </Link>
          )}
        </div>
      ))}
    </nav>
  );
}
