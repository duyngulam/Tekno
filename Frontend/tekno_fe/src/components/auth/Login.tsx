"use client";
import React, { useState } from "react";
import Input from "./Input";

export default function Login() {
  const [email, setEmail] = useState("test@example.com");
  const [password, setPassword] = useState("123456");

  return (
    <div>
      <h2 className="text-center font-semibold text-lg mb-4">
        Log in to Tekno
      </h2>

      {/* Inputs */}
      <form className="space-y-4">
        <Input
          label="E-mail"
          placeholder="E-mail"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <Input
          label="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </form>

      {/* Options */}
      <div className="flex items-center justify-between mt-3 text-sm">
        <label className="flex items-center gap-2">
          <input type="checkbox" className="checkbox checkbox-sm" />
          Keep me logged in
        </label>
        <a href="#" className="text-blue-600 hover:underline">
          Forgot Password?
        </a>
      </div>

      {/* Login button */}
      <button className="mt-4 w-full bg-yellow-400 hover:bg-yellow-500 text-white font-semibold py-2 rounded-md">
        Log In
      </button>

      {/* button */}
      <div className="flex flex-row gap-2 items-center mt-4">
        <hr className="border-1 border-gray w-full" />
        <p className="w-full text-center">Or login with</p>
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
        Don’t have an account?{" "}
        <a href="#" className="text-yellow-500 font-medium hover:underline">
          sign up
        </a>
      </p>
    </div>
  );
}
