"use client";

import React from "react";
import { X, XCircle } from "lucide-react";

type Props = {
  title?: string;
  message?: string;
  onRetry?: () => void;
  onClose?: () => void;
};

export default function False({
  title = "Payment Failed",
  message = "Unfortunately we have an issue with your payment, try again later.",
  onRetry,
  onClose,
}: Props) {
  return (
    <div className=" flex items-center justify-center">
      <div className=" bg-black/30" onClick={onClose} />
      <div className="z-10 w-full max-w-md rounded-2xl bg-white p-6 md:p-8 shadow-2xl">
        <button
          aria-label="Close"
          onClick={onClose}
          className=" right-3 top-3 rounded-full p-1 hover:bg-gray-100"
        >
          <X className="h-4 w-4 text-gray-500" />
        </button>

        <div className="flex flex-col items-center text-center gap-4">
          <XCircle className="h-14 w-14 text-red-500" />
          <h3 className="text-2xl font-semibold text-gray-800">{title}</h3>
          <p className="text-sm text-gray-600 max-w-sm">{message}</p>

          <button
            onClick={onRetry}
            className="mt-2 w-full rounded-lg bg-yellow-400 px-4 py-2 font-medium text-white hover:bg-yellow-500"
          >
            Try again
          </button>
        </div>
      </div>
    </div>
  );
}
