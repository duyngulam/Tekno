"use client";
import { getOrderByOrderId } from "@/services/order";
import { OrderItem } from "@/type/order";
import { useSearchParams } from "next/navigation";
import React, { useEffect, useState } from "react";

export default function page() {
  const searchParams = useSearchParams();
  const orderId = searchParams.get("orderId");

  console.log("Order ID from params:", orderId);

  const [items, setItems] = useState<OrderItem[]>([]);
  const [loadingOrder, setLoadingOrder] = useState(true);
  const [orderTotal, setOrderTotal] = useState<number>(0);

  useEffect(() => {
    let mounted = true;
    (async () => {
      if (!orderId) return;
      try {
        setLoadingOrder(true);
        const token = localStorage.getItem("token") || "";
        const res = await getOrderByOrderId(token, Number(orderId));
        // normalize response

        console.log(res);

        const order = res;
        const list = order.items as OrderItem[];
        if (mounted) {
          setItems(list);
          setOrderTotal(Number(order.totalAmount ?? 0));
        }
      } catch (e) {
        console.error("Fetch order by id error:", e);
      } finally {
        if (mounted) setLoadingOrder(false);
      }
    })();
    return () => {
      mounted = false;
    };
  }, [orderId]);
  return <div>page</div>;
}
