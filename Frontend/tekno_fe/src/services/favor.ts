import { Product } from "@/type/product";

const BASE_URL = "https://yourserver.com/api";

// FAVORITE API
export const favorApi = {
  getFavor: async (): Promise<Product[]> => {
    const res = await fetch(`${BASE_URL}/favor`, { credentials: "include" });
    if (!res.ok) throw new Error("Failed to fetch favorites");
    return res.json();
  },

  addFavor: async (productId: number) => {
    const res = await fetch(`${BASE_URL}/favor/add`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ productId }),
    });
    if (!res.ok) throw new Error("Failed to add favorite");
    return res.json();
  },

  removeFavor: async (productId: number) => {
    const res = await fetch(`${BASE_URL}/favor/remove/${productId}`, {
      method: "DELETE",
      credentials: "include",
    });
    if (!res.ok) throw new Error("Failed to remove favorite");
    return res.json();
  },
};
