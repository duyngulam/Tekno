import React from "react";

export default function FormattedPrice({
  price,
  className = "",
}: {
  price: number;
  className?: string;
}) {
  const formatted = Number(price || 0).toLocaleString("vi-VN");

  return <span className={className}>{formatted}đ</span>;
}
