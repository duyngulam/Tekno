import React from "react";
import clsx from "clsx";

interface InputProps {
  label: string;
  name: string;
  placeholder?: string;
  type?: string;
  leftIcon?: React.ReactNode;
  rightIcon?: React.ReactNode;
  supportingText?: string;
  error?: boolean;
  disabled?: boolean;
  value?: string;
  onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

export default function Input({
  label,
  name,
  placeholder,
  type = "text",
  leftIcon,
  rightIcon,
  supportingText,
  error = false,
  disabled = false,
  value,
  onChange,
}: InputProps) {
  return (
    <div className="w-full">
      <div
        className={clsx(
          "relative group flex items-center gap-2 rounded-md border px-3 py-2 transition-colors",
          {
            // trạng thái bình thường
            "border-gray-300 hover:border-primary focus-within:border-primary":
              !error && !disabled,
            // trạng thái lỗi
            "border-red-500 hover:border-red-600 focus-within:border-red-600":
              error && !disabled,
            // trạng thái disabled
            "bg-gray-100 border-gray-200 cursor-not-allowed opacity-70":
              disabled,
          }
        )}
      >
        {/* Label */}
        <label
          className={clsx(
            "absolute -top-2 left-3 bg-white px-1 text-xs transition-colors",
            {
              "text-gray-500 group-hover:text-primary group-focus-within:text-primary":
                !error && !disabled,
              "text-red-500": error,
              "text-gray-400": disabled,
            }
          )}
        >
          {label}
        </label>

        {/* Left icon */}
        {leftIcon && (
          <span
            className={clsx("text-gray-400", {
              "text-red-500": error,
              "text-gray-300": disabled,
            })}
          >
            {leftIcon}
          </span>
        )}

        {/* Input field */}
        <input
          name={name}
          type={type}
          placeholder={placeholder}
          disabled={disabled}
          className={clsx(
            "flex-1 bg-transparent outline-none placeholder-gray-400 text-gray-700",
            {
              "placeholder-red-400 text-red-600": error,
              "text-gray-400 placeholder-gray-300 cursor-not-allowed": disabled,
            }
          )}
        />

        {/* Right icon */}
        {rightIcon && (
          <span
            className={clsx("text-gray-400 cursor-pointer", {
              "text-red-500": error,
              "text-gray-300 cursor-not-allowed": disabled,
            })}
          >
            {rightIcon}
          </span>
        )}
      </div>

      {/* Supporting text */}
      {supportingText && (
        <p
          className={clsx("mt-1 text-xs", {
            "text-gray-500": !error && !disabled,
            "text-red-500": error,
            "text-gray-400": disabled,
          })}
        >
          {supportingText}
        </p>
      )}
    </div>
  );
}
