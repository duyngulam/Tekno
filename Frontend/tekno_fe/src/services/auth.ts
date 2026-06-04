// src/services/auth.ts
import { httpClient } from "@/lib/httpClient";

export class AuthService {
  private static instance: AuthService | null = null;

  private constructor() {}

  public static getInstance(): AuthService {
    if (!AuthService.instance) {
      AuthService.instance = new AuthService();
    }
    return AuthService.instance;
  }

  public async signup(data: {
    username: string;
    email: string;
    password: string;
    role: string;
  }) {
    return httpClient.post<any>("/auth/register", data);
  }

  public async login(data: { email: string; password: string }) {
    return httpClient.post<any>("/auth/login", data);
  }
}

export const authService = AuthService.getInstance();