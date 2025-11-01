import { X } from "lucide-react";
import { Badge } from "@/components/ui/badge";

export function FilterChips() {
  const filters = ["Silver", "Intel Core i9", "Apple", "12 GB"];

  return (
    <div className="flex flex-wrap gap-2">
      {filters.map((f) => (
        <Badge
          key={f}
          variant="outline"
          className="flex items-center gap-2 border-gray-300 text-gray-700 bg-gray-50 hover:bg-gray-100 rounded-full px-3 py-1"
        >
          {f}
          <X className="w-3.5 h-3.5 cursor-pointer text-gray-500 hover:text-red-500" />
        </Badge>
      ))}
    </div>
  );
}
