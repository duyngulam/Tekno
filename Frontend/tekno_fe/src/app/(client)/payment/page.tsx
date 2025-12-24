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
import { PaymentGateway } from "@/type/payment";
import { getPaymentGateways } from "@/services/payment";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "recharts";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";

type OrderItem = {
  id: string | number;
  name: string;
  price: number;
  qty: number;
  image: string;
  color?: string;
};

export default function PaymentPage() {
  // mock order items (replace with real cart state)
  const [items] = useState<OrderItem[]>([
    {
      id: 1,
      name: "MacBook Pro M2 MNEJ3 2022 LLA 13.3 inch",
      price: 433.0,
      qty: 1,
      image: "/images/sample/macbook.jpg",
      color: "Black",
    },
    {
      id: 2,
      name: "Hardcase 13-15 inch Laptop Case Silicone",
      price: 35.25,
      qty: 1,
      image: "/images/sample/case.jpg",
      color: "Blue",
    },
    {
      id: 3,
      name: "Laptop Privacy Screen for 13 inch MacBook",
      price: 33.58,
      qty: 1,
      image: "/images/sample/privacy.jpg",
      color: "Black",
    },
  ]);

  // discount code
  const [code, setCode] = useState("");
  const [appliedDiscount, setAppliedDiscount] = useState<number>(0);

  // payment method
  const [gateways, setGateways] = useState<PaymentGateway[]>([]);
  const [paymentMethod, setPaymentMethod] = useState<string>("");

  useEffect(() => {
    (async () => {
      try {
        const data = await getPaymentGateways();
        // const available = data.filter((g) => g.available);
        setGateways(data);
        if (data.length && !paymentMethod)
          setPaymentMethod(data[0].id.toString());
      } catch (e) {
        console.error("Fetch gateways error", e);
      }
    })();
  }, []);

  // billing address same as shipping
  const [sameAsShipping] = useState(true);

  // shipment cost (mock)
  const shipmentCost = 22.5;

  const subtotal = useMemo(
    () => items.reduce((sum, it) => sum + it.price * it.qty, 0),
    [items]
  );
  const grandTotal = useMemo(
    () => Math.max(0, subtotal - appliedDiscount + shipmentCost),
    [subtotal, appliedDiscount, shipmentCost]
  );

  const applyCode = () => {
    // Simple mock: apply -$11.87 if any non-empty code
    if (code.trim()) {
      setAppliedDiscount(11.87);
    } else {
      setAppliedDiscount(0);
    }
  };

  const continueToPay = () => {
    // Normally redirect to payment gateway step or call backend to create session
    alert(
      `Proceeding to pay with ${paymentMethod.toUpperCase()} - Total: $${grandTotal.toFixed(
        2
      )}`
    );
  };

  return (
    <Container className="flex flex-col space-y-6 my-10">
      <Stepper isActive={3} />

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        {/* Left: Payment + Billing */}
        <div className="lg:col-span-7 rounded-xl bg-blue-50 p-4 space-y-4 border border-blue-100">
          <h2 className="font-semibold text-gray-800">Payment</h2>

          <RadioGroup
            value={paymentMethod}
            onValueChange={(value) => setPaymentMethod(value)}
          >
            {gateways.map((g) => (
              <div key={g.id} className="flex items-center gap-2">
                <RadioGroupItem value={g.name} disabled={g.available} />
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
              {items.map((it) => (
                <div key={it.id} className="flex items-center gap-3">
                  <div className="w-16 h-16 rounded-md bg-gray-100 overflow-hidden">
                    <Image
                      src={it.image}
                      alt={it.name}
                      width={64}
                      height={64}
                      className="w-16 h-16 object-cover"
                    />
                  </div>
                  <div className="flex-1">
                    <div className="text-sm font-medium line-clamp-2">
                      {it.name}
                    </div>
                    <div className="text-xs text-gray-500">
                      x{it.qty} {it.color ? `· ${it.color}` : ""}
                    </div>
                  </div>
                  <div className="text-sm text-gray-700">
                    ${it.price.toFixed(2)}
                  </div>
                </div>
              ))}
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
