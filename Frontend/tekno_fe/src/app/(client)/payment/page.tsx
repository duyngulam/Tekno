// import { Container } from "@/components/MainLayout/Container";
// import Stepper from "@/components/share/Stepper";
// import React from "react";

// export default function PaymentPage() {
//   return (
//     <Container className="flex flex-col space-y-5 my-10">
//       <Stepper isActive={3} />
//     </Container>
//   );
// }

"use client";

import React, { useEffect, useMemo, useState } from "react";
import { Container } from "@/components/MainLayout/Container";
import Stepper from "@/components/share/Stepper";
import { CreditCard, Plus, Edit2, User } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import { PaymentGateway, PaymentPayload } from "@/type/payment";
import { getPaymentGateways, processPayment } from "@/services/payment";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "recharts";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { ProfileAddress } from "@/type/address";
import { getProfileAddresses } from "@/services/profile";
import { toast } from "sonner";
import { useSearchParams } from "next/navigation";
import { getOrderByOrderId } from "@/services/order";
import { OrderItem } from "@/type/order";
import { log } from "console";

export default function PaymentPage() {
  const searchParams = useSearchParams();
  const orderId = searchParams.get("orderId");

  // remove mock
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

  // discount code
  const [code, setCode] = useState("");
  const [appliedDiscount, setAppliedDiscount] = useState<number>(0);

  // payment method
  const [gateways, setGateways] = useState<PaymentGateway[]>([]);
  const [paymentMethod, setPaymentMethod] = useState<string>("");

  // address (mock default)
  const [open, setOpen] = useState(false);
  const [addresses, setAddresses] = useState<ProfileAddress[]>([]);
  const defaultAddress = addresses.length ? addresses[0] : undefined;
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (!token) {
      setLoading(false);
      return;
    }

    // fetch addresses
    (async () => {
      try {
        const list = await getProfileAddresses(token);
        setAddresses(Array.isArray(list) ? list : []);
      } catch (e) {
        console.error("Fetch addresses error:", e);
        setAddresses([]);
      }
    })();
  }, []);

  useEffect(() => {
    (async () => {
      try {
        const data = await getPaymentGateways();
        setGateways(data);
        if (data.length && !paymentMethod) setPaymentMethod(String(data[0].id));
      } catch (e) {
        console.error("Fetch gateways error", e);
      }
    })();
  }, []);

  // billing address same as shipping
  const [sameAsShipping] = useState(true);

  // shipment cost (mock)
  const shipmentCost = 22.5;

  // recompute subtotal from fetched items
  const subtotal = useMemo(
    () => items.reduce((sum, it) => sum + it.price * it.quantity, 0),
    [items]
  );

  // prefer backend total if present; otherwise compute
  const grandTotal = useMemo(() => {
    const computed = Math.max(0, subtotal - appliedDiscount + shipmentCost);
    return orderTotal > 0 ? orderTotal : computed;
  }, [orderTotal, subtotal, appliedDiscount, shipmentCost]);

  const applyCode = () => {
    // Simple mock: apply -$11.87 if any non-empty code
    if (code.trim()) {
      setAppliedDiscount(11.87);
    } else {
      setAppliedDiscount(0);
    }
  };

  const continueToPay = async () => {
    try {
      const token = localStorage.getItem("token") || "";
      if (!token) throw new Error("Missing token");
      if (!orderId) throw new Error("Missing orderId");

      const shippingAddressId = defaultAddress?.id;
      if (!shippingAddressId) throw new Error("Select a shipping address");

      const gatewayId = Number(paymentMethod || gateways[0]?.id);
      console.log("Selected gatewayId:", gateways);

      // if (!gatewayId) throw new Error("Select a payment gateway");

      const gw = gateways.find((g) => Number(g.id) === gatewayId);
      // pick a method id from gateway definition if available; fallback to 1
      const method =
        Number((gw as any)?.methods?.[0]?.id) ||
        Number((gw as any)?.defaultMethod) ||
        Number((gw as any)?.method) ||
        1;

      const payload: PaymentPayload = {
        shippingAddressId,
        gateway: gatewayId,
        method,
        returnUrl: `${window.location.origin}/payment/result?orderId=${orderId}`,
        orderId: Number(orderId),
      };

      console.log("Payment payload:", payload);

      const { paymentUrl } = await processPayment(token, payload);
      localStorage.setItem("Payment URL:", paymentUrl);

      if (!paymentUrl) throw new Error("No payment URL returned");
      window.location.href = paymentUrl;
    } catch (e: any) {
      toast.error(e?.message || "Payment error");
    }
  };

  return (
    <Container className="flex flex-col space-y-6 my-10">
      <Stepper isActive={3} />

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        {/* địa chỉ nhân hàng */}

        {/* Left: Payment + Billing */}
        <div className="lg:col-span-7 rounded-xl bg-blue-50 p-4 space-y-4 border border-blue-100">
          <div>
            <div className="rounded-md border bg-white">
              <div className="border-b bg-gradient-to-r from-red-200 via-blue-200 to-red-200 h-2 rounded-t-md" />
              <div className="p-4 flex items-start gap-3">
                <span className="text-red-500 mt-1">📍</span>
                <div className="flex-1">
                  <div className="text-lg font-semibold text-red-600">
                    Địa Chỉ Nhận Hàng
                  </div>
                  <div className="mt-2 flex flex-wrap items-center gap-3">
                    <span className="font-semibold">
                      {defaultAddress?.recipientName || "Khánh Trang"}
                    </span>
                    <span className="text-gray-700">
                      (
                      {defaultAddress?.phoneNumber
                        ? `+84 ${defaultAddress.phoneNumber}`
                        : "(+84) 358 517 126"}
                      )
                    </span>
                    <span className="text-gray-800">
                      {defaultAddress
                        ? `${defaultAddress.addressLine} , ${defaultAddress.wardName}, ${defaultAddress.districtName}, ${defaultAddress.provinceName}`
                        : "Xóm 4-Trà Tri, Xã Hải Hưng, Huyện Hải Lăng, Quảng Trị"}
                    </span>
                  </div>
                </div>
                <button
                  type="button"
                  onClick={() => {
                    /* open change address modal/page */
                  }}
                  className="text-blue-600 hover:underline whitespace-nowrap"
                >
                  Thay Đổi
                </button>
              </div>
            </div>
          </div>

          <h2 className="font-semibold text-gray-800">Payment</h2>

          <RadioGroup
            value={paymentMethod}
            onValueChange={(value) => setPaymentMethod(value)}
          >
            {gateways.map((g) => (
              <div key={g.id} className="flex items-center gap-2">
                <RadioGroupItem value={String(g.id)} disabled={!g.available} />
                <span>{g.name}</span>
              </div>
            ))}
          </RadioGroup>

          <Link
            href="/checkout"
            className="text-sm text-yellow-500 hover:underline"
          >
            Return to checkout
          </Link>
        </div>

        {/* Right: Order Summary */}
        <div className="lg:col-span-5">
          <div className="rounded-xl bg-white p-4 border">
            <h3 className="font-semibold text-gray-800 mb-4">Your Order</h3>

            <div className="space-y-3 max-h-72 overflow-auto">
              {loadingOrder ? (
                <div className="py-3 text-sm text-gray-500">Loading order…</div>
              ) : (
                items.map((it) => (
                  <div key={it.id} className="flex items-center gap-3">
                    <div className="w-16 h-16 rounded-md bg-gray-100 overflow-hidden">
                      <Image
                        src={it.product.primaryImagePath}
                        alt={it.product.slug}
                        width={64}
                        height={64}
                        className="w-16 h-16 object-cover"
                      />
                    </div>
                    <div className="flex-1">
                      <div className="text-sm font-medium line-clamp-2">
                        {it.product.name}
                      </div>
                      <div className="text-xs text-gray-500">
                        x{it.quantity}
                        {/* {it.variant. ? `· ${it.color}` : ""} */}
                      </div>
                    </div>
                    <div className="text-sm text-gray-700">
                      ${it.price.toFixed(2)}
                    </div>
                  </div>
                ))
              )}
            </div>

            {/* Discount code */}
            <div className="mt-4 flex gap-2">
              <input
                className="flex-1 border rounded-md px-3 py-2 text-sm"
                placeholder="discount code"
                value={code}
                onChange={(e) => setCode(e.target.value)}
              />
              <button
                onClick={applyCode}
                className="px-4 py-2 rounded-md bg-gray-100 hover:bg-gray-200 text-sm"
              >
                Apply
              </button>
            </div>

            {/* Totals */}
            <div className="mt-4 space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-600">Subtotal</span>
                <span className="text-gray-800">${subtotal.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Discount</span>
                <span className="text-gray-800">
                  {appliedDiscount > 0
                    ? `-$${appliedDiscount.toFixed(2)}`
                    : "$0.00"}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Shipment cost</span>
                <span className="text-gray-800">
                  ${shipmentCost.toFixed(2)}
                </span>
              </div>
              <div className="flex justify-between font-semibold pt-2 border-t">
                <span>Grand Total</span>
                <span>${grandTotal.toFixed(2)}</span>
              </div>
            </div>

            {/* Continue button */}
            <button
              onClick={continueToPay}
              className="mt-4 w-full bg-yellow-400 hover:bg-yellow-500 text-white font-medium py-3 rounded-md"
            >
              Continue to pay
            </button>
          </div>
        </div>
      </div>
    </Container>
  );
}
