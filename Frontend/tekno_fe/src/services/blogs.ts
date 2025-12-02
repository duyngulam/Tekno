import { Blog } from "@/type/blog";


export async function getBlogsRecent(): Promise<Blog[]> {
  try {
    const res = await fetch("http://localhost:5000/api/blog/recent?count=2", {
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
    return result.data as Blog[];

  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error;
  }
}

export async function getBlogsList() {
  try {
    const res = await fetch("http://localhost:5000/api/blog", {
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

      console.log("fetch blog list", res);
      
    return await res.json();

  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error;
  }
}

export async function getBlogDetail(slug: string): Promise<Blog> {
  try {
    const res = await fetch(`http://localhost:5000/api/blog/${slug}`, {
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

    return result.data as Blog;
  } catch (error) {
    console.error("Error in getProductDetail:", error);
    throw error;
  }
}