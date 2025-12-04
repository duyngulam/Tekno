"use client";
import ProductInCart from "@/components/cart-payment-checkout/ProductInCart";
import { Container } from "@/components/MainLayout/Container";
import Stepper from "@/components/share/Stepper";
import { Button } from "@/components/ui/button";
import { useCart } from "@/hook/useCart";
import { getProductsInCart } from "@/services/products";
import { Product } from "@/type/product";
import { get } from "http";
import Link from "next/link";
import React from "react";

interface CartItem {
  product: Product;
  quantity: number;
}
export default function CartPage() {
  const { cart, cleanCart } = useCart();
  console.log("Cart item", cart);

  const ProductsInCart = cart?.data.items ?? [];

  return (
    <Container className="flex flex-col space-y-5 my-10">
      <Stepper isActive={1} />
      <div className="flex justify-between">
        <div className="w-full md:w-7/12">
          <div className="flex justify-between items-center mb-5">
            <p>Your cart</p>
            <Button>Clean All</Button>
          </div>

          {ProductsInCart.length === 0 && (
            <p className="text-center text-lg font-medium py-10">
              Your cart is empty.
            </p>
          )}
          {ProductsInCart?.map((p) => (
            <ProductInCart product={p} />
          ))}
        </div>
        <div className="flex flex-col py-5 px-3 w-full md:w-4/12 border border-gray-300 rounded-md gap-4">
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
