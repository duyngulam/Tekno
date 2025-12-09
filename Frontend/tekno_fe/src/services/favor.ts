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
};
