import SearchModal from "@/components/landing/SearchModal";
import { Search } from "lucide-react";
import React, { useState } from "react";

export default function () {
  const [open, setOpen] = useState(false);
  return (
    <div>
      <Search
        className="w-5 h-5 hover:text-primary hoverEffect"
        onClick={() => setOpen(true)}
      />
      <SearchModal open={open} onClose={() => setOpen(false)} />
    </div>
  );
}
