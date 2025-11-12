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

export default function SignIn() {
  const [mode, setMode] = useState<"login" | "register">("login");
  return (
    // <button className="text-lg font-semibold text-gray-500 hover:text-black hover:cursor-pointer hoverEffect">
    //   Sign In
    // </button>
    <>
      <Dialog>
        <DialogTrigger asChild>
          <button onClick={() => setMode("login")}>Đăng nhập</button>
        </DialogTrigger>
        <hr></hr>
        <DialogTrigger asChild>
          <button onClick={() => setMode("register")}>Đăng kí</button>
        </DialogTrigger>
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
