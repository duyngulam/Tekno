"use client";
import Link from "next/link";
import logo from "@asset/MainLogo.png";
import Image from "next/image";
import React, { useState } from "react";
import { Search, ShoppingBasket, UserRound } from "lucide-react";
import { usePathname } from "next/navigation";
import AuthModal from "../auth/AuthModal";
import { useAuth } from "@/hook/useAuth";
import { Button } from "../ui/button";

const Header = () => {
  const { user, isAuthenticated, logout } = useAuth();

  const pathname = usePathname();
  const [isLoginOpen, setIsLoginOpen] = useState(false);

  const navItems = [
    { href: "/", label: "Home" },
    { href: "/products", label: "Products" },
    { href: "/blogs", label: "Blogs" },
    { href: "/faq", label: "FAQ" },
    { href: "/contact-us", label: "Contact us" },
  ];

  console.log(isAuthenticated);
  return (
    <div className="bg-white text-black w-full flex items-center justify-between px-6 md:px-16 lg:px-32 py-3 border-b border-secondary">
      {/* Logo */}
      <Image src={logo} alt="Logo" className="w-10 md:w-12" />

      {/* Navbar */}
      <div className="flex items-center gap-4 lg:gap-8 max-md:hidden">
        {navItems.map((item) => {
          const isActive = pathname === item.href;
          return (
            <div key={item.href} className="group">
              <Link
                href={item.href}
                className={`transition ${
                  isActive
                    ? "text-primary font-semibold"
                    : "hover:text-primary active:text-primary"
                }`}
              >
                {item.label}
              </Link>
              <hr
                className={`border-t mt-1 transition ${
                  isActive
                    ? "border-primary"
                    : "border-transparent group-hover:border-secondary group-active:border-primary"
                }`}
              />
            </div>
          );
        })}
      </div>

      {/* Actions */}
      <div className="flex items-center gap-4">
        <Search className="h-6 w-6 cursor-pointer hover:text-primary active:text-primary transition max-md:hidden" />
        <ShoppingBasket className="h-6 w-6 cursor-pointer hover:text-primary active:text-primary transition" />

        {isAuthenticated ? (
          <div className="flex items-center gap-2">
            <span className="font-medium">{user?.email}</span>
            <Button
              className="bg-gray-200 text-gray-700 py-2 px-4 rounded-md hover:bg-gray-300"
              onClick={logout}
            >
              Logout
            </Button>
          </div>
        ) : (
          <Button
            //className="bg-primary text-white py-2 px-4 rounded-md hover:bg-primary/60 active:bg-primary"
            onClick={() => setIsLoginOpen(true)}
          >
            Login / Sign Up
          </Button>
        )}
      </div>
      {/* AuthModal renders outside the flex row, so it overlays the page */}
      <AuthModal isOpen={isLoginOpen} onClose={() => setIsLoginOpen(false)} />
    </div>
  );
};

export default Header;
