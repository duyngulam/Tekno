import { Category } from "@/type/categories";

export async function getCategoriesList(): Promise<Category[]> {
  try {
    const res = await fetch("http://localhost:5000/api/categories/list", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      cache: "no-store", // optional: tránh cache khi SSR
    });

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message || "Lấy danh sách thất bại!");
    }

    // ⬇️ Trả về đúng kiểu Category[]
    const result = await res.json();

    // Trả về chỉ phần data là Category[]
    return result.data as Category[];

  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error;
  }
}
