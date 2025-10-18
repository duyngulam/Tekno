"use client";
import { useAuthContext } from "@/context/AuthContext";

export function useAuth() {
  const { user, isAuthenticated, isAdmin, hasRole, login, logout } =
    useAuthContext();
  return { user, isAuthenticated, isAdmin, hasRole, login, logout };
}
