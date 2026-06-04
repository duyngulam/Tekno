// src/services/products.ts
import { httpClient } from "@/lib/httpClient";
import { Product } from "@/type/product";

export type CreateProductVariantPayload = {
  productId: number;
  sku: string;
  price: number;
  stock: number;
  status?: string;
  attributes: Array<{
    id?: number;
    name?: string;
    value: string;
  }>;
};

export class ProductService {
  private static instance: ProductService | null = null;

  private constructor() {}

  public static getInstance(): ProductService {
    if (!ProductService.instance) {
      ProductService.instance = new ProductService();
    }
    return ProductService.instance;
  }

  public async getProductsList(params?: {
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
    const query = new URLSearchParams();
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

    const queryString = query.toString();
    const endpoint = `/products${queryString ? `?${queryString}` : ""}`;
    
    // getProductsList expects the actual paginated result wrapper, so we map it.
    // In httpClient.ts, we automatically return result.data if 'success' in result.
    // Let's ensure the response structure returns the pagination metadata.
    // Wait, the API response for list is { success, data: { data, totalRecords, ... } } or { success, data: Product[] }?
    // Let's check products/page.tsx:
    // `setproductsList(res.data);` and `setTotalRecords(res.totalRecords);`
    // Wait! This means getProductsList returns an object containing `data` and `totalRecords`.
    // If the API returns `{ success: true, data: { data: Product[], totalRecords: 100, ... } }`,
    // httpClient automatically returns `result.data`, which is the object `{ data, totalRecords, ... }`.
    // This perfectly matches what the component expects!
    return httpClient.get<any>(endpoint, { cache: "no-store" });
  }

  public async getProductDetail(slug: string): Promise<Product> {
    return httpClient.get<Product>(`/products/${slug}`, { cache: "no-store" });
  }

  public async getProductRecommendation(userId: number, k: number): Promise<Product[]> {
    const res = await httpClient.get<any>(`/recommend/cf/products/${userId}?k=${k}`, { cache: "no-store" });
    return res.recommendations as Product[];
  }

  // Admin product helpers
  public async getAdminProducts(params?: { pageSize?: number; page?: number }) {
    const query = new URLSearchParams();
    if (params?.pageSize) query.append("PageSize", String(params.pageSize));
    if (params?.page) query.append("Page", String(params.page));
    
    const queryString = query.toString();
    const endpoint = `/admin/products${queryString ? `?${queryString}` : ""}`;
    return httpClient.get<any>(endpoint, { cache: "no-store" });
  }

  public async getAdminProduct(slug: string) {
    return httpClient.get<any>(`/admin/products/${slug}`, { cache: "no-store" });
  }

  public async createAdminProduct(fd: FormData) {
    return httpClient.post<any>("/admin/products", fd);
  }

  public async updateAdminProduct(id: number | string, fd: FormData) {
    return httpClient.put<any>(`/admin/products/${id}`, fd);
  }

  public async deleteAdminProduct(id: number | string) {
    return httpClient.del<any>(`/admin/products/${id}`);
  }

  public async createProductVariant(payload: CreateProductVariantPayload) {
    return httpClient.post<any>("/admin/products/variants", payload);
  }

  public async deleteProductVariant(variantId: number | string) {
    return httpClient.del<any>(`/admin/products/variants/${variantId}`);
  }

  public async updateProductVariant(
    variantId: number | string,
    payload: Partial<CreateProductVariantPayload>
  ) {
    return httpClient.put<any>(`/admin/products/variants/${variantId}`, payload);
  }

  public async getProductsOnSale(params?: { count: number; categorySlug?: string }) {
    const query = new URLSearchParams();
    if (params?.count) query.append("count", String(params.count));
    if (params?.categorySlug) query.append("categorySlug", params.categorySlug);

    const queryString = query.toString();
    const endpoint = `/products/on-sale${queryString ? `?${queryString}` : ""}`;
    return httpClient.get<any>(endpoint, { cache: "no-store" });
  }
}

export const productService = ProductService.getInstance();

// Backward compatibility exports
export const getProductsList = (params?: any) => productService.getProductsList(params);
export const getProductDetail = (slug: string) => productService.getProductDetail(slug);
export const getProductRecommendation = (userId: number, k: number) => productService.getProductRecommendation(userId, k);
export const getAdminProducts = (params?: any) => productService.getAdminProducts(params);
export const getAdminProduct = (slug: string) => productService.getAdminProduct(slug);
export const createAdminProduct = (fd: FormData) => productService.createAdminProduct(fd);
export const updateAdminProduct = (id: number | string, fd: FormData) => productService.updateAdminProduct(id, fd);
export const deleteAdminProduct = (id: number | string) => productService.deleteAdminProduct(id);
export const createProductVariant = (payload: CreateProductVariantPayload) => productService.createProductVariant(payload);
export const deleteProductVariant = (variantId: number | string) => productService.deleteProductVariant(variantId);
export const updateProductVariant = (variantId: number | string, payload: Partial<CreateProductVariantPayload>) => productService.updateProductVariant(variantId, payload);
export const getProductsOnSale = (params?: any) => productService.getProductsOnSale(params);