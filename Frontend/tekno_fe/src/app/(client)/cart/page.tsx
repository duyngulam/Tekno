import ProductInCart from "@/components/cart-payment-checkout/ProductInCart";
import { Container } from "@/components/MainLayout/Container";
import Stepper from "@/components/share/Stepper";
import Link from "next/link";
import React from "react";

export default function CartPage() {
  return (
    <Container className="flex flex-col space-y-5 my-10">
      <Stepper isActive={1} />
      <div className="flex justify-between">
        <div className="w-7/12 bg-amber-300">
          <ProductInCart />
          <ProductInCart />
        </div>
        <div className="flex flex-col py-5 px-3 w-4/12 border border-gray-300 rounded-md gap-4">
          <p className="text-black font-bold text-2xl ">Payment details</p>
          <div className="p-1 flex flex-col">
            <div className="flex justify-between">
              <p className="text-start">Subtotal</p>
              <p className="text-end">$519.52</p>
            </div>
            <hr></hr>
            <div className="flex justify-between">
              <p className="text-start">Grand total</p>
              <p className="text-end">$519.52</p>
            </div>
          </div>
          <Link
            href="/payment"
            className="bg-primary/70 text-white font-normal text-xl rounded-md hover:bg-primary hoverEffect p-3 text-center"
          >
            Procced to checkout
          </Link>
        </div>
      </div>
    </Container>
  );
}
