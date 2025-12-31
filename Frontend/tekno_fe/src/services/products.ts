import { get,post, postForm, put, del, API_BASE } from "@/lib/api";
import { API_BASE_URL } from "@/lib/apiConfig";


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
import { ApiResponse } from "@/type/share";

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

export async function getProductRecommendation(userId: number, k: number): Promise<Product[]> {
  try {
    const res = await fetch(`${API_BASE_URL}/recommend/cf/products/${userId}?k=${k}`, {
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

    return result.data.recommendations as Product[];
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

// Admin product helpers
export async function getAdminProducts(params?: {
  pageSize?: number;
  page?: number;
} ) {
  try {
    const query = new URLSearchParams();
    if (params?.pageSize) query.append("PageSize", String(params.pageSize));
    if (params?.page) query.append("Page", String(params.page));
    
    const url = `${API_BASE}/admin/products${query.toString() ? `?${query.toString()}` : ""}`;
    return await get(url, { cache: "no-store" });
  } catch (error) {
    console.error("❌ Failed to load admin products:", error);
    throw error;
  }
}

export async function getAdminProduct(slug: string) {
  try {
    return await get(`${API_BASE}/admin/products/${slug}`, { cache: "no-store" });
  } catch (error) {
    console.error("❌ Failed to load admin product:", error);
    throw error;
  }
}

export async function createAdminProduct(fd: FormData) {
  try {
    return await postForm(`${API_BASE}/admin/products`, fd);
  } catch (error) {
    console.error("❌ Failed to create admin product:", error);
    throw error;
  }
}

export async function updateAdminProduct(id: number | string, fd: FormData) {
  try {
    return await put(`${API_BASE}/admin/products/${id}`, fd);
  } catch (error) {
    console.error("❌ Failed to update admin product:", error);
    throw error;
  }
}

export async function deleteAdminProduct(id: number | string) {
  try {
    return await del(`${API_BASE}/admin/products/${id}`);
  } catch (error) {
    console.error("❌ Failed to delete admin product:", error);
    throw error;
  }
}

export type CreateProductVariantPayload = {
  productId: number;
  sku: string;
  price: number;
  stock: number;
  status?: string;
  attributes: Array<{  // ✅ Changed from attributeValues to attributes
    id?: number;        // ✅ Use 'id' for existing attributes
    name?: string;      // ✅ Use 'name' for new attributes  
    value: string;
  }>;
};

export async function createProductVariant(
  payload: CreateProductVariantPayload
) {
  try {
    return await post(
      `${API_BASE}/admin/products/variants`,
      payload
    );
  } catch (error) {
    console.error("❌ Failed to create product variant:", error);
    throw error;
  }
}

export async function deleteProductVariant(variantId: number | string) {
  try {
    return await del(`${API_BASE}/admin/products/variants/${variantId}`);
  } catch (error) {
    console.error("❌ Failed to delete product variant:", error);
    throw error;
  }
}

export async function updateProductVariant(
  variantId: number | string,
  payload: Partial<CreateProductVariantPayload>
) {
  try {
    return await put(
      `${API_BASE}/admin/products/variants/${variantId}`,
      payload
    );
  } catch (error) {
    console.error("❌ Failed to update product variant:", error);
    throw error;
  }
}

//product on sale
export async function getProductsOnSale(params?: {
  count: number;
  categorySlug?: string;
}) {
  try {
    const query = new URLSearchParams();

    // API expects PascalCase parameter names according to Swagger UI
    if (params?.count) query.append("count", String(params.count));
    if (params?.categorySlug) query.append("categorySlug", params.categorySlug);
    

    const url = `${API_BASE_URL}/products/on-sale${query.toString() ? `?${query.toString()}` : ""}`;

    const res = await fetch(url, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
      cache: "no-store",
    });

    if (!res.ok) {
      throw new Error(`Failed to fetch product on sale list: ${res.status}`);
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