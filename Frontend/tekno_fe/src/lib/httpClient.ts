// src/lib/httpClient.ts

export class ApiError extends Error {
  status?: number;
  constructor(message: string, status?: number) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

export class HttpClient {
  private static instance: HttpClient | null = null;
  private baseUrl: string;

  private constructor() {
    this.baseUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";
  }

  public static getInstance(): HttpClient {
    if (!HttpClient.instance) {
      HttpClient.instance = new HttpClient();
    }
    return HttpClient.instance;
  }

  private getAuthToken(): string | null {
    if (typeof window !== "undefined") {
      return localStorage.getItem("token");
    }
    return null;
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const token = this.getAuthToken();
    const headers = new Headers(options.headers);

    // Default headers
    if (!headers.has("Content-Type") && !(options.body instanceof FormData)) {
      headers.set("Content-Type", "application/json");
    }

    if (token) {
      headers.set("Authorization", `Bearer ${token}`);
    }

    const url = endpoint.startsWith("http")
      ? endpoint
      : `${this.baseUrl}${endpoint.startsWith("/") ? endpoint : `/${endpoint}`}`;

    const config: RequestInit = {
      ...options,
      headers,
    };

    try {
      const res = await fetch(url, config);

      if (!res.ok) {
        let errorMessage = `HTTP error! status: ${res.status}`;
        try {
          const errData = await res.json();
          errorMessage = errData.message || errorMessage;
        } catch {
          // Fallback if response is not JSON
        }
        throw new ApiError(errorMessage, res.status);
      }

      // Check content-type to see if it is JSON
      const contentType = res.headers.get("content-type") || "";
      if (contentType.includes("application/json")) {
        const result = await res.json();
        
        // If it follows the standard ApiResponse wrapper { success, data, message }
        if (result && typeof result === "object" && "success" in result) {
          if (!result.success) {
            throw new ApiError(result.message || "API execution failed", res.status);
          }
          return result.data !== undefined ? result.data : result;
        }
        return result;
      }

      const textData = await res.text();
      return textData as unknown as T;
    } catch (error: unknown) {
      if (error instanceof ApiError) {
        throw error;
      }
      const err = error as any;
      throw new ApiError(err?.message || "Connection error. Failed to reach server.");
    }
  }

  public async get<T>(endpoint: string, options?: RequestInit): Promise<T> {
    return this.request<T>(endpoint, { method: "GET", ...options });
  }

  public async post<T>(
    endpoint: string,
    body?: unknown,
    options?: RequestInit
  ): Promise<T> {
    const isFormData = typeof FormData !== "undefined" && body instanceof FormData;
    return this.request<T>(endpoint, {
      method: "POST",
      body: isFormData ? (body as FormData) : JSON.stringify(body),
      ...options,
    });
  }

  public async put<T>(
    endpoint: string,
    body?: unknown,
    options?: RequestInit
  ): Promise<T> {
    const isFormData = typeof FormData !== "undefined" && body instanceof FormData;
    return this.request<T>(endpoint, {
      method: "PUT",
      body: isFormData ? (body as FormData) : JSON.stringify(body),
      ...options,
    });
  }

  public async patch<T>(
    endpoint: string,
    body?: unknown,
    options?: RequestInit
  ): Promise<T> {
    const isFormData = typeof FormData !== "undefined" && body instanceof FormData;
    return this.request<T>(endpoint, {
      method: "PATCH",
      body: isFormData ? (body as FormData) : JSON.stringify(body),
      ...options,
    });
  }

  public async del<T>(endpoint: string, options?: RequestInit): Promise<T> {
    return this.request<T>(endpoint, { method: "DELETE", ...options });
  }
}

export const httpClient = HttpClient.getInstance();
