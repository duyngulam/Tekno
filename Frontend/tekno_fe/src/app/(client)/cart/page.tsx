"use client";
import EmptyCart from "@/components/cart-payment-checkout/EmptyCart";
import NoAccess from "@/components/cart-payment-checkout/NoAccess";
import ProductInCart from "@/components/cart-payment-checkout/ProductInCart";
import { Container } from "@/components/MainLayout/Container";
import FormattedPriced from "@/components/share/FormattedPriced";
import Stepper from "@/components/share/Stepper";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/hook/useAuth";
import { useCart } from "@/hook/useCart";
import { getProductsInCart } from "@/services/products";
import { Product } from "@/type/product";
import { get } from "http";
import { ShoppingBag } from "lucide-react";
import Link from "next/link";
import React from "react";

interface CartItem {
  product: Product;
  quantity: number;
}
export default function CartPage() {
  const { items, cleanCart, SubTotalPrice, getTotalPrice } = useCart();
  const { user, isAuthenticated } = useAuth();

  const ProductsInCart = items ?? [];

  return (
    <div>
      {isAuthenticated ? (
        <Container className="flex flex-col space-y-5 my-10">
          {ProductsInCart.length === 0 ? (
            <EmptyCart />
          ) : (
            <>
              <Stepper isActive={1} />
              <div className="grid lg:grid-cols-3 md:gap-8">
                <div className="lg:col-span-2">
                  <div className="flex gap-2 items-center mb-5">
                    <ShoppingBag />
                    <h1>Your shopping cart</h1>
                  </div>

                  <div className="flex flex-col gap-2">
                    {ProductsInCart?.map((p) => (
                      <ProductInCart product={p} />
                    ))}
                  </div>
                </div>
                <div className="flex flex-col py-5 px-3 border border-gray-300 rounded-md gap-4">
                  <p className="text-black font-bold text-2xl ">
                    Payment details
                  </p>
                  <div className="p-1 flex flex-col">
                    <div className="flex justify-between">
                      <p className="text-start">Subtotal</p>
                      <FormattedPriced price={SubTotalPrice} />
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
            </>
          )}
        </Container>
      ) : (
        <NoAccess></NoAccess>
      )}
    </div>
  );
}
