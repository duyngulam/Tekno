"use client";
import { createContext, useContext, useState, useEffect } from "react";
import { loginApi } from "@/services/auth";
import { userInfo } from "os";

export interface User {
  id: number;
  email: string;
  role: string;
  token: string;
  expiresAt?: string;
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  hasRole: (role: string) => boolean;
  login: (email: string, password: string) => Promise<void | User>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    try {
      const saved = localStorage.getItem("user");
      if (saved) setUser(JSON.parse(saved));
    } catch {
      setUser(null);
    }
  }, []);

  // Auto logout khi token hết hạn
  useEffect(() => {
    if (!user?.expiresAt) return;

    const expireTime = new Date(user.expiresAt).getTime() - Date.now();

    if (expireTime <= 0) {
      logout();
      return;
    }

    const timer = setTimeout(() => {
      logout();
    }, expireTime);

    return () => clearTimeout(timer);
  }, [user]);

  const login = async (email: string, password: string) => {
    const res = await loginApi({ email, password });

    const userData = res.data;

    if (userData) {
      const userInfo: User = {
        id: userData.id,
        email: userData.email,
        role: userData.role,
        token: userData.token,
        expiresAt: userData.expiresAt,
      };

      setUser(userInfo);
      console.log("Login response:", user);
      localStorage.setItem("user", JSON.stringify(userInfo));
      localStorage.setItem("token", userData.token);

      return userInfo;
    }
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem("user");
    localStorage.removeItem("token");
  };

  const hasRole = (role: string) =>
    user?.role?.toLowerCase() === role.toLowerCase();

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        isAdmin: user?.role?.toLowerCase() === "admin",
        hasRole,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuthContext = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuthContext must be used within AuthProvider");
  return ctx;
};
