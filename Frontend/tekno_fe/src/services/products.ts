const API_BASE_URL = "http://localhost:5000/api"; // đổi theo API thật của bạn

// services/products.ts
export async function getProductsList(params?: {
  page?: number;
  pageSize?: number;
  category?: string;
  sortBy?: string;
  keyword?: string;
  brand?: string;
  minPrice?: number;
  maxPrice?: number;
  filters?: Record<string, string[]>;
  suggest?: boolean;
}) {
  try {
    const query = new URLSearchParams();

    // API expects PascalCase parameter names according to Swagger UI
    if (params?.keyword) query.append("Keyword", params.keyword);
    if (params?.category) query.append("Category", params.category);
    if (params?.brand) query.append("Brand", params.brand);
    if (params?.sortBy) query.append("Sort", params.sortBy);
    if (typeof params?.minPrice !== "undefined") query.append("MinPrice", String(params.minPrice));
    if (typeof params?.maxPrice !== "undefined") query.append("MaxPrice", String(params.maxPrice));
    if (params?.filters) query.append("Filters", JSON.stringify(params.filters));
    if (typeof params?.suggest !== "undefined") query.append("Suggest", String(Boolean(params.suggest)));
    if (params?.page) query.append("Page", String(params.page));
    if (params?.pageSize) query.append("PageSize", String(params.pageSize));
    console.log("filter query",params?.filters);
    

    const url = `${API_BASE_URL}/products${query.toString() ? `?${query.toString()}` : ""}`;

    const res = await fetch(url, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
      cache: "no-store",
    });

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

