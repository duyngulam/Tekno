"use client";

import { motion } from "framer-motion";
import { ShoppingCart } from "lucide-react";
import { Button } from "@/components/ui/button";
import Link from "next/link";

export default function EmptyCart() {
  return (
    <div className="py-10 md:py-20 bg-gradient-to-b from-blue-50 to-white flex items-center justify-center p-4">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="bg-white rounded-2xl shadow-xl p-8 max-w-md w-full space-y-8"
      >
        {/* ICON ANIMATION */}
        <motion.div
          animate={{
            scale: [1, 1.1, 1],
            rotate: [0, 5, -5, 0],
          }}
          transition={{
            repeat: Infinity,
            duration: 5,
            ease: "easeInOut",
          }}
          className="relative w-48 h-48 mx-auto"
        >
          {/* image */}
          <motion.div
            animate={{
              x: [0, -10, 10, 0],
              y: [0, -5, 5, 0],
            }}
            transition={{
              repeat: Infinity,
              duration: 3,
              ease: "linear",
            }}
            className="absolute -top-4 -right-4 bg-secondary-500 rounded-full p-2"
          >
            {/* image */}
            <ShoppingCart
              size={24}
              className="w-20 h-20 text-white"
              strokeWidth={1.5}
            />
          </motion.div>
        </motion.div>

        {/* TEXT */}
        <div className="text-center space-y-4">
          <h2 className="text-3xl font-bold text-gray-800">
            Your cart is empty
          </h2>
          <p className="text-gray-600">
            It look like you haven't added anything to your cart yet. Let's
            change that and find some thing amazing products for you!
          </p>
        </div>

        {/* BUTTON */}
        <div>
          <Link
            href="/products"
            className="block bg-secondary/5 border border-secondary/20 text-center py-2.5 rounded-full text-sm font-semibold tracking-wide hover:border-secondary/80 hover:bg-secondary hover:text-white hoverEffect"
          >
            Discover Products
          </Link>
        </div>
      </motion.div>
    </div>
  );
}
