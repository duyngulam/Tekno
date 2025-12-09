"use client";
import { useState, useEffect } from "react";
import { Product } from "@/type/product";
import { favorApi } from "@/services/favor";

export function useFavor() {
  const [favor, setFavor] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchFavor = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await favorApi.getFavor();
      setFavor(data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const addFavor = async (productId: number) => {
    await favorApi.addFavor(productId);
    await fetchFavor();
  };

  const removeFavor = async (productId: number) => {
    await favorApi.removeFavor(productId);
    await fetchFavor();
  };

  useEffect(() => {
    fetchFavor();
  }, []);

  return { favor, loading, error, fetchFavor, addFavor, removeFavor };
}
