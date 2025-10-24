"use client";
import Link from "next/link";
import logo from "../../../../src/asset/MainLogo.png";
import Image from "next/image";
import React, { useState } from "react";
import {
  MagnifyingGlassIcon,
  ShoppingCartIcon,
} from "@heroicons/react/24/outline";
import { usePathname } from "next/navigation";
import { useAuth } from "@/hook/useAuth";

const Header = () => {
  const { user, isAuthenticated, logout } = useAuth();

  const pathname = usePathname();
  const [isLoginOpen, setIsLoginOpen] = useState(false);

  console.log(isAuthenticated);
  return (
    <div className="bg-white text-black w-full flex items-center justify-between px-6 md:px-16 lg:px-32 py-3 border-b border-secondary sticky top-0">
      {/* Logo */}
      <Image src={logo} alt="Logo" className="w-10 md:w-12" />

      {/* Actions */}
      <div className="flex items-center gap-4">
        <MagnifyingGlassIcon className="h-6 w-6 cursor-pointer hover:text-primary active:text-primary transition max-md:hidden" />

        {isAuthenticated ? (
          <div className="flex items-center gap-2">
            <span className="font-medium">{user?.email}</span>
            <button
              className="bg-gray-200 text-gray-700 py-2 px-4 rounded-md hover:bg-gray-300"
              onClick={logout}
            >
              Logout
            </button>
          </div>
        ) : (
          <button
            className="bg-primary text-white py-2 px-4 rounded-md hover:bg-primary/60 active:bg-primary"
            onClick={() => setIsLoginOpen(true)}
          >
            Login / Sign Up
          </button>
        )}
      </div>
    </div>
  );
};

export default Header;
