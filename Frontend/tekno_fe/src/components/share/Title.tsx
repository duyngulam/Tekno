import { cn } from "@/lib/utils";
import React from "react";

export default function Title({
  title,
  className,
}: {
  title: string;
  className?: string;
}) {
  return (
    <div className={cn("text-2xl font-bold mb-4", className)}>{title}</div>
  );
}
