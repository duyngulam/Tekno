// src/components/admin/AdminHeader.tsx
"use client";

import Image from "next/image";
import logo from "@/asset/MainLogo.png";
import { Search, User } from "lucide-react";
import { useAuth } from "@/hook/useAuth";

const AdminHeader = () => {
  const { user } = useAuth();

  return (
    <header className="bg-white border-b border-secondary px-6 h-16 flex items-center justify-between">
      {/* Logo và Text */}
      <div className="flex items-center gap-3">
        <Image src={logo} alt="Tekno Logo" className="w-10 h-10" />
        <span className="text-2xl font-bold text-primary">Tekno</span>
      </div>

      {/* Right side - Search và User info */}
      <div className="flex items-center gap-6">
        {/* Search Icon */}
        <button className="hover:text-primary transition">
          <Search size={20} className="text-gray-600" />
        </button>

        {/* User Info */}
        <div className="flex items-center gap-2">
          <User size={20} className="text-gray-600" />
          <span className="text-gray-700">
            Hello, <span className="text-primary font-semibold">admin</span>
          </span>
        </div>
      </div>
    </header>
  );
};

export default AdminHeader;