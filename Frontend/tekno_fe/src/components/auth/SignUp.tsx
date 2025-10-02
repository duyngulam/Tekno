"use client";
import React, { useState } from "react";
import Input from "./Input";

export default function Login() {
  return (
    <div>
      <h2 className="text-center font-semibold text-lg mb-4">
        Create your account
      </h2>

      {/* Inputs */}
      <div className="space-y-4">
        <Input label="E-mail" placeholder="E-mail" />
        <Input label="Password" type="password" />
        <Input label="Password" type="password" />
      </div>

      {/* Options */}
      <div className="flex items-center justify-between mt-3 text-sm">
        <label className="flex items-center gap-2">
          <input type="checkbox" className="checkbox checkbox-sm" />I agree to
          the Terms of Service and Privacy Policy
        </label>
      </div>

      {/* Login button */}
      <button className="mt-4 w-full bg-yellow-400 hover:bg-yellow-500 text-white font-semibold py-2 rounded-md">
        Create Account
      </button>

      {/* button */}
      <div className="flex flex-row gap-2 items-center mt-4">
        <hr className="border-1 border-gray w-full" />
        <p className="w-full text-center">Or Sign up with</p>
        <hr className="border-1 border-gray w-full" />
      </div>
      <div className="flex flex-row gap-2">
        <button className="mt-4 w-full bg-none hover:bg-gray-300 text-secondary border-secondary border-1 font-semibold py-2 rounded-md flex items-center justify-center gap-2">
          Facebook
        </button>
        <button className="mt-4 w-full bg-none hover:bg-gray-300 text-secondary border-secondary border-1 font-semibold py-2 rounded-md flex items-center justify-center gap-2">
          Google
        </button>
      </div>

      {/* Footer */}
      <p className="text-center text-sm mt-4 text-gray-500">
        Already have an account?{" "}
        <a href="#" className="text-yellow-500 font-medium hover:underline">
          Login
        </a>
      </p>
    </div>
  );
}
