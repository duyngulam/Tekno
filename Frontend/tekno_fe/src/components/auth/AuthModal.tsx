"use client";

import React, { useEffect, useRef, useState } from "react";
import Input from "@/components/auth/Input";
import Login from "./Login";
import SignUp from "./SignUp";

type ModalProps = {
  isOpen: boolean;
  onClose: () => void;
};

export default function AuthModal({ isOpen, onClose }: ModalProps) {
  const [activeTab, setActiveTab] = useState<"login" | "register">("login");
  const dialogRef = useRef<HTMLDialogElement | null>(null);

  // Control open/close từ props
  useEffect(() => {
    if (!dialogRef.current) return;
    if (isOpen && !dialogRef.current.open) {
      dialogRef.current.showModal();
    } else if (!isOpen && dialogRef.current.open) {
      dialogRef.current.close();
    }
  }, [isOpen]);

  return (
    <dialog ref={dialogRef} className="modal" onClose={onClose}>
      <div className="modal-box w-md md:w-lg max-w-full px-7 md:px-14">
        {/* Tabs */}
        <div className="flex mb-4">
          {/* Login Tab */}
          <button
            onClick={() => setActiveTab("login")}
            className={`flex-1 py-2 text-center font-medium border-b-2 transition-colors duration-300 ease-in-out
          ${
            activeTab === "login"
              ? "text-secondary border-secondary"
              : "text-gray-500 border-gray hover:text-primary"
          }`}
          >
            Log in
          </button>

          {/* Register Tab */}
          <button
            onClick={() => setActiveTab("register")}
            className={`flex-1 py-2 text-center font-medium border-b-2 transition-colors duration-300 ease-in-out
          ${
            activeTab === "register"
              ? "text-secondary border-secondary"
              : "text-gray-500 border-gray hover:text-primary"
          }`}
          >
            Create Account
          </button>
        </div>
        {activeTab === "login" && <Login />}
        {activeTab === "register" && <SignUp />}
        {/* <Login /> */}
      </div>

      {/* Overlay click */}
      <form method="dialog" className="modal-backdrop" onClick={onClose}>
        <button>close</button>
      </form>
    </dialog>
  );
}
