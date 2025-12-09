"use client";
import { headerData } from "@/data/HeaderData";
import Link from "next/link";
import { usePathname } from "next/navigation";
import React from "react";

export default function HeaderMenu() {
  const pathname = usePathname();

  return (
    <div className="hidden md:inline-flex w-1/3 items-cent gap-7 text-sm capitalize font-normal text-gray-900">
      {headerData?.map((item) => {
        const isRoot = item.match === "/";

        const isActive = isRoot
          ? pathname === "/"
          : pathname.startsWith(item.match);

        return (
          <Link
            key={item.label}
            href={item.href}
            className={`hover:text-primary hoverEffect relative group ${
              isActive && "text-primary"
            }`}
          >
            {item.label}

            {/* left bar */}
            <span
              className={`absolute -bottom-0.5 left-1/2 w-0 h-0.5 bg-primary
                group-hover:w-1/2 group-hover:left-0 hoverEffect
                ${isActive && "w-1/2 left-0"}
              `}
            />

            {/* right bar */}
            <span
              className={`absolute -bottom-0.5 right-1/2 w-0 h-0.5 bg-primary
                group-hover:w-1/2 group-hover:right-0 hoverEffect
                ${isActive && "w-1/2 right-0"}
              `}
            />
          </Link>
        );
      })}
    </div>
  );
}
