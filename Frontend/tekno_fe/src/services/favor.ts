// src/services/favor.ts
import { httpClient } from "@/lib/httpClient";
import { Product } from "@/type/product";

export class FavorService {
  private static instance: FavorService | null = null;

  private constructor() {}

  public static getInstance(): FavorService {
    if (!FavorService.instance) {
      FavorService.instance = new FavorService();
    }
    return FavorService.instance;
  }

  public async getFavor(): Promise<Product[]> {
    return httpClient.get<Product[]>("/wishlist", { credentials: "include" });
  }

  public async addToFavor(productId: number) {
    return httpClient.post<any>(
      "/wishlist/items",
      { productId },
      { credentials: "include" }
    );
  }

  public async removeFavor(productId: number) {
    return httpClient.del<any>(`/wishlist/items/${productId}`, {
      body: JSON.stringify({ productId }),
      credentials: "include",
    });
  }

  public async checkFavor(productId: number) {
    return httpClient.get<any>(`/wishlist/check/${productId}`, {
      credentials: "include",
    });
  }
}

export const favorService = FavorService.getInstance();

// Backward compatibility favorApi object
export const favorApi = {
  getFavor: async (token?: string) => favorService.getFavor(),
  addToFavor: async (token: string | number, productId?: number) => {
    if (typeof token === "number") {
      return favorService.addToFavor(token);
    }
    return favorService.addToFavor(productId!);
  },
  removeFavor: async (token: string | number, productId?: number) => {
    if (typeof token === "number") {
      return favorService.removeFavor(token);
    }
    return favorService.removeFavor(productId!);
  },
  checkFavor: async (token: string | number, productId?: number) => {
    if (typeof token === "number") {
      return favorService.checkFavor(token);
    }
    return favorService.checkFavor(productId!);
  },
};
