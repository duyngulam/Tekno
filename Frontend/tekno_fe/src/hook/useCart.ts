'use client';

import { useState, useEffect } from 'react';
import { Product } from '@/type/product';
import { cartApi } from '@/services/cart';
export interface CartItem {
  id: number;
  cartId: number;
  variantId: number;
  quantity: number;
  price: number;
}

export interface CartResponse {
  success: boolean;
  message: string;
  data: {
    id: number;
    createdAt: string;
    updatedAt: string;
    userId: number;
    subtotal: number;
    totalItems: number;
    items: CartItem[];
  };
}


export function useCart() {
    const [cart, setCart] = useState<CartResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const getToken = () => {
    if (typeof window === "undefined") return null;
    return localStorage.getItem("token");
  };

  const fetchCart = async () => {
    const token = getToken();
    if (!token) return;

    setLoading(true);
    setError(null);

    try {
      const data = await cartApi.getCart(token);
      console.log("Fetched cart data:", data);
      
      setCart(data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const addToCart = async (variantId: number, quantity: number) => {
    const token = getToken();
    if (!token) return alert("Bạn cần đăng nhập!");

    await cartApi.addToCart(token, { variantId, quantity });
    await fetchCart();
  };

  const removeFromCart = async (variantId: number) => {
    const token = getToken();
    if (!token) return;

    await cartApi.removeFromCart(token, variantId);
    await fetchCart();
  };

  const cleanCart = async () => {
    try {
      setLoading(true);
          const token = getToken();
      if (!token) throw new Error("Không tìm thấy token");

      const res = await cartApi.cleanCart(token);

      if (!res.success) throw new Error("Xoá giỏ hàng thất bại");

      // API trả về 200 OK → làm rỗng cart trên FE tu fetch
      setCart(null);

      return true;
    } catch (err) {
      console.error(err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const updateQuantity = async (variantId: number, quantity: number) => {
    try {
      setLoading(true);

      const token = localStorage.getItem("token");
      if (!token) throw new Error("Token not found");

      const updated = await cartApi.updateQuantity(variantId, quantity, token);

      // Tự update cart FE
      //setCart(updated.data);

      return true;
    } catch (err) {
      console.error(err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCart();
  }, []);

  return { cart, loading, error, fetchCart, addToCart, removeFromCart,cleanCart };
}