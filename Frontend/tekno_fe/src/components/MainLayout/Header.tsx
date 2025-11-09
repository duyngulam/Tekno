"use client";
import Link from "next/link";
import logo from "../../../src/asset/MainLogo.png";
import Image from "next/image";
import React, { useState } from "react";
import { Search, ShoppingBasket, UserRound } from "lucide-react";
import { usePathname } from "next/navigation";

import { useAuth } from "@/hook/useAuth";
import { Button } from "../ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { AuthModal } from "../auth/AuthModal";
import LoginForm from "../auth/LoginForm";
import SignUpForm from "../auth/SignUpForm";
import { Container } from "./Container";
import Logo from "./Logo";
import HeaderMenu from "./HeaderMenu";
import SearchBar from "./SearchBar";
import CartIcon from "./CartIcon";
import SignIn from "./SignIn";
import MobileMenu from "./MobileMenu";

const Header = () => {
  const { user, isAuthenticated, logout } = useAuth();

  const pathname = usePathname();
  const [isLoginOpen, setIsLoginOpen] = useState(false);
  const [mode, setMode] = useState<"login" | "register">("login");

  const navItems = [
    { href: "/", label: "Home" },
    { href: "/products", label: "Products" },
    { href: "/blogs", label: "Blogs" },
    { href: "/faq", label: "FAQ" },
    { href: "/contact-us", label: "Contact us" },
  ];

  console.log(isAuthenticated);
  return (
    // <header className="bg-white text-black w-full flex items-center justify-between px-6 md:px-16 lg:px-32 py-3 border-b border-secondary">
    //   {/* Logo */}
    //   <Image src={logo} alt="Logo" className="w-10 md:w-12" />

    //   {/* Navbar */}
    //   <div className="flex items-center gap-4 lg:gap-8 max-md:hidden">
    //     {navItems.map((item) => {
    //       const isActive = pathname === item.href;
    //       return (
    //         <div key={item.href} className="group">
    //           <Link
    //             href={item.href}
    //             className={`transition ${
    //               isActive
    //                 ? "text-primary font-semibold"
    //                 : "hover:text-primary active:text-primary"
    //             }`}
    //           >
    //             {item.label}
    //           </Link>
    //           <hr
    //             className={`border-t mt-1 transition ${
    //               isActive
    //                 ? "border-primary"
    //                 : "border-transparent group-hover:border-secondary group-active:border-primary"
    //             }`}
    //           />
    //         </div>
    //       );
    //     })}
    //   </div>

    //   {/* Actions */}
    //   <div className="flex items-center gap-4">
    //     <Search className="h-6 w-6 cursor-pointer hover:text-primary active:text-primary transition max-md:hidden" />
    //     <ShoppingBasket className="h-6 w-6 cursor-pointer hover:text-primary active:text-primary transition" />

    //     {isAuthenticated ? (
    //       <div className="flex items-center gap-2">
    //         <span className="font-medium">{user?.email}</span>
    //         <Button
    //           className="bg-gray-200 text-gray-700 py-2 px-4 rounded-md hover:bg-gray-300"
    //           onClick={logout}
    //         >
    //           Logout
    //         </Button>
    //       </div>
    //     ) : (
    //       <Dialog>
    //         <DialogTrigger asChild>
    //           <Button variant="ghost" onClick={() => setMode("login")}>
    //             Đăng nhập
    //           </Button>
    //         </DialogTrigger>
    //         <hr></hr>
    //         <DialogTrigger asChild>
    //           <Button variant="ghost" onClick={() => setMode("register")}>
    //             Đăng kí
    //           </Button>
    //         </DialogTrigger>
    //         <DialogContent
    //           onInteractOutside={(e) => e.preventDefault()}
    //           onEscapeKeyDown={(e) => e.preventDefault()}
    //         >
    //           {/* <AuthModal mode={mode} /> */}
    //           {mode === "login" ? (
    //             <LoginForm switchToRegister={() => setMode("register")} />
    //           ) : (
    //             <SignUpForm switchToLogin={() => setMode("login")} />
    //           )}
    //         </DialogContent>
    //       </Dialog>
    //     )}
    //   </div>
    // </header>
    <header className="bg-white border-b border-b-secondary py-5">
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

          {!user ? <SignIn /> : <UserRound />}
        </div>
      </Container>
    </header>
  );
};

export default Header;
