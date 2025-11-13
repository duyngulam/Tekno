"use client";

import React, { useState } from "react";
import { signupApi } from "@/services/auth";
import {
  Field,
  FieldContent,
  FieldDescription,
  FieldError,
  FieldGroup,
  FieldLabel,
  FieldLegend,
  FieldSeparator,
  FieldSet,
  FieldTitle,
} from "@/components/ui/field";
import { Input } from "../ui/input";
import { Checkbox } from "../ui/checkbox";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "../ui/input-group";
import { KeyIcon, MailIcon, UserRound } from "lucide-react";

type SignupFormProps = {
  switchToLogin: () => void;
};

export default function SignUpForm({ switchToLogin }: SignupFormProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    const formData = new FormData(e.currentTarget);
    const username = formData.get("username") as string;
    const email = formData.get("email") as string;
    const password = formData.get("password") as string;
    const role = "Customer";

    try {
      const data = await signupApi({ username, email, password, role });

      if (data.token) {
        localStorage.setItem("token", data.token);
      }

      alert("Đăng ký thành công! Hãy đăng nhập để tiếp tục.");
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4 px-6  py-6">
      <FieldSet>
        <FieldLegend className="w-full text-center font-bold ">
          Create your account
        </FieldLegend>
        <FieldGroup>
          <Field>
            <InputGroup>
              <InputGroupInput
                id="username"
                type="text"
                name="username"
                autoComplete="off"
                placeholder="Your username"
              />
              <InputGroupAddon>
                <UserRound />
              </InputGroupAddon>
            </InputGroup>
          </Field>
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
                <KeyIcon />
              </InputGroupAddon>
            </InputGroup>
          </Field>
          <Field orientation="horizontal">
            <Checkbox id="checkout-7j9-same-as-shipping-wgm" defaultChecked />
            <FieldLabel
              htmlFor="checkout-7j9-same-as-shipping-wgm"
              className="font-normal"
            >
              I agree to all Terms & Conditions
            </FieldLabel>
          </Field>
        </FieldGroup>
        <FieldError>{error}</FieldError>
      </FieldSet>

      {/* {error && <p className="text-red-500 text-sm">{error}</p>} */}

      <button
        type="submit"
        disabled={loading}
        className="mt-4 w-full bg-yellow-400 hover:bg-yellow-500 text-white font-semibold py-2 rounded-md"
      >
        {loading ? "Creating..." : "Create Account"}
      </button>
      <p className="text-sm text-center text-muted-foreground">
        Chưa có tài khoản?{" "}
        <span
          className="text-primary cursor-pointer hover:underline"
          onClick={switchToLogin}
        >
          Đăng nhập
        </span>
      </p>
    </form>
  );
}
