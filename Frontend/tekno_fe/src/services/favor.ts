import { Product } from "@/type/product";

const BASE_URL = "http://localhost:5000/api";

// FAVORITE API
export const favorApi = {
  getFavor: async (token: string): Promise<Product[]> => {
    const res = await fetch(`${BASE_URL}/wishlist`, { credentials: "include", headers: {
          "Authorization": `Bearer ${token}`,
        },
       });
    if (!res.ok) throw new Error("Failed to fetch favorites");
    return res.json();
  },

  addToFavor: async (
    token: string,
     variantId :  number
  ) => {
    const res = await fetch(`${BASE_URL}/wishlist/items/${variantId}`, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`,
      },
      body: JSON.stringify( variantId ),
    });

    if (!res.ok) throw new Error("Failed to add to favor");
    return res.json();
  },

  removeFavor: async (token: string, variantId: number) => {
    const res = await fetch(`${BASE_URL}/cart/items/${variantId}`, {
      method: "DELETE",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`,
      },
      body: JSON.stringify({ variantId }),
    });

    if (!res.ok) throw new Error("Failed to remove from favor");
    return res.json();
  },
};
