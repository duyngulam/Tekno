export async function getBranchList() {
  try {
    const res = await fetch("https://localhost:5000/api/brands/list", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message || "Lấy danh sách chi nhánh thất bại!");
    }

    return await res.json();
  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error; // để component bên ngoài handle
  }
}
