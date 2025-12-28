import { get, post, put, del, API_BASE } from "@/lib/api";

// Types
export type OrderStatus = 
  | "Pending" 
  | "Processing" 
  | "Shipped" 
  | "Delivered" 
  | "Cancelled";

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
  status?: OrderStatus;
  keyword?: string;
  sortBy?: string;
  sortOrder?: "asc" | "desc";
};

// Get all orders (admin view)
export async function getAdminOrders(params?: OrdersListParams) {
  try {
    const query = new URLSearchParams();

    if (params?.page) query.append("Page", String(params.page));
    if (params?.pageSize) query.append("PageSize", String(params.pageSize));
    if (params?.status) query.append("Status", params.status);
    if (params?.keyword) query.append("Keyword", params.keyword);
    if (params?.sortBy) query.append("SortBy", params.sortBy);
    if (params?.sortOrder) query.append("SortOrder", params.sortOrder);

    const url = `/api/admin/orders${query.toString() ? `?${query.toString()}` : ""}`;
    
    return await get(url, { cache: "no-store" });
  } catch (error) {
    console.error("❌ Failed to load admin orders:", error);
    throw error;
  }
}

// Get order details by ID (admin view)
export async function getAdminOrder(orderId: number | string) {
  try {
    return await get(`/api/admin/orders/${orderId}`, { cache: "no-store" });
  } catch (error) {
    console.error("❌ Failed to load admin order:", error);
    throw error;
  }
}

// Cancel an order
export async function cancelOrder(orderId: number | string, reason?: string) {
  try {
    return await post(`/api/admin/orders/${orderId}/cancel`, {
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
    return await post(`/api/admin/orders/${orderId}/deliver`, {});
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
    return await post(`/api/admin/orders/${orderId}/ship`, {
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
    return await put(`/api/admin/orders/${orderId}/status`, {
      status,
      notes
    });
  } catch (error) {
    console.error("❌ Failed to update order status:", error);
    throw error;
  }
}