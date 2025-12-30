"use client";
import False from "@/components/cart-payment-checkout/result/false";
import FormattedPrice from "@/components/share/FormattedPriced";
import { getOrderByOrderId } from "@/services/order";
import { Order, OrderItem } from "@/type/order";
import { CheckCircle, X } from "lucide-react";
import { useSearchParams } from "next/navigation";
import React, { useEffect, useState } from "react";

export default function page() {
  const searchParams = useSearchParams();
  const orderId = searchParams.get("OrderId");

  console.log("Order ID from params:", orderId);

  const [items, setItems] = useState<OrderItem[]>([]);
  const [loadingOrder, setLoadingOrder] = useState(true);
  const [orderTotal, setOrderTotal] = useState<number>(0);
  const [status, setStatus] = useState<string>("");
  const [orderDetails, setOrderDetails] = useState<Order>();

  useEffect(() => {
    if (!orderId) return;

    let mounted = true;

    (async () => {
      try {
        setLoadingOrder(true);
        const token = localStorage.getItem("token") || "";
        const res = await getOrderByOrderId(token, Number(orderId));

        if (!mounted) return;

        setOrderDetails(res);
        setStatus(res?.payment?.status || "");
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

  console.log("status order", status);

  // Show loading state
  if (loadingOrder) {
    return (
      <div className="fixed inset-0 z-50 flex items-center justify-center">
        <div className="absolute inset-0 bg-black/30" />
        <div className="relative z-10 w-full max-w-md rounded-2xl bg-white p-6 md:p-8 shadow-2xl">
          <div className="flex flex-col gap-4 items-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
            <h3 className="text-lg font-medium text-gray-700">
              Processing payment...
            </h3>
            <p className="text-sm text-gray-500 text-center">
              Please wait while we verify your payment
            </p>
          </div>
        </div>
      </div>
    );
  }

  if (status === "Completed") {
    //return success page
    return (
      <div className="fixed inset-0 z-50 flex items-center justify-center">
        <div className="absolute inset-0 bg-black/30" />
        <div className="relative z-10 w-full max-w-md rounded-2xl bg-white p-6 md:p-8 shadow-2xl">
          <button
            aria-label="Close"
            className="absolute right-3 top-3 rounded-full p-1 hover:bg-gray-100"
          >
            <X className="h-4 w-4 text-gray-500" />
          </button>

          <div className="flex flex-col gap-4">
            <div className="flex items-center justify-center">
              <CheckCircle className="h-14 w-14 text-green-500" />
            </div>
            <h3 className="text-2xl font-semibold text-center text-green-700">
              Successful Payment
            </h3>

            <div className="grid grid-cols-2 gap-y-2 text-sm text-gray-700">
              <span className="text-gray-500">Payment type</span>
              {/* <span className="text-right">{paymentType}</span> */}

              <span className="text-gray-500">Phone number</span>
              <span className="text-right">{orderDetails?.orderNumber}</span>

              <span className="text-gray-500">Email</span>
              <span className="text-right">{orderDetails?.totalAmount}</span>

              <span className="text-gray-500">Transaction id</span>
              <span className="text-right">
                {orderDetails?.totalAmount ?? "-"}
              </span>

              <span className="text-gray-500">Amount Paid</span>
              <span className="text-right font-semibold">
                {orderDetails?.totalAmount != null ? (
                  <FormattedPrice price={orderDetails?.totalAmount} />
                ) : (
                  "-"
                )}
              </span>
            </div>

            <button
              onClick={() => {
                window.location.href = `/account/orders/order-status/${orderDetails?.orderNumber}`;
              }}
              className="mt-3 w-full rounded-lg bg-yellow-400 px-4 py-2 font-medium text-white hover:bg-yellow-500"
            >
              Order Status
            </button>
          </div>
        </div>
      </div>
    );
  } else {
    // Show failure page for any other status or if status is empty/failed
    return <False />;
  }
}
