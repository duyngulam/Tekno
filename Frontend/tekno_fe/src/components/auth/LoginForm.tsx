"use client";

import React, { useState } from "react";
import Input from "./Input";
import { useRouter } from "next/navigation";
import { useAuthContext } from "@/context/AuthContext"; // ✅ thêm dòng này

type LoginFormProps = {
  onClose?: () => void;
};

export default function LoginForm({ onClose }: LoginFormProps) {
  const router = useRouter();
  const { login, isAdmin, user } = useAuthContext(); // ✅ gọi login từ context

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    const formData = new FormData(e.currentTarget);
    const email = formData.get("email") as string;
    const password = formData.get("password") as string;

    try {
      const user = await login(email, password); // ✅ user trả về ngay dữ liệu đúng
      alert("Đăng nhập thành công!");
      onClose?.();

      if (user && user.role.toLowerCase() === "admin")
        router.push("/dashboard");
      else router.push("/");
    } catch (err: any) {
      setError(err.message || "Đăng nhập thất bại");
    } finally {
      setLoading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <h2 className="text-center font-semibold text-lg mb-4">Sign in</h2>

      <Input label="E-mail" name="email" type="email" />
      <Input label="Password" name="password" type="password" />

      {error && <p className="text-red-500 text-sm">{error}</p>}

      <button
        type="submit"
        disabled={loading}
        className="mt-4 w-full bg-yellow-400 hover:bg-yellow-500 text-white font-semibold py-2 rounded-md"
      >
        {loading ? "Signing in..." : "Sign in"}
      </button>
    </form>
  );
}
