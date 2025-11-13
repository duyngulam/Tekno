import { AlignLeft } from "lucide-react";
import React from "react";

export default function MobileMenu() {
  return (
    <>
      <button>
        <AlignLeft className="md:hidden hover:text-primary hoverEffect" />
      </button>
    </>
  );
}
