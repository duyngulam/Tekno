"use client";

import React, { useState } from "react";
import Input from "./Input";
import { useRouter } from "next/navigation";
import { signupApi } from "@/api/auth";

type SignupFormProps = {
  setActiveTab?: React.Dispatch<React.SetStateAction<"login" | "register">>;
};

export default function SignUpForm({ setActiveTab }: SignupFormProps) {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    const formData = new FormData(e.currentTarget);
    const username = formData.get("name") as string;
    const email = formData.get("email") as string;
    const password = formData.get("password") as string;
    const role = "Customer";

    try {
      const data = await signupApi({ username, email, password, role });

      if (data.token) {
        localStorage.setItem("token", data.token);
      }

      alert("Đăng ký thành công! Hãy đăng nhập để tiếp tục.");

      setActiveTab?.("login");
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <h2 className="text-center font-semibold text-lg mb-4">
        Create your account
      </h2>

      <Input label="Name" name="name" type="text" />
      <Input label="E-mail" name="email" type="email" />
      <Input label="Password" name="password" type="password" />

      {error && <p className="text-red-500 text-sm">{error}</p>}

      <button
        type="submit"
        disabled={loading}
        className="mt-4 w-full bg-yellow-400 hover:bg-yellow-500 text-white font-semibold py-2 rounded-md"
      >
        {loading ? "Creating..." : "Create Account"}
      </button>
    </form>
  );
}
