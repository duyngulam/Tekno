import { AlignLeft } from "lucide-react";
import React from "react";

export default function MobileMenu() {
  return (
    <>
      <button>
        <AlignLeft className="md:hidden md:gap-0 hover:text-primary hoverEffect" />
      </button>
    </>
  );
}
