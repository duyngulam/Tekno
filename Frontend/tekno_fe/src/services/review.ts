import { API_BASE_URL } from "@/lib/apiConfig";
import { ProductReviewsResponse } from "@/type/review";
import { ApiResponse } from "@/type/share";


interface GetProductReviewsParams {
  productId: number;
  page?: number;
  pageSize?: number;
  verifiedOnly?: boolean;
}

export async function getProductReviews({
  productId,
  page = 1,
  pageSize = 20,
  verifiedOnly,
}: GetProductReviewsParams): Promise<ApiResponse<ProductReviewsResponse>> {
  const params = new URLSearchParams();

  params.append("page", page.toString());
  params.append("pageSize", pageSize.toString());

  if (verifiedOnly !== undefined) {
    params.append("verifiedOnly", String(verifiedOnly));
  }

  const res = await fetch(
    `${API_BASE_URL}/products/${productId}/reviews?${params.toString()}`,
    {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      cache: "no-store", // nếu muốn luôn fresh
    }
  );

  if (!res.ok) {
    throw new Error("Failed to fetch product reviews");
  }

  return res.json();
}
