import React from "react";
import { X } from "lucide-react";
import { Badge } from "@/components/ui/badge";

interface Props {
  filters: Record<string, string[]>;
  HandleRemoveFilter?: (attr: string, value: string) => void;
}

export default function FilterChips({ filters, HandleRemoveFilter }: Props) {
  const chips = Object.entries(filters).flatMap(([attr, vals]) =>
    vals.map((v) => ({
      key: `${attr}|${v}`,
      label: `${attr}: ${v}`,
      attr,
      value: v,
    }))
  );

  if (chips.length === 0) return null;

  return (
    <div className="flex flex-wrap gap-2">
      {chips.map((c) => (
        <Badge
          key={c.key}
          variant="outline"
          className="flex items-center gap-2 border-gray-300 text-gray-700 bg-gray-50 hover:bg-gray-100 rounded-md px-3 py-1"
        >
          {c.label}
          <X
            className="w-3.5 h-3.5 cursor-pointer text-gray-500 hover:text-red-500"
            onClick={() => HandleRemoveFilter?.(c.attr, c.value)}
          />
        </Badge>
      ))}
    </div>
  );
}
