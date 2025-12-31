"use client";

import React, { useEffect, useState } from "react";
import { X, Package, Truck, CheckCircle, XCircle, Clock, CreditCard, MapPin, User } from "lucide-react";
import { Button } from "@/components/ui/button";
import { getAdminOrder, OrderStatus, OrderStatusLabels } from "@/services/orders";

type OrderDetailProps = {
  orderId: number;
  onClose: () => void;
  onActionComplete?: () => void; // Callback after cancel/ship/deliver
};

type OrderDetailData = {
  id: number;
  orderNumber: string;
  userId: number;
  userEmail: string;
  userPhone: string | null;
  status: OrderStatus;
  statusName: string;
  totalAmount: number;
  createdAt: string;
  completedAt: string | null;
  shippedAt: string | null;
  deliveredAt: string | null;
  trackingNumber: string | null;
  shippingCarrier: string | null;
  customerNote: string | null;
  shippingAddress: {
  fullName?: string;
  phone?: string;
  addressLine?: string;
  ward?: string;
  district?: string;
  province?: string;
} | string | null;
  items: Array<{
    id: number;
    quantity: number;
    price: number;
    totalPrice: number;
    product: {
      id: number;
      brandName: string;
      categoryName: string;
      name: string;
      slug: string;
      basePrice: number;
      discountPercent: number;
      finalPrice: number;
      overview: string;
      totalSold: number;
      averageRating: number;
      totalReviews: number;
      createdAt: string;
      primaryImagePath: string;
    };
    variant: {
      id: number;
      sku: string;
      price: number;
      stock: number;
      attributes: Array<{
        name: string;
        value: string;
      }>;
    };
  }>;
  payment: {
    method: string;
    status: string;
    transactionId?: string;
    paidAt?: string;
  } | null;
};

export default function OrderDetail({ orderId, onClose, onActionComplete }: OrderDetailProps) {
  const [order, setOrder] = useState<OrderDetailData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadOrderDetail();
  }, [orderId]);

  const loadOrderDetail = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getAdminOrder(orderId);
      const data = response?.data || response;

      console.log("📦 Order Detail Response:", data);
      setOrder(data);
    } catch (err) {
      console.error("Failed to load order detail:", err);
      setError(err instanceof Error ? err.message : "Failed to load order details");
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status: OrderStatus) => {
    switch (status) {
      case OrderStatus.Pending:
        return "bg-yellow-100 text-yellow-800 border-yellow-300";
      case OrderStatus.Processing:
        return "bg-blue-100 text-blue-800 border-blue-300";
      case OrderStatus.Shipping:
        return "bg-purple-100 text-purple-800 border-purple-300";
      case OrderStatus.Delivered:
        return "bg-green-100 text-green-800 border-green-300";
      case OrderStatus.Cancelled:
        return "bg-red-100 text-red-800 border-red-300";
      case OrderStatus.RefundRequested:
        return "bg-orange-100 text-orange-800 border-orange-300";
      case OrderStatus.Refunded:
        return "bg-gray-100 text-gray-800 border-gray-300";
      default:
        return "bg-gray-100 text-gray-800 border-gray-300";
    }
  };

  const getStatusIcon = (status: OrderStatus) => {
    switch (status) {
      case OrderStatus.Pending:
        return <Clock className="w-5 h-5" />;
      case OrderStatus.Processing:
        return <Package className="w-5 h-5" />;
      case OrderStatus.Shipping:
        return <Truck className="w-5 h-5" />;
      case OrderStatus.Delivered:
        return <CheckCircle className="w-5 h-5" />;
      case OrderStatus.Cancelled:
        return <XCircle className="w-5 h-5" />;
      default:
        return <Package className="w-5 h-5" />;
    }
  };

  const formatDate = (dateString: string | null) => {
    if (!dateString) return "N/A";
    return new Date(dateString).toLocaleString("vi-VN", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const formatAddress = (address: any) => {
  if (!address) return "No shipping address provided";
  if (typeof address === "string") return address;
  
  const parts = [];
  if (address.addressLine) parts.push(address.addressLine);
  if (address.ward) parts.push(address.ward);
  if (address.district) parts.push(address.district);
  if (address.province) parts.push(address.province);
  
  return parts.length > 0 ? parts.join(", ") : "No address details";
};

  const formatCurrency = (amount: number) => {
    return amount.toLocaleString("vi-VN") + "đ";
  };

  if (loading) {
    return (
      <div className="fixed inset-0 bg-black/60 flex justify-center items-center z-50">
        <div className="bg-white p-8 rounded-lg shadow-xl">
          <div className="flex items-center gap-3">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            <p className="text-gray-700">Loading order details...</p>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="fixed inset-0 bg-black/60 flex justify-center items-center z-50">
        <div className="bg-white p-6 rounded-lg shadow-xl max-w-md">
          <div className="flex justify-between items-center mb-4">
            <h3 className="text-xl font-semibold text-red-600">Error</h3>
            <button onClick={onClose} className="text-gray-500 hover:text-gray-700">
              <X size={24} />
            </button>
          </div>
          <p className="text-gray-700 mb-4">{error}</p>
          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={onClose}>
              Close
            </Button>
            <Button onClick={loadOrderDetail}>Retry</Button>
          </div>
        </div>
      </div>
    );
  }

  if (!order) return null;

  return (
    <div className="fixed inset-0 bg-black/60 flex justify-center items-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-5xl max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="sticky top-0 bg-white border-b px-6 py-4 flex justify-between items-center">
          <div>
            <h2 className="text-2xl font-bold text-gray-800">Order Details</h2>
            <p className="text-sm text-gray-600">{order.orderNumber}</p>
          </div>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 transition-colors"
          >
            <X size={28} />
          </button>
        </div>

        <div className="p-6 space-y-6">
          {/* Status Banner */}
          <div
            className={`flex items-center gap-3 p-4 rounded-lg border-2 ${getStatusColor(
              order.status
            )}`}
          >
            {getStatusIcon(order.status)}
            <div className="flex-1">
              <p className="font-semibold text-lg">{order.statusName}</p>
              <p className="text-sm opacity-80">Order {order.orderNumber}</p>
            </div>
            <div className="text-right text-sm">
              <p className="opacity-80">Created</p>
              <p className="font-medium">{formatDate(order.createdAt)}</p>
            </div>
          </div>

          {/* Order Timeline */}
          <div className="bg-gray-50 rounded-lg p-4">
            <h3 className="font-semibold text-lg mb-3 flex items-center gap-2">
              <Clock className="w-5 h-5" />
              Order History
            </h3>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-600">Created:</span>
                <span className="font-medium">{formatDate(order.createdAt)}</span>
              </div>
              {order.shippedAt && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Shipped:</span>
                  <span className="font-medium">{formatDate(order.shippedAt)}</span>
                </div>
              )}
              {order.deliveredAt && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Delivered:</span>
                  <span className="font-medium">{formatDate(order.deliveredAt)}</span>
                </div>
              )}
              {order.completedAt && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Completed:</span>
                  <span className="font-medium">{formatDate(order.completedAt)}</span>
                </div>
              )}
            </div>
          </div>

          <div className="grid md:grid-cols-2 gap-6">
            {/* Customer Information */}
            <div className="bg-white border rounded-lg p-4">
              <h3 className="font-semibold text-lg mb-3 flex items-center gap-2">
                <User className="w-5 h-5" />
                Customer Information
              </h3>
              <div className="space-y-2 text-sm">
                <div>
                  <p className="text-gray-600">User ID</p>
                  <p className="font-medium">#{order.userId}</p>
                </div>
                <div>
                  <p className="text-gray-600">Email</p>
                  <p className="font-medium">{order.userEmail}</p>
                </div>
                {order.userPhone && (
                  <div>
                    <p className="text-gray-600">Phone</p>
                    <p className="font-medium">{order.userPhone}</p>
                  </div>
                )}
              </div>
            </div>

            {/* Delivery Information */}
            <div className="bg-white border rounded-lg p-4">
              <h3 className="font-semibold text-lg mb-3 flex items-center gap-2">
                <MapPin className="w-5 h-5" />
                Delivery Information
              </h3>
              <div className="space-y-2 text-sm">
<div>
  <p className="text-gray-600">Address</p>
  <p className="font-medium">{formatAddress(order.shippingAddress)}</p>
  {typeof order.shippingAddress === 'object' && order.shippingAddress && (
    <div className="text-xs text-gray-500 mt-1 space-y-0.5">
      {order.shippingAddress.fullName && (
        <p>Name: {order.shippingAddress.fullName}</p>
      )}
      {order.shippingAddress.phone && (
        <p>Phone: {order.shippingAddress.phone}</p>
      )}
    </div>
  )}
</div>
                {order.trackingNumber && (
                  <div>
                    <p className="text-gray-600">Tracking Number</p>
                    <p className="font-medium font-mono">{order.trackingNumber}</p>
                  </div>
                )}
                {order.shippingCarrier && (
                  <div>
                    <p className="text-gray-600">Carrier</p>
                    <p className="font-medium">{order.shippingCarrier}</p>
                  </div>
                )}
              </div>
            </div>
          </div>

          {/* Payment Information */}
          {order.payment && (
            <div className="bg-white border rounded-lg p-4">
              <h3 className="font-semibold text-lg mb-3 flex items-center gap-2">
                <CreditCard className="w-5 h-5" />
                Payment Information
              </h3>
              <div className="grid md:grid-cols-3 gap-4 text-sm">
                <div>
                  <p className="text-gray-600">Method</p>
                  <p className="font-medium">{order.payment.method}</p>
                </div>
                <div>
                  <p className="text-gray-600">Status</p>
                  <p
                    className={`font-medium ${
                      order.payment.status === "Paid"
                        ? "text-green-600"
                        : "text-yellow-600"
                    }`}
                  >
                    {order.payment.status}
                  </p>
                </div>
                {order.payment.transactionId && (
                  <div>
                    <p className="text-gray-600">Transaction ID</p>
                    <p className="font-medium font-mono text-xs">
                      {order.payment.transactionId}
                    </p>
                  </div>
                )}
              </div>
            </div>
          )}

          {/* Customer Note */}
          {order.customerNote && (
            <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
              <h3 className="font-semibold text-sm text-yellow-800 mb-2">
                Customer Note
              </h3>
              <p className="text-sm text-gray-700">{order.customerNote}</p>
            </div>
          )}

          {/* Order Items */}
          <div className="bg-white border rounded-lg p-4">
            <h3 className="font-semibold text-lg mb-4 flex items-center gap-2">
              <Package className="w-5 h-5" />
              Order Items ({order.items.length})
            </h3>
            <div className="space-y-3">
              {order.items.map((item) => (
                <div
                  key={item.id}
                  className="flex gap-4 p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
                >
                  {/* Product Image */}
                  <div className="flex-shrink-0">
                    <img
                      src={item.product.primaryImagePath}
                      alt={item.product.name}
                      className="w-24 h-24 object-cover rounded-lg border"
                      onError={(e) => {
                        e.currentTarget.src =
                          "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='96' height='96'%3E%3Crect width='96' height='96' fill='%23ddd'/%3E%3Ctext x='50%25' y='50%25' text-anchor='middle' dy='.3em' fill='%23999' font-size='12'%3ENo Image%3C/text%3E%3C/svg%3E";
                      }}
                    />
                  </div>

                  {/* Product Info */}
                  <div className="flex-1 min-w-0">
                    <h4 className="font-semibold text-gray-800 mb-1">
                      {item.product.name}
                    </h4>
                    <div className="text-sm text-gray-600 space-y-1">
                      <p>
                        <span className="font-medium">Brand:</span>{" "}
                        {item.product.brandName}
                      </p>
                      <p>
                        <span className="font-medium">Category:</span>{" "}
                        {item.product.categoryName}
                      </p>
                      {item.variant && (
                        <>
                          <p>
                            <span className="font-medium">SKU:</span>{" "}
                            <span className="font-mono">{item.variant.sku}</span>
                          </p>
                          {item.variant.attributes.length > 0 && (
                            <div className="flex flex-wrap gap-1 mt-1">
                              {item.variant.attributes.map((attr, idx) => (
                                <span
                                  key={idx}
                                  className="px-2 py-0.5 bg-blue-100 text-blue-700 rounded text-xs"
                                >
                                  {attr.name}: {attr.value}
                                </span>
                              ))}
                            </div>
                          )}
                        </>
                      )}
                    </div>
                  </div>

                  {/* Price Info */}
                  <div className="text-right flex-shrink-0">
                    <p className="text-sm text-gray-600">Unit Price</p>
                    <p className="font-medium">{formatCurrency(item.price)}</p>
                    <p className="text-sm text-gray-600 mt-2">Quantity</p>
                    <p className="font-medium">x{item.quantity}</p>
                    <p className="text-sm text-gray-600 mt-2">Subtotal</p>
                    <p className="font-bold text-lg text-blue-600">
                      {formatCurrency(item.totalPrice)}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Total */}
          <div className="bg-gradient-to-r from-blue-50 to-purple-50 border-2 border-blue-200 rounded-lg p-6">
            <div className="flex justify-between items-center">
              <span className="text-xl font-semibold text-gray-700">
                Total Amount
              </span>
              <span className="text-3xl font-bold text-blue-600">
                {formatCurrency(order.totalAmount)}
              </span>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex justify-end gap-3 pt-4 border-t">
            <Button variant="outline" onClick={onClose}>
              Close
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}