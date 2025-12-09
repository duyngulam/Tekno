"use client";
import { useCallback, useEffect, useState } from "react";
import { favorApi } from "@/services/favor";
import { Product } from "@/type/product";

export default function useFavor(enabled = true) {
  const [items, setItems] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<Error | null>(null);

  const fetchFavor = useCallback(async () => {
    setError(null);
    setLoading(true);
    try {
      const token =
        typeof window !== "undefined" ? localStorage.getItem("token") : null;
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

  return {
    items,
    setItems,
    loading,
    error,
    refetch: fetchFavor,
  } as const;
}
