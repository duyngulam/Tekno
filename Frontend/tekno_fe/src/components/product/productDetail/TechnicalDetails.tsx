"use client";
import React, { useState } from "react";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
  TableFooter,
} from "@/components/ui/table";
export default function TechnicalDetails({
  specs,
}: {
  specs: { name: string; value: string[] }[];
}) {
  const [showAll, setShowAll] = useState(false);
  const visibleSpecs = showAll ? specs : specs.slice(0, 5);

  return (
    <div className="flex flex-col gap-4">
      <div className="font-bold text-xl">Technical Details</div>
      <Table>
        {/* Chỉ hiện nút Show more nếu có hơn 5 dòng */}
        {specs.length > 5 && (
          <TableCaption>
            {!showAll ? (
              <button
                onClick={() => setShowAll(true)}
                className="text-blue-600 hover:underline"
              >
                Show more
              </button>
            ) : (
              <button
                onClick={() => setShowAll(false)}
                className="text-blue-600 hover:underline"
              >
                Show less
              </button>
            )}
          </TableCaption>
        )}

        <TableBody className="[&_tr:nth-child(odd)]:bg-white [&_tr:nth-child(even)]:bg-gray-50">
          {visibleSpecs.map((spec, index) => (
            <TableRow key={index}>
              {/* Cột 1: tên spec + màu xám + dấu chấm đầu dòng */}
              <TableCell className="w-2/6 p-2">{spec.name}</TableCell>

              {/* Cột 2: value (array) → join thành chuỗi */}
              <TableCell className="w-4/6">
                {Array.isArray(spec.value) ? spec.value.join(", ") : spec.value}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
