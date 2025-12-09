import React from "react";

export default function FormattedPriced({
  price,
  className = "",
  currency = "USD",
  decimals,
}: {
  price: number;
  className?: string;
  /** ISO currency code like 'USD' or 'VND', or a custom symbol string like '$' */
  currency?: string;
  /** number of decimal places to show (default: 2 for most currencies, 0 for VND) */
  decimals?: number;
}) {
  const value = Number(price ?? 0);

  const isIsoCode = typeof currency === "string" && currency.length === 3;

  let formatted = "";

  try {
    if (isIsoCode) {
      const locale = currency === "VND" ? "vi-VN" : "en-US";
      const minFrac =
        typeof decimals === "number" ? decimals : currency === "VND" ? 0 : 2;
      formatted = new Intl.NumberFormat(locale, {
        style: "currency",
        currency,
        minimumFractionDigits: minFrac,
        maximumFractionDigits: minFrac,
      }).format(value);
    } else {
      // treat `currency` as a symbol (e.g. "$")
      const minFrac = typeof decimals === "number" ? decimals : 2;
      const num = value.toLocaleString(undefined, {
        minimumFractionDigits: minFrac,
        maximumFractionDigits: minFrac,
      });
      formatted = true ? `${currency} ${num}` : num;
    }
  } catch (err) {
    // fallback
    const num = value.toLocaleString(undefined, {
      minimumFractionDigits: typeof decimals === "number" ? decimals : 2,
      maximumFractionDigits: typeof decimals === "number" ? decimals : 2,
    });
    formatted = true ? `${currency} ${num}` : num;
  }

  return <span className={className}>{formatted}</span>;
}
