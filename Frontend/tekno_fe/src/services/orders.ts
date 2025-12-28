import { get, post, put, del, API_BASE } from "@/lib/api";

// Types - Updated with numeric status
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

export type OrderItem = {
  id: number;
  orderId: number;
  productId: number;
  productName: string;
  variantId?: number;
  variantSku?: string;
  quantity: number;
  price: number;
  subtotal: number;
  imageUrl?: string;
};

export type Order = {
  id: number;
  orderNumber: string;
  userId: number;
  userName?: string;
  userEmail?: string;
  status: OrderStatus;
  totalAmount: number;
  shippingAddress: string;
  shippingCity?: string;
  shippingDistrict?: string;
  shippingWard?: string;
  phoneNumber: string;
  paymentMethod?: string;
  paymentStatus?: string;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
  items?: OrderItem[];
  [key: string]: any;
};

export type OrdersListParams = {
  page?: number;
  pageSize?: number;
  status?: OrderStatus | null; // null = all orders
  keyword?: string; // Search by order number or user ID
  startDate?: string; // Date range filter
  endDate?: string;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
};

export type PaginatedResponse<T> = {
  data: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

// Get all orders (admin view) with pagination
export async function getAdminOrders(params?: OrdersListParams) {
  try {
    const query = new URLSearchParams();

    if (params?.page) query.append("Page", String(params.page));
    if (params?.pageSize) query.append("PageSize", String(params.pageSize));
    
    // Status: null means all orders, otherwise use numeric value
    if (params?.status !== undefined && params?.status !== null) {
      query.append("Status", String(params.status));
    }
    
    // Keyword for order number or user ID
    if (params?.keyword) query.append("search", params.keyword);
    
    // Date range
    if (params?.startDate) query.append("StartDate", params.startDate);
    if (params?.endDate) query.append("EndDate", params.endDate);
    
    if (params?.sortBy) query.append("SortBy", params.sortBy);
    if (params?.sortOrder) query.append("SortOrder", params.sortOrder);

    const url = `${API_BASE}/admin/orders${query.toString() ? `?${query.toString()}` : ""}`;
    
    return await get(url, { cache: "no-store" });
  } catch (error) {
    console.error("❌ Failed to load admin orders:", error);
    throw error;
  }
}

// Get order details by ID (admin view)
export async function getAdminOrder(orderId: number | string) {
  try {
    return await get(`${API_BASE}/admin/orders/${orderId}`, { cache: "no-store" });
  } catch (error) {
    console.error("❌ Failed to load admin order:", error);
    throw error;
  }
}

// Cancel an order
export async function cancelOrder(orderId: number | string, reason?: string) {
  try {
    return await post(`${API_BASE}/admin/orders/${orderId}/cancel`, {
      reason: reason || "Cancelled by admin"
    });
  } catch (error) {
    console.error("❌ Failed to cancel order:", error);
    throw error;
  }
}

// Mark order as delivered
export async function deliverOrder(orderId: number | string) {
  try {
    return await post(`${API_BASE}/admin/orders/${orderId}/deliver`, {});
  } catch (error) {
    console.error("❌ Failed to deliver order:", error);
    throw error;
  }
}

// Ship an order
export async function shipOrder(
  orderId: number | string, 
  trackingNumber?: string,
  carrier?: string
) {
  try {
    return await post(`${API_BASE}/admin/orders/${orderId}/ship`, {
      trackingNumber,
      carrier
    });
  } catch (error) {
    console.error("❌ Failed to ship order:", error);
    throw error;
  }
}

// Update order status (generic)
export async function updateOrderStatus(
  orderId: number | string,
  status: OrderStatus,
  notes?: string
) {
  try {
    return await put(`${API_BASE}/admin/orders/${orderId}/status`, {
      status,
      notes
    });
  } catch (error) {
    console.error("❌ Failed to update order status:", error);
    throw error;
  }
}