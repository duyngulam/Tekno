// src/services/order.ts
import { httpClient } from "@/lib/httpClient";
import { CreateOrderRequest, CreateOrderResponse, Order, OrderHistoryResponse } from "@/type/order";
import { ApiResponse } from "@/type/share";

export enum OrderStatus {
  Pending = 1,
  Processing = 2,
  Shipping = 4,
  Delivered = 5,
  Cancelled = 6,
  RefundRequested = 7,
  Refunded = 8,
}

export const OrderStatusLabels: Record<OrderStatus, string> = {
  [OrderStatus.Pending]: "Pending",
  [OrderStatus.Processing]: "Processing",
  [OrderStatus.Shipping]: "Shipping",
  [OrderStatus.Delivered]: "Delivered",
  [OrderStatus.Cancelled]: "Cancelled",
  [OrderStatus.RefundRequested]: "Refund Requested",
  [OrderStatus.Refunded]: "Refunded",
};

export type OrdersListParams = {
  page?: number;
  pageSize?: number;
  status?: OrderStatus | null;
  keyword?: string;
  startDate?: string;
  endDate?: string;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
};

export class OrderService {
  private static instance: OrderService | null = null;

  private constructor() {}

  public static getInstance(): OrderService {
    if (!OrderService.instance) {
      OrderService.instance = new OrderService();
    }
    return OrderService.instance;
  }

  // Client operations
  public async fetchOrderHistory(
    status?: number,
    page = 1,
    pageSize = 20
  ): Promise<OrderHistoryResponse> {
    const searchParams = new URLSearchParams();
    if (status !== undefined) {
      searchParams.append("status", status.toString());
    }
    if (page) {
      searchParams.append("page", page.toString());
    }
    if (pageSize) {
      searchParams.append("pageSize", pageSize.toString());
    }

    return httpClient.get<OrderHistoryResponse>(
      `/orders/history?${searchParams.toString()}`,
      { cache: "no-store" }
    );
  }

  public async getOrderByOrderId(orderId: number): Promise<Order> {
    const json = await httpClient.get<any>(`/orders/by-id/${orderId}`, {
      cache: "no-store",
    });
    return (json?.data ?? json) as Order;
  }

  public async getOrderByOrderNumber(orderNumber: string): Promise<Order> {
    const json = await httpClient.get<any>(`/orders/${orderNumber}`, {
      cache: "no-store",
    });
    return (json?.data ?? json) as Order;
  }

  public async createOrder(
    payload: CreateOrderRequest
  ): Promise<CreateOrderResponse> {
    return httpClient.post<CreateOrderResponse>("/orders/create", payload);
  }

  // Admin operations
  public async getAdminOrders(params?: OrdersListParams) {
    const query = new URLSearchParams();
    if (params?.page) query.append("Page", String(params.page));
    if (params?.pageSize) query.append("PageSize", String(params.pageSize));
    if (params?.status !== undefined && params?.status !== null) {
      query.append("Status", String(params.status));
    }
    if (params?.keyword) query.append("search", params.keyword);
    if (params?.startDate) query.append("StartDate", params.startDate);
    if (params?.endDate) query.append("EndDate", params.endDate);
    if (params?.sortBy) query.append("SortBy", params.sortBy);
    if (params?.sortOrder) query.append("SortOrder", params.sortOrder);

    const queryString = query.toString();
    const endpoint = `/admin/orders${queryString ? `?${queryString}` : ""}`;
    return httpClient.get<any>(endpoint, { cache: "no-store" });
  }

  public async getAdminOrder(orderId: number | string) {
    return httpClient.get<any>(`/admin/orders/${orderId}`, {
      cache: "no-store",
    });
  }

  public async cancelOrder(orderId: number | string, reason?: string) {
    return httpClient.post<any>(`/admin/orders/${orderId}/cancel`, {
      reason: reason || "Cancelled by admin",
    });
  }

  public async deliverOrder(orderId: number | string) {
    return httpClient.post<any>(`/admin/orders/${orderId}/deliver`, {});
  }

  public async shipOrder(
    orderId: number | string,
    trackingNumber?: string,
    carrier?: string
  ) {
    return httpClient.post<any>(`/admin/orders/${orderId}/ship`, {
      trackingNumber,
      carrier,
    });
  }

  public async updateOrderStatus(
    orderId: number | string,
    status: OrderStatus,
    notes?: string
  ) {
    return httpClient.put<any>(`/admin/orders/${orderId}/status`, {
      status,
      notes,
    });
  }
}

export const orderService = OrderService.getInstance();

// Backward compatibility exports
export const fetchOrderHistory = (
  arg1?: number | string,
  arg2?: number,
  arg3?: number,
  arg4?: string
) => {
  if (typeof arg1 === "string" || typeof arg4 === "string") {
    // Legacy call (status, page, pageSize, accessToken)
    return orderService.fetchOrderHistory(
      typeof arg1 === "number" ? arg1 : undefined,
      arg2,
      arg3
    );
  }
  return orderService.fetchOrderHistory(arg1, arg2, arg3);
};

export const getOrderByOrderId = (arg1: string | number, arg2?: number) => {
  if (typeof arg1 === "string") {
    return orderService.getOrderByOrderId(arg2!);
  }
  return orderService.getOrderByOrderId(arg1);
};

export const getOrderByOrderNumber = (
  arg1: string,
  arg2?: string
) => {
  if (arg2 !== undefined) {
    return orderService.getOrderByOrderNumber(arg2);
  }
  return orderService.getOrderByOrderNumber(arg1);
};

export const createOrder = (
  payload: CreateOrderRequest,
  token?: string
) => orderService.createOrder(payload);

export const getAdminOrders = (params?: OrdersListParams) => orderService.getAdminOrders(params);
export const getAdminOrder = (orderId: number | string) => orderService.getAdminOrder(orderId);
export const cancelOrder = (orderId: number | string, reason?: string) => orderService.cancelOrder(orderId, reason);
export const deliverOrder = (orderId: number | string) => orderService.deliverOrder(orderId);
export const shipOrder = (orderId: number | string, trackingNumber?: string, carrier?: string) => orderService.shipOrder(orderId, trackingNumber, carrier);
export const updateOrderStatus = (orderId: number | string, status: OrderStatus, notes?: string) => orderService.updateOrderStatus(orderId, status, notes);