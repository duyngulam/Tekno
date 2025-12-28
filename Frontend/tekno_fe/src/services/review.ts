import { API_BASE_URL } from "@/lib/apiConfig";
import { CanReviewData, ProductReviewsResponse, SubmitReviewPayload, SubmitReviewResponse } from "@/type/review";
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

// POST /api/products/{productId}/reviews
export async function createReview(
  token: string,
  productId: number,
  payload: SubmitReviewPayload
): Promise<ApiResponse<SubmitReviewResponse>> {
  const res = await fetch(
    `${API_BASE_URL}/products/${productId}/reviews`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    }
  );

  const json = await res.json().catch(() => ({}));

  if (!res.ok) {
    throw json;
  }

  return json as ApiResponse<SubmitReviewResponse>;
}

// PUT /api/products/{productId}/reviews/{reviewId}
export async function updateReview(
  token: string,
  productId: number,
  reviewId: number,
  payload: SubmitReviewPayload
): Promise<ApiResponse<SubmitReviewResponse>> {
  const res = await fetch(
    `${API_BASE_URL}/products/${productId}/reviews/${reviewId}`,
    {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    }
  );

  const json = await res.json().catch(() => ({}));

  if (!res.ok) {
    throw json;
  }

  return json as ApiResponse<SubmitReviewResponse>;
}
// DELETE /api/products/{productId}/reviews/{reviewId}
export async function deleteReview(
  token: string,
  productId: number,
  reviewId: number
): Promise<ApiResponse<boolean>> {
  const res = await fetch(
    `${API_BASE_URL}/products/${productId}/reviews/${reviewId}`,
    {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    }
  );

  const json = await res.json().catch(() => ({}));

  if (!res.ok) {
    throw json;
  }

  return json as ApiResponse<boolean>;
}

// GET /api/products/{productId}/reviews/can-review
export async function canReviewProduct(
  token: string,
  productId: number
): Promise<ApiResponse<CanReviewData>> {
  const res = await fetch(
    `${API_BASE_URL}/products/${productId}/reviews/can-review`,
    {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    }
  );

  const json = await res.json().catch(() => ({}));

  if (!res.ok) {
    throw json;
  }

  return json as ApiResponse<CanReviewData>;
}
