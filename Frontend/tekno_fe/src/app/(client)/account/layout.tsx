"use client";

import AccountMenu from "@/components/account/AccountMenu";
import SignUpForm from "@/components/auth/SignUpForm";
import { Container } from "@/components/MainLayout/Container";
import SignIn from "@/components/MainLayout/Header/SignIn";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { useAuth } from "@/hook/useAuth";
import { Lock, LogIn, ShieldX } from "lucide-react";
import Link from "next/link";
import React, { use } from "react";

export default function AccountLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user } = useAuth();
  if (!user) {
    // no access view
    return (
      <Container className="flex flex-col items-center justify-center min-h-[60vh] space-y-6">
        <div className="text-center space-y-4">
          <div className="mx-auto w-20 h-20 bg-red-100 rounded-full flex items-center justify-center">
            <ShieldX className="w-10 h-10 text-red-500" />
          </div>
          <h1 className="text-3xl font-bold text-gray-800">Access Denied</h1>
          <p className="text-gray-600 max-w-md">
            You need to be logged in to access your account dashboard. Please
            sign in to continue.
          </p>
        </div>

        <div className="flex flex-col sm:flex-row gap-3">
          {/* <Link
            href="/login"
            className="inline-flex items-center gap-2 px-6 py-3 bg-primary text-white rounded-lg hover:bg-primary/90 transition-colors"
          >
            <LogIn className="w-4 h-4" />
            Sign In
          </Link>
          <Link
            href="/"
            className="inline-flex items-center gap-2 px-6 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
          >
            Back to Home
          </Link> */}
          <SignIn />
        </div>

        <div className="text-sm text-gray-500">
          <Link href="/" className="text-primary hover:underline">
            Back to home
          </Link>
        </div>
      </Container>
    );
  }

  return (
    <Container className="flex flex-col space-y-5 my-10">
      <Breadcrumb />
      <div className="flex gap-2">
        <div className="w-1/4 hidden md:inline-flex">
          <AccountMenu />
        </div>
        <main className="w-full md:w-3/4">{children}</main>
      </div>
    </Container>
  );
}
