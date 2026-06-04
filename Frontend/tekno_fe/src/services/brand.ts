// src/services/brand.ts
import { httpClient } from "@/lib/httpClient";

export class BrandService {
  private static instance: BrandService | null = null;

  private constructor() {}

  public static getInstance(): BrandService {
    if (!BrandService.instance) {
      BrandService.instance = new BrandService();
    }
    return BrandService.instance;
  }

  public async getBrandList() {
    return httpClient.get<any>("/admin/brands/list", { cache: "no-store" });
  }

  public async createBrand(fd: FormData) {
    return httpClient.post<any>("/admin/brands/create", fd);
  }

  public async updateBrand(fd: FormData) {
    return httpClient.put<any>("/admin/brands/update", fd);
  }

  public async deleteBrand(id: string) {
    return httpClient.del<any>("/admin/brands/delete", {
      body: JSON.stringify({ Id: id }),
    });
  }
}

export const brandService = BrandService.getInstance();

// Backward compatibility exports
export const getBrandList = () => brandService.getBrandList();
export const createBrand = (fd: FormData) => brandService.createBrand(fd);
export const updateBrand = (fd: FormData) => brandService.updateBrand(fd);
export const deleteBrand = (id: string) => brandService.deleteBrand(id);