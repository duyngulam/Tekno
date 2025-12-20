import {  get,
  post,
  postForm,
  put,
  del } from "@/lib/api";
import API_BASE  from "./../lib/api";

export async function getBrandList() {
  try {
    const res = await get(`${API_BASE}/admin/brands/list`, { cache: "no-store" });

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message || "Lấy danh sách thuong hieu thất bại!");
    }

    return await res.json();
  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error; // để component bên ngoài handle
  }
}

export async function createBrand(fd: FormData) {
  try {
    const res = await postForm(`${API_BASE}/admin/brands`, fd);

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message || "Tạo thương hiệu thất bại!");
    }

    return await res.json();
  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error; // để component bên ngoài handle
  }
}

export async function updateBrand(id: string, fd: FormData) {
  try {
    const res = await put(`${API_BASE}/admin/brands/${id}`, fd);

    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message || "Cập nhật thương hiệu thất bại!");
    }

    return await res.json();
  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error; // để component bên ngoài handle
  }
}

export async function deleteBrand(id: string) {
  try {
    const res = await del(`${API_BASE}/admin/brands/${id}`);
    
    if (!res.ok) {
      const err = await res.json().catch(() => ({}));
      throw new Error(err.message || "Xóa thương hiệu thất bại!");
    }

    return await res.json();
  } catch (error) {
    console.error("❌ Lỗi khi gọi API:", error);
    throw error; // để component bên ngoài handle
  }
}