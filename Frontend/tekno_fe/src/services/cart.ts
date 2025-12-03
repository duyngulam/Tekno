import { Product } from "@/type/product";

const BASE_URL = "http://localhost:5000/api";
const token = localStorage.getItem('token');

// CART API
export const cartApi = {
  getCart: async (): Promise<{ product: Product; quantity: number }[]> => {
    const res = await fetch(`${BASE_URL}/cart`, { credentials: "include" });
    if (!res.ok) throw new Error("Failed to fetch cart");
    return res.json();
  },

  addToCart: async({variantId, quantity}: {variantId: number; quantity: number}) => {
    const res = await fetch(`${BASE_URL}/cart/items`, {
      method: "POST",
      headers: {
    "Content-Type": "application/json",
    "Authorization": `Bearer ${token}`, // <-- gắn JWT
  },
      credentials: "include",
      body: JSON.stringify({
  "variantId": variantId,
  "quantity": quantity
}),
    });
    if (!res.ok) throw new Error("Failed to add to cart");
    return res.json();
  },

  removeFromCart: async (variantId : number) => {
    const res = await fetch(`${BASE_URL}/cart/items/${variantId}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ variantId  }),
    });
    if (!res.ok) throw new Error("Failed to remove from cart");
    return res.json();
  },
};
