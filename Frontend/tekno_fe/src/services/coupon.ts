import { API_BASE_URL } from "@/lib/apiConfig";
import type { CouponResponse } from "@/type/coupon";
import { ApiResponse } from "@/type/share";

export async function getCoupons(
  search?: string,
  page: number = 1,
  pageSize: number = 20
): Promise<ApiResponse<CouponResponse>> {
  // build query params, only append search if not empty
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  });
  const q = (search ?? "").trim();
  if (q.length > 0) params.append("search", q);

  const res = await fetch(`${API_BASE_URL}/coupons?${params.toString()}`, {
    method: "GET",
    headers: { "Content-Type": "application/json" },
    cache: "no-store",
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err?.message || "Failed to fetch coupons");
  }

  return res.json();
}