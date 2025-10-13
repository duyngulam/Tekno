"use client";
import { createContext, useContext, useState, useEffect } from "react";
import { loginApi, getCurrentUserApi } from "@/api/auth";
//import { loginApi, logoutApi, getCurrentUserApi } from "@/api/auth";

export interface User {
  id: string;
  username: string;
  email: string;
  role: "user" | "admin" | string;
}

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  hasRole: (role: string) => boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);

  // 🧭 Lấy user khi mở lại tab hoặc reload
  useEffect(() => {
    (async () => {
      try {
        const saved = localStorage.getItem("user");
        if (saved) setUser(JSON.parse(saved));
        else {
          const data = await getCurrentUserApi();
          setUser(data.user);
          localStorage.setItem("user", JSON.stringify(data.user));
        }
      } catch {
        setUser(null);
      }
    })();
  }, []);

  // 🧩 Đăng nhập
  const login = async (email: string, password: string) => {
    const data = await loginApi({ email, password });
    setUser(data.user);
    localStorage.setItem("user", JSON.stringify(data.user));
  };

  //   🧩 Đăng xuất
  const logout = async () => {
    //     try {
    //       await logoutApi();
    //     } finally {
    //       setUser(null);
    //       localStorage.removeItem("user");
    //     }
  };

  const hasRole = (role: string) => user?.role === role;

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: !!user,
        isAdmin: user?.role === "admin",
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
