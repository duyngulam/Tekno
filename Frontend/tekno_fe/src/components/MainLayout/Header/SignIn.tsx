import React, { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import LoginForm from "../../auth/LoginForm";
import SignUpForm from "../../auth/SignUpForm";
import { Button } from "@/components/ui/button";

export default function SignIn() {
  const [mode, setMode] = useState<"login" | "register">("login");
  return (
    // <button className="text-lg font-semibold text-gray-500 hover:text-black hover:cursor-pointer hoverEffect">
    //   Sign In
    // </button>
    <>
      <Dialog>
        <div className="flex items-center justify-center gap-3">
          <DialogTrigger asChild>
            <button
              onClick={() => setMode("login")}
              className="
        px-1 py-1.5
        text-md font-medium text-gray-700
        hover:text-primary
        hover:scale-105
        transition-all duration-200 ease-out
        focus:outline-none
      "
            >
              Đăng nhập
            </button>
          </DialogTrigger>

          {/* Divider */}
          <span className="h-4 w-px bg-gray-300" />

          <DialogTrigger asChild>
            <button
              onClick={() => setMode("register")}
              className="
        px-1 py-1.5
        text-md font-medium text-gray-700
        hover:text-primary
        hover:scale-105
        transition-all duration-200 ease-out
        focus:outline-none
      "
            >
              Đăng ký
            </button>
          </DialogTrigger>
        </div>

        <DialogContent
          onInteractOutside={(e) => e.preventDefault()}
          onEscapeKeyDown={(e) => e.preventDefault()}
        >
          {/* <AuthModal mode={mode} /> */}
          {mode === "login" ? (
            <LoginForm switchToRegister={() => setMode("register")} />
          ) : (
            <SignUpForm switchToLogin={() => setMode("login")} />
          )}
        </DialogContent>
      </Dialog>
    </>
  );
}
