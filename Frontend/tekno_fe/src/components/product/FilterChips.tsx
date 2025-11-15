import { X } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { useState } from "react";

interface FilterChipsProps {
  filters: string[];
  HandleRemoveFilter?: (f: string) => void;
}

export function FilterChips({ filters, HandleRemoveFilter }: FilterChipsProps) {
  // const [filters, setFilters] = useState([
  //   "Silver",
  //   "Intel Core i9",
  //   "Apple",
  //   "12 GB",
  // ]);

  // const HandleRemoveFilter = (f: string) => {
  //   setFilters((prev) => prev.filter((item) => item !== f));
  // };

  return (
    <div className="flex flex-wrap gap-2">
      {filters.map((f) => (
        <Badge
          key={f}
          variant="outline"
          className="flex items-center gap-2 border-gray-300 text-gray-700 bg-gray-50 hover:bg-gray-100 rounded-md px-3 py-1"
        >
          {f}
          <X
            className="w-3.5 h-3.5 cursor-pointer text-gray-500 hover:text-red-500"
            onClick={() => HandleRemoveFilter?.(f)}
          />
        </Badge>
      ))}
    </div>
  );
}
