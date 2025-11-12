import Link from "next/link";
import React from "react";
import { Button } from "../ui/button";
import Image from "next/image";
import banner from "@/asset/MainLogo.png";

export default function HomeBanner() {
  return (
    <div className="py-16 md:py-0 bg-amber-100 px-10 lg:px-24 flex items-center justify-between">
      {/* title */}
      <div className="space-y-10">
        <div className="flex-1">
          <h2 className="text-4xl md:text-6xl font-bold text-primary capitalize tracking-wide md-10">
            Tekno
          </h2>
          <p className="text-2xl md:text-3xl font-normal text-primary md-10">
            "Join the <span className="text-secondary">digital revolution</span>
            "
          </p>
        </div>
        <Link
          href={"/products"}
          className="bg-secondary/90 rounded-lg text-white/90 px-15 py-4 text-md font-semibold
          hover:bg-secondary hover:text-white hoverEffect"
        >
          Explore more
        </Link>
      </div>
      {/* image */}
      <div>
        <Image
          src={banner}
          alt="banner"
          className="hidden md:inline-flex w-82"
        />
      </div>
    </div>
  );
}
