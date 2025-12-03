const API_BASE_URL = "http://localhost:5000/api"; // đổi theo API thật của bạn

// services/products.ts
export async function getProductsList(params?: {
  page?: number;
  pageSize?: number;
  category?: string;
  sortBy?: string;
}) {
  try {
    const query = new URLSearchParams();

    if (params?.page) query.append("Page", String(params.page));
    if (params?.pageSize) query.append("PageSize", String(params.pageSize));
    if (params?.category) query.append("Category", params.category);
    if (params?.sortBy) query.append("SortBy", params.sortBy);

    const res = await fetch(
      `http://localhost:5000/api/products?${query.toString()}`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
        cache: "no-store",
      }
    );

    if (!res.ok) {
      throw new Error(`Failed to fetch product list: ${res.status}`);
    }

    const result = await res.json();

    if (!result.success || !result.data) {
      throw new Error(result.message || "Invalid API response");
    }

    return result.data;
  } catch (error) {
    console.error("Error in getProductsList:", error);
    throw error;
  }
}

import { Product } from "@/type/product"; 

// Trả về dữ liệu chi tiết sản phẩm đúng kiểu ProductDetail
export async function getProductDetail(slug: string): Promise<Product> {
  try {
    const res = await fetch(`${API_BASE_URL}/products/${slug}`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
      cache: "no-store",
    });

    if (!res.ok) {
      throw new Error(`Failed to fetch product detail: ${res.status}`);
    }

    const result = await res.json();

    if (!result.success || !result.data) {
      throw new Error(result.message || "Invalid API response");
    }

    return result.data as Product;
  } catch (error) {
    console.error("Error in getProductDetail:", error);
    throw error;
  }
}

export async function getProductsInCart() {
// try {
//     const res = await fetch(`${API_BASE_URL}/cart`, {
//       method: "GET",
//       headers: { "Content-Type": "application/json" },
//       cache: "no-store",
//     });

//     if (!res.ok) {
//       throw new Error(`Failed to fetch product in cart: ${res.status}`);
//     }

//     const result = await res.json();

//     if (!result.success || !result.data) {
//       throw new Error(result.message || "Invalid API response");
//     }

//     return result.data as Product[];
//   } catch (error) {
//     console.error("Error in getProductCart:", error);
//     throw error;
//   }
}

