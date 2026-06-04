// src/services/cart.ts
import { httpClient } from "@/lib/httpClient";
import { CartResponse } from "@/hook/useCart";

export class CartService {
  private static instance: CartService | null = null;

  private constructor() {}

  public static getInstance(): CartService {
    if (!CartService.instance) {
      CartService.instance = new CartService();
    }
    return CartService.instance;
  }

  public async getCart(): Promise<CartResponse> {
    // Explicitly casting response to CartResponse
    return httpClient.get<CartResponse>("/cart", { credentials: "include" });
  }

  public async addToCart(variantId: number, quantity: number) {
    return httpClient.post<any>(
      "/cart/items",
      { variantId, quantity },
      { credentials: "include" }
    );
  }

  public async removeFromCart(variantId: number) {
    // Note: del requires sending payload in options if needed, but endpoint might only need url parameter
    return httpClient.del<any>(`/cart/items/${variantId}`, {
      body: JSON.stringify({ variantId }),
      credentials: "include",
    });
  }

  public async cleanCart(): Promise<CartResponse> {
    return httpClient.del<CartResponse>("/cart", { credentials: "include" });
  }

  public async updateQuantity(
    variantId: number,
    quantity: number
  ): Promise<CartResponse> {
    return httpClient.put<CartResponse>(
      `/cart/items/${variantId}`,
      { quantity },
      { credentials: "include" }
    );
  }
}

export const cartService = CartService.getInstance();
