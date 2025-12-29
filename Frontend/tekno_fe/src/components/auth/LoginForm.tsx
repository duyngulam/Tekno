"use client";

import React, { useState } from "react";

import { useRouter } from "next/navigation";
import { useAuthContext } from "@/context/AuthContext"; // ✅ thêm dòng này
import { Field, FieldError, FieldGroup, FieldSet } from "../ui/field";
import { Input } from "../ui/input";

import {
  InputGroup,
  InputGroupAddon,
  InputGroupButton,
  InputGroupInput,
} from "../ui/input-group";
import { EyeClosed, Eye, Key, MailIcon } from "lucide-react";
import { toast } from "sonner";

type LoginFormProps = {
  switchToRegister: () => void;
};

export default function LoginForm({ switchToRegister }: LoginFormProps) {
  const router = useRouter();
  const { login, isAdmin, user } = useAuthContext(); // ✅ gọi login từ context

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showPassword, setShowPassword] = useState(false);

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    const formData = new FormData(e.currentTarget);
    const email = formData.get("email") as string;
    const password = formData.get("password") as string;

    try {
      const user = await login(email, password); // ✅ user trả về ngay dữ liệu đúng
      toast.success("Đăng nhập thành công!");
      //alert("Đăng nhập thành công!");

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
    <form onSubmit={handleSubmit} className="space-y-4 px-6  py-6">
      <h2 className="text-center font-semibold text-lg mb-4">
        Log in to Tekno
      </h2>

      <FieldSet>
        {/* <FieldLegend className="w-full justify-center">
          Create your account
        </FieldLegend>
        <FieldDescription>This appears on Create your account</FieldDescription> */}
        <FieldGroup>
          {/* email */}
          <Field>
            <InputGroup>
              <InputGroupInput
                type="email"
                id="email"
                name="email"
                autoComplete="off"
                placeholder="Enter your email"
              />
              <InputGroupAddon>
                <MailIcon />
              </InputGroupAddon>
            </InputGroup>
          </Field>
          {/* password */}
          <Field>
            <InputGroup>
              <InputGroupInput
                type={showPassword ? "text" : "password"}
                id="password"
                name="password"
                autoComplete="off"
                placeholder="Your Password"
              />
              <InputGroupAddon>
                <Key />
              </InputGroupAddon>
              <InputGroupAddon align="inline-end">
                <InputGroupButton
                  aria-label={showPassword ? "Hide password" : "Show password"}
                  title={showPassword ? "Hide" : "Show"}
                  size="icon-xs"
                  onClick={() => setShowPassword((v) => !v)}
                  type="button"
                >
                  {showPassword ? <Eye /> : <EyeClosed />}
                </InputGroupButton>
              </InputGroupAddon>
            </InputGroup>
          </Field>
        </FieldGroup>
        <FieldError>{error}</FieldError>
      </FieldSet>
      <p className="text-end">
        Forgot your password?{" "}
        <span className="text-primary cursor-pointer hover:underline">
          Reset here
        </span>
      </p>

      <button
        type="submit"
        disabled={loading}
        className="mt-4 w-full bg-yellow-400 hover:bg-yellow-500 text-white font-semibold py-2 rounded-md"
      >
        {loading ? "Signing in..." : "Sign in"}
      </button>
      <p className="text-sm text-center text-muted-foreground">
        Chưa có tài khoản?{" "}
        <span
          className="text-primary cursor-pointer hover:underline"
          onClick={switchToRegister}
        >
          Đăng ký ngay
        </span>
      </p>
    </form>
  );
}
