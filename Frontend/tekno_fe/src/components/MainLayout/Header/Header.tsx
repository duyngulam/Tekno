"use client";
import React, { useState } from "react";
import { Search, ShoppingBasket, UserRound } from "lucide-react";
import { usePathname } from "next/navigation";

import { useAuth } from "@/hook/useAuth";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Container } from "../Container";
import Logo from "./Logo";
import HeaderMenu from "./HeaderMenu";
import SearchBar from "./SearchBar";
import CartIcon from "./CartIcon";
import SignIn from "./SignIn";
import MobileMenu from "./MobileMenu";
import ProfileMenu from "../ProfileMenu";
import Link from "next/link";

const Header = () => {
  const { user, isAuthenticated, logout } = useAuth();
  return (
    <header className="bg-white/70 md:border-b md:border-b-secondary py-5 sticky top-0 z-50 backdrop-blur-md">
      <Container className="flex items-center justify-between">
        {/* logo */}
        <div className="w-auto md:w-1/3 flex items-center gap-2.5 justify-start md:gap-0">
          <MobileMenu />
          <Logo className="hidden md:inline-flex" />
        </div>

        {/* NavButton */}
        <HeaderMenu />
        {/* NavAdmin */}
        <div className="w-auto md:w-1/3 flex items-center justify-end gap-5">
          <SearchBar />
          <CartIcon />

          {/* {!user ? <SignIn /> : <UserRound />} */}
          {/* USER MENU DROPDOWN */}
          <div className="relative group">
            {!user ? (
              <SignIn />
            ) : (
              <>
                <Link href="/account/personal-data">
                  <UserRound className="cursor-pointer" />
                </Link>

                {/* Dropdown */}
                <div
                  className="
          absolute right-0 top-full mt-2
          hidden group-hover:block
        "
                >
                  <ProfileMenu />
                </div>
              </>
            )}
          </div>
        </div>
      </Container>
    </header>
  );
};

export default Header;
