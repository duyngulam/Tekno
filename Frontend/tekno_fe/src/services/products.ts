const API_BASE_URL = "http://localhost:5000/api"; // đổi theo API thật của bạn

// services/products.ts
export async function getProductsList(page:number, pageSize: number, selectedCategory: string, sortBy: string) {
try {
    const res = await fetch(`http://localhost:5000/api/products?Page=${page}&PageSize=${pageSize}&Category=${selectedCategory} `, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
      cache: "no-store",
    });

    if (!res.ok) {
      throw new Error(`Failed to fetch product detail: ${res.status}`);
    }

    const result = await res.json();

    // Giả sử API luôn trả về cấu trúc { success, message, data, errors, timestamp }
    if (!result.success || !result.data) {
      throw new Error(result.message || "Invalid API response");
    }

    return result.data;
  } catch (error) {
    console.error("Error in getProductDetail:", error);
    throw error;
  }
 }


import { Product, ProductDetail } from "@/type/product"; 



// Trả về dữ liệu chi tiết sản phẩm đúng kiểu ProductDetail
export async function getProductDetail(slug: string): Promise<ProductDetail> {
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

    // Giả sử API luôn trả về cấu trúc { success, message, data, errors, timestamp }
    if (!result.success || !result.data) {
      throw new Error(result.message || "Invalid API response");
    }

    return result.data as ProductDetail;
  } catch (error) {
    console.error("Error in getProductDetail:", error);
    throw error;
  }
}

