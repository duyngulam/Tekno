"use client";
import { useCallback, useEffect, useState } from "react";
import { favorApi } from "@/services/favor";
import { Product } from "@/type/product";

export default function useFavor(enabled = true) {
  const [items, setItems] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<Error | null>(null);
  const token =
    typeof window !== "undefined" ? localStorage.getItem("token") : null;

  const fetchFavor = useCallback(async () => {
    setError(null);
    setLoading(true);
    try {
      if (!token) throw new Error("No auth token");
      const res = await favorApi.getFavor(token);
      // favorApi.getFavor may return data array or { data: [...] }, handle both
      if (Array.isArray(res)) setItems(res);
      else if (res && Array.isArray((res as any).data))
        setItems((res as any).data);
      else setItems([]);
    } catch (err: any) {
      setError(err);
      setItems([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (!enabled) return;
    fetchFavor();
  }, [enabled, fetchFavor]);

  const addToFavor = async (variantId: number) => {
    if (!token) return alert("Bạn cần đăng nhập!");

    await favorApi.addToFavor(token, variantId);
    await fetchFavor();
  };

  // REMOVE ITEM
  const removeFavor = async (variantId: number) => {
    if (!token) return;

    await favorApi.removeFavor(token, variantId);
    await fetchFavor();
  };

  return {
    items,
    setItems,
    loading,
    error,
    refetch: fetchFavor,
    addToFavor,
    removeFavor,
  } as const;
}
