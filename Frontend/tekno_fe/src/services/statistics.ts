import { API_BASE_URL } from "@/lib/apiConfig";
import { ApiResponse } from "@/type/share";

// Types
export type DatePeriod =
  | "Today"
  | "Yesterday"
  | "Last7Days"
  | "Last30Days"
  | "ThisWeek"
  | "LastWeek"
  | "ThisMonth"
  | "LastMonth"
  | "ThisQuarter"
  | "LastQuarter"
  | "ThisYear"
  | "LastYear"
  | "Custom";

export interface StatisticsOverview {
  totalRevenue: number;
  totalOrders: number;
  totalCustomers: number;
  averageOrderValue: number;
  revenueGrowth?: number;
  ordersGrowth?: number;
}

export interface TopProduct {
  id: number;
  name: string;
  totalSold: number;
  totalRevenue: number;
  imageUrl?: string;
}

export interface RevenueByCategory {
  categoryId: number;
  categoryName: string;
  totalRevenue: number;
  percentage: number;
}

export interface TopCustomer {
  id: number;
  name: string;
  email: string;
  totalOrders: number;
  totalSpent: number;
}

export interface RevenueChartData {
  date: string;
  revenue: number;
  orders: number;
}

export interface RecentOrder {
  id: number;
  orderDate: string;
  customerName: string;
  totalAmount: number;
  status: string;
}

export interface ProductPerformance {
  totalProducts: number;
  activeProducts: number;
  outOfStock: number;
  lowStock: number;
}

export interface StatisticsResponse {
  overview: StatisticsOverview;
  topProducts: TopProduct[];
  revenueByCategory: RevenueByCategory[];
  topCustomers: TopCustomer[];
  revenueChart: RevenueChartData[];
  recentOrders: RecentOrder[];
  productPerformance: ProductPerformance;
  period: DatePeriod;
  startDate?: string;
  endDate?: string;
}

export interface FilterOptions {
  periods: DatePeriod[];
  defaultPeriod: DatePeriod;
}

export interface GetStatisticsParams {
  period?: DatePeriod;
  startDate?: string;
  endDate?: string;
  topCount?: number;
}

// Custom error class for better error handling
export class StatisticsError extends Error {
  constructor(
    public message: string,
    public status?: number,
    public details?: any
  ) {
    super(message);
    this.name = "StatisticsError";
  }
}

// GET /api/admin/statistics
export async function getAdminStatistics(
  token: string,
  params?: GetStatisticsParams
): Promise<ApiResponse<StatisticsResponse>> {
  const queryParams = new URLSearchParams();

  // Validate period
  const validPeriods: DatePeriod[] = [
    "Today", "Yesterday", "Last7Days", "Last30Days",
    "ThisWeek", "LastWeek", "ThisMonth", "LastMonth",
    "ThisQuarter", "LastQuarter", "ThisYear", "LastYear", "Custom"
  ];

  if (params?.period && validPeriods.includes(params.period)) {
    queryParams.append("period", params.period);
  } else {
    queryParams.append("period", "Last30Days");
  }

  // Validate và append startDate
  if (params?.startDate && params.startDate.trim()) {
    queryParams.append("startDate", params.startDate);
  }

  // Validate và append endDate
  if (params?.endDate && params.endDate.trim()) {
    queryParams.append("endDate", params.endDate);
  }

  // Validate topCount - LUÔN GỬI với giá trị mặc định 10 nếu không được cung cấp
  const topCount = params?.topCount ? Math.max(5, Math.min(50, params.topCount)) : 10;
  queryParams.append("topCount", topCount.toString());

  const url = `${API_BASE_URL}/admin/statistics${
    queryParams.toString() ? `?${queryParams.toString()}` : ""
  }`;

  console.log("[Statistics API] Request Details:");
  console.log("  URL:", url);
  console.log("  Query Params:", {
    period: queryParams.get("period"),
    startDate: queryParams.get("startDate"),
    endDate: queryParams.get("endDate"),
    topCount: queryParams.get("topCount"),
  });
  console.log("  Token exists:", !!token);

  try {
    const res = await fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      cache: "no-store",
    });

    console.log("[Statistics API] Response status:", res.status, res.statusText);

    if (!res.ok) {
      let errorData;
      try {
        errorData = await res.json();
      } catch {
        errorData = await res.text();
      }
      console.error("[Statistics API] Error response:", {
        status: res.status,
        statusText: res.statusText,
        body: errorData,
        url: url,
      });
      throw new StatisticsError(
        `Failed to fetch statistics: ${res.statusText}`,
        res.status,
        errorData
      );
    }

    const json = await res.json();
    console.log("[Statistics API] Success:", json);
    return json as ApiResponse<StatisticsResponse>;
  } catch (error) {
    console.error("[Statistics API] Fetch error:", error);
    throw error;
  }
}

// GET /api/admin/statistics/filters
export async function getStatisticsFilters(
  token: string
): Promise<ApiResponse<FilterOptions>> {
  try {
    const res = await fetch(`${API_BASE_URL}/admin/statistics/filters`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      cache: "no-store",
    });

    if (!res.ok) {
      let errorData;
      try {
        errorData = await res.json();
      } catch {
        errorData = await res.text();
      }
      throw new StatisticsError(
        `Failed to fetch filters: ${res.statusText}`,
        res.status,
        errorData
      );
    }

    const json = await res.json();
    return json as ApiResponse<FilterOptions>;
  } catch (error) {
    console.error("[Statistics Filters API] Error:", error);
    throw error;
  }
}

// POST /api/admin/statistics/invalidate-cache
export async function invalidateStatisticsCache(
  token: string
): Promise<ApiResponse<boolean>> {
  try {
    const res = await fetch(
      `${API_BASE_URL}/admin/statistics/invalidate-cache`,
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      }
    );

    if (!res.ok) {
      let errorData;
      try {
        errorData = await res.json();
      } catch {
        errorData = await res.text();
      }
      throw new StatisticsError(
        `Failed to invalidate cache: ${res.statusText}`,
        res.status,
        errorData
      );
    }

    const json = await res.json();
    return json as ApiResponse<boolean>;
  } catch (error) {
    console.error("[Invalidate Cache API] Error:", error);
    throw error;
  }
}

// GET /api/admin/statistics/overview
export async function getStatisticsOverview(
  token: string,
  params?: GetStatisticsParams
): Promise<ApiResponse<StatisticsOverview>> {
  const queryParams = new URLSearchParams();

  if (params?.period) {
    queryParams.append("period", params.period);
  }

  if (params?.startDate) {
    queryParams.append("startDate", params.startDate);
  }

  if (params?.endDate) {
    queryParams.append("endDate", params.endDate);
  }

  const url = `${API_BASE_URL}/admin/statistics/overview${
    queryParams.toString() ? `?${queryParams.toString()}` : ""
  }`;

  try {
    const res = await fetch(url, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      cache: "no-store",
    });

    if (!res.ok) {
      let errorData;
      try {
        errorData = await res.json();
      } catch {
        errorData = await res.text();
      }
      throw new StatisticsError(
        `Failed to fetch overview: ${res.statusText}`,
        res.status,
        errorData
      );
    }

    const json = await res.json();
    return json as ApiResponse<StatisticsOverview>;
  } catch (error) {
    console.error("[Statistics Overview API] Error:", error);
    throw error;
  }
}

// GET /api/admin/statistics/revenue/last-days/{numberOfDays}
export async function getRevenueLastDays(
  token: string,
  numberOfDays: number
): Promise<ApiResponse<RevenueChartData[]>> {
  try {
    const res = await fetch(
      `${API_BASE_URL}/admin/statistics/revenue/last-days/${numberOfDays}`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        cache: "no-store",
      }
    );

    if (!res.ok) {
      let errorData;
      try {
        errorData = await res.json();
      } catch {
        errorData = await res.text();
      }
      throw new StatisticsError(
        `Failed to fetch revenue: ${res.statusText}`,
        res.status,
        errorData
      );
    }

    const json = await res.json();
    return json as ApiResponse<RevenueChartData[]>;
  } catch (error) {
    console.error("[Revenue Last Days API] Error:", error);
    throw error;
  }
}

// GET /api/admin/statistics/revenue/last-weeks/{numberOfWeeks}
export async function getRevenueLastWeeks(
  token: string,
  numberOfWeeks: number
): Promise<ApiResponse<RevenueChartData[]>> {
  try {
    const res = await fetch(
      `${API_BASE_URL}/admin/statistics/revenue/last-weeks/${numberOfWeeks}`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        cache: "no-store",
      }
    );

    if (!res.ok) {
      let errorData;
      try {
        errorData = await res.json();
      } catch {
        errorData = await res.text();
      }
      throw new StatisticsError(
        `Failed to fetch revenue: ${res.statusText}`,
        res.status,
        errorData
      );
    }

    const json = await res.json();
    return json as ApiResponse<RevenueChartData[]>;
  } catch (error) {
    console.error("[Revenue Last Weeks API] Error:", error);
    throw error;
  }
}

// GET /api/admin/statistics/revenue/last-months/{numberOfMonths}
export async function getRevenueLastMonths(
  token: string,
  numberOfMonths: number
): Promise<ApiResponse<RevenueChartData[]>> {
  try {
    const res = await fetch(
      `${API_BASE_URL}/admin/statistics/revenue/last-months/${numberOfMonths}`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        cache: "no-store",
      }
    );

    if (!res.ok) {
      let errorData;
      try {
        errorData = await res.json();
      } catch {
        errorData = await res.text();
      }
      throw new StatisticsError(
        `Failed to fetch revenue: ${res.statusText}`,
        res.status,
        errorData
      );
    }

    const json = await res.json();
    return json as ApiResponse<RevenueChartData[]>;
  } catch (error) {
    console.error("[Revenue Last Months API] Error:", error);
    throw error;
  }
}