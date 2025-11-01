
export async function getCategoriesList() {
  try {
    const res = await fetch("http://localhost:5000/api/categories/list", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message || "Lấy danh sách thất bại!");
    }

    return await res.json();
  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error; // để component bên ngoài handle
  }
}
