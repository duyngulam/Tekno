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
import { EyeClosed, Key, MailIcon } from "lucide-react";

type LoginFormProps = {
  switchToRegister: () => void;
};

export default function LoginForm({ switchToRegister }: LoginFormProps) {
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
          <Field>
            <InputGroup>
              <InputGroupInput
                type="password"
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
                  aria-label="Copy"
                  title="Copy"
                  size="icon-xs"
                  onClick={() => {}}
                >
                  {/* {isCopied ? <IconCheck /> : <IconCopy />} */}
                  <EyeClosed />
                </InputGroupButton>
              </InputGroupAddon>
            </InputGroup>
          </Field>
        </FieldGroup>
        <FieldError>{error}</FieldError>
      </FieldSet>
      <p className="text-end">
        forgot your password?{" "}
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
