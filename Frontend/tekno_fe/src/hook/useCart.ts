'use client';
import { useState, useEffect } from 'react';
import { Product } from '@/type/product';
import { cartApi } from '@/services/cart';

export function useCart() {
  const [cart, setCart] = useState<{ product: Product; quantity: number }[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchCart = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await cartApi.getCart();
      setCart(data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const addToCart = async (variantId: number, quantity: number) => {
    await cartApi.addToCart({variantId, quantity});
    await fetchCart();
  };

  const removeFromCart = async (productId: number) => {
    await cartApi.removeFromCart(productId);
    await fetchCart();
  };

  useEffect(() => {
    fetchCart();
  }, []);

  return { cart, loading, error, fetchCart, addToCart, removeFromCart };
}
