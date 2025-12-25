



import { API_BASE_URL } from "@/lib/apiConfig";
import { District, Province, Ward } from "@/type/location";

// /locations/provinces
export async function getProvinces(): Promise<Province[]> {
  const res = await fetch(`${API_BASE_URL}/locations/provinces`, {
    method: "GET",
    headers: { "Content-Type": "application/json" },
    cache: "no-store",
  });
  const json = await res.json();
  if (!res.ok) throw new Error(json?.message || "Failed to fetch provinces");
  return (json.data ?? json) as Province[];
}
// /locations/provinces/92/districts
export async function getDistricts(provinceId: number): Promise<District[]> {
  const res = await fetch(`${API_BASE_URL}/locations/provinces/${provinceId}/districts`, {
    method: "GET",
    headers: { "Content-Type": "application/json" },
    cache: "no-store",
  });
  const json = await res.json();
  if (!res.ok) throw new Error(json?.message || "Failed to fetch districts");
  return (json.data ?? json) as District[];
}
// /locations/districts/926/wards
export async function getWards(districtId: number): Promise<Ward[]> {
  const res = await fetch(`${API_BASE_URL}/locations/districts/${districtId}/wards`, {
    method: "GET",
    headers: { "Content-Type": "application/json" },
    cache: "no-store",
  });
  const json = await res.json();
  if (!res.ok) throw new Error(json?.message || "Failed to fetch wards");
  return (json.data ?? json) as Ward[];
}