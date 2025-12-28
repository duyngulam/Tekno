"use client";

import React, { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { X, Eye, Package, Truck, CheckCircle, XCircle } from "lucide-react";
import {
  getAdminOrders,
  getAdminOrder,
  cancelOrder,
  deliverOrder,
  shipOrder,
  Order,
  OrderStatus,
} from "@/services/orders";

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [openDetail, setOpenDetail] = useState(false);
  const [openShipModal, setOpenShipModal] = useState(false);
  
  // Filters
  const [statusFilter, setStatusFilter] = useState<OrderStatus | "">("");
  const [searchKeyword, setSearchKeyword] = useState("");

  // Ship form
  const [trackingNumber, setTrackingNumber] = useState("");
  const [carrier, setCarrier] = useState("");

  useEffect(() => {
    loadOrders();
  }, [statusFilter]);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const res = await getAdminOrders({
        status: statusFilter || undefined,
        keyword: searchKeyword || undefined,
        sortBy: "createdAt",
        sortOrder: "desc",
      });
      
      const list = res?.data?.data || res?.data || [];
      setOrders(list);
    } catch (err) {
      console.error("Failed to load orders:", err);
      setOrders([]);
    } finally {
      setLoading(false);
    }
  };

  const loadOrderDetail = async (order: Order) => {
    try {
      const detail = await getAdminOrder(order.id);
      const orderData = detail?.data || detail;
      
      console.log("📦 Order Detail:", orderData);
      setSelectedOrder(orderData);
      setOpenDetail(true);
    } catch (err) {
      console.error("Failed to load order detail:", err);
      alert("Failed to load order details");
    }
  };

  const handleSearch = () => {
    loadOrders();
  };

  const handleCancelOrder = async (orderId: number) => {
    const reason = prompt("Enter cancellation reason (optional):");
    if (reason === null) return; // User clicked cancel
    
    try {
      await cancelOrder(orderId, reason);
      alert("Order cancelled successfully!");
      await loadOrders();
      
      // Update detail view if open
      if (selectedOrder?.id === orderId) {
        await loadOrderDetail({ id: orderId } as Order);
      }
    } catch (err) {
      console.error("Failed to cancel order:", err);
      alert("Failed to cancel order");
    }
  };

  const handleDeliverOrder = async (orderId: number) => {
    if (!confirm("Mark this order as delivered?")) return;
    
    try {
      await deliverOrder(orderId);
      alert("Order marked as delivered!");
      await loadOrders();
      
      if (selectedOrder?.id === orderId) {
        await loadOrderDetail({ id: orderId } as Order);
      }
    } catch (err) {
      console.error("Failed to deliver order:", err);
      alert("Failed to mark as delivered");
    }
  };

  const openShipForm = (order: Order) => {
    setSelectedOrder(order);
    setTrackingNumber("");
    setCarrier("");
    setOpenShipModal(true);
  };

  const handleShipOrder = async () => {
    if (!selectedOrder) return;
    
    try {
      await shipOrder(selectedOrder.id, trackingNumber, carrier);
      alert("Order shipped successfully!");
      await loadOrders();
      setOpenShipModal(false);
      
      if (openDetail) {
        await loadOrderDetail({ id: selectedOrder.id } as Order);
      }
    } catch (err) {
      console.error("Failed to ship order:", err);
      alert("Failed to ship order");
    }
  };

  const getStatusColor = (status: string) => {
    switch (status?.toLowerCase()) {
      case "pending":
        return "bg-yellow-100 text-yellow-800";
      case "processing":
        return "bg-blue-100 text-blue-800";
      case "shipped":
        return "bg-purple-100 text-purple-800";
      case "delivered":
        return "bg-green-100 text-green-800";
      case "cancelled":
        return "bg-red-100 text-red-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString("vi-VN");
  };

  const formatCurrency = (amount: number) => {
    return amount.toLocaleString("vi-VN") + "đ";
  };

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-xl font-semibold">Orders Management</h2>
      </div>

      {/* Filters */}
      <div className="mb-4 flex gap-3">
        <div className="flex-1">
          <Input
            placeholder="Search by order number, customer name, email..."
            value={searchKeyword}
            onChange={(e) => setSearchKeyword(e.target.value)}
            onKeyPress={(e) => e.key === "Enter" && handleSearch()}
          />
        </div>

        <select
          className="border rounded px-3 py-2"
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value as OrderStatus | "")}
        >
          <option value="">All Status</option>
          <option value="Pending">Pending</option>
          <option value="Processing">Processing</option>
          <option value="Shipped">Shipped</option>
          <option value="Delivered">Delivered</option>
          <option value="Cancelled">Cancelled</option>
        </select>

        <Button onClick={handleSearch}>Search</Button>
      </div>

      {/* Orders Table */}
      {loading ? (
        <p>Loading orders...</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-sm bg-white shadow rounded">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2">Order #</th>
                <th>Customer</th>
                <th>Status</th>
                <th>Total</th>
                <th>Payment</th>
                <th>Date</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>
              {orders.length === 0 ? (
                <tr>
                  <td colSpan={7} className="p-8 text-center text-gray-500">
                    No orders found
                  </td>
                </tr>
              ) : (
                orders.map((order) => (
                  <tr
                    key={order.id}
                    className="hover:bg-gray-50 border-b cursor-pointer"
                    onClick={() => loadOrderDetail(order)}
                  >
                    <td className="p-2 font-medium">{order.orderNumber}</td>
                    <td className="p-2">
                      <div>
                        <p className="font-medium">{order.userName || "N/A"}</p>
                        <p className="text-xs text-gray-600">{order.userEmail}</p>
                        <p className="text-xs text-gray-600">{order.phoneNumber}</p>
                      </div>
                    </td>
                    <td className="p-2">
                      <span
                        className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(
                          order.status
                        )}`}
                      >
                        {order.status}
                      </span>
                    </td>
                    <td className="p-2 font-medium">
                      {formatCurrency(order.totalAmount)}
                    </td>
                    <td className="p-2">
                      <div>
                        <p className="text-xs">{order.paymentMethod || "N/A"}</p>
                        <span
                          className={`text-xs px-1 py-0.5 rounded ${
                            order.paymentStatus === "Paid"
                              ? "bg-green-100 text-green-700"
                              : "bg-yellow-100 text-yellow-700"
                          }`}
                        >
                          {order.paymentStatus || "Pending"}
                        </span>
                      </div>
                    </td>
                    <td className="p-2 text-xs">{formatDate(order.createdAt)}</td>
                    <td className="p-2">
                      <div className="flex gap-2" onClick={(e) => e.stopPropagation()}>
                        <button
                          onClick={() => loadOrderDetail(order)}
                          className="p-1 text-blue-600 hover:bg-blue-50 rounded"
                          title="View Details"
                        >
                          <Eye size={16} />
                        </button>

                        {order.status === "Processing" && (
                          <button
                            onClick={() => openShipForm(order)}
                            className="p-1 text-purple-600 hover:bg-purple-50 rounded"
                            title="Ship Order"
                          >
                            <Truck size={16} />
                          </button>
                        )}

                        {order.status === "Shipped" && (
                          <button
                            onClick={() => handleDeliverOrder(order.id)}
                            className="p-1 text-green-600 hover:bg-green-50 rounded"
                            title="Mark as Delivered"
                          >
                            <CheckCircle size={16} />
                          </button>
                        )}

                        {["Pending", "Processing"].includes(order.status) && (
                          <button
                            onClick={() => handleCancelOrder(order.id)}
                            className="p-1 text-red-600 hover:bg-red-50 rounded"
                            title="Cancel Order"
                          >
                            <XCircle size={16} />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Order Detail Modal */}
      {openDetail && selectedOrder && (
        <div className="fixed inset-0 bg-black/60 flex justify-center items-center z-50">
          <div className="bg-white p-6 rounded-lg w-[900px] max-h-[90vh] overflow-y-auto shadow-xl">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">
                Order Details - {selectedOrder.orderNumber}
              </h2>
              <button
                onClick={() => setOpenDetail(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X size={28} />
              </button>
            </div>

            {/* Order Info */}
            <div className="grid grid-cols-2 gap-4 mb-6">
              <div>
                <h3 className="font-semibold mb-2">Customer Information</h3>
                <p><strong>Name:</strong> {selectedOrder.userName || "N/A"}</p>
                <p><strong>Email:</strong> {selectedOrder.userEmail}</p>
                <p><strong>Phone:</strong> {selectedOrder.phoneNumber}</p>
              </div>

              <div>
                <h3 className="font-semibold mb-2">Order Information</h3>
                <p><strong>Order #:</strong> {selectedOrder.orderNumber}</p>
                <p>
                  <strong>Status:</strong>{" "}
                  <span
                    className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(
                      selectedOrder.status
                    )}`}
                  >
                    {selectedOrder.status}
                  </span>
                </p>
                <p><strong>Payment:</strong> {selectedOrder.paymentMethod}</p>
                <p><strong>Payment Status:</strong> {selectedOrder.paymentStatus}</p>
                <p><strong>Created:</strong> {formatDate(selectedOrder.createdAt)}</p>
              </div>
            </div>

            {/* Shipping Address */}
            <div className="mb-6">
              <h3 className="font-semibold mb-2">Shipping Address</h3>
              <p>{selectedOrder.shippingAddress}</p>
              {selectedOrder.shippingWard && <p>Ward: {selectedOrder.shippingWard}</p>}
              {selectedOrder.shippingDistrict && (
                <p>District: {selectedOrder.shippingDistrict}</p>
              )}
              {selectedOrder.shippingCity && <p>City: {selectedOrder.shippingCity}</p>}
            </div>

            {/* Order Items */}
            <div className="mb-6">
              <h3 className="font-semibold mb-2">Order Items</h3>
              {selectedOrder.items && selectedOrder.items.length > 0 ? (
                <div className="space-y-2">
                  {selectedOrder.items.map((item) => (
                    <div
                      key={item.id}
                      className="flex items-center gap-3 p-3 border rounded"
                    >
                      {item.imageUrl && (
                        <img
                          src={item.imageUrl}
                          alt={item.productName}
                          className="w-16 h-16 object-cover rounded"
                        />
                      )}
                      <div className="flex-1">
                        <p className="font-medium">{item.productName}</p>
                        {item.variantSku && (
                          <p className="text-xs text-gray-600">SKU: {item.variantSku}</p>
                        )}
                        <p className="text-sm text-gray-600">
                          {formatCurrency(item.price)} x {item.quantity}
                        </p>
                      </div>
                      <div className="text-right">
                        <p className="font-medium">{formatCurrency(item.subtotal)}</p>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-gray-500 text-sm">No items</p>
              )}
            </div>

            {/* Total */}
            <div className="border-t pt-4">
              <div className="flex justify-between text-lg font-bold">
                <span>Total Amount:</span>
                <span>{formatCurrency(selectedOrder.totalAmount)}</span>
              </div>
            </div>

            {/* Notes */}
            {selectedOrder.notes && (
              <div className="mt-4 p-3 bg-yellow-50 border border-yellow-200 rounded">
                <p className="text-sm">
                  <strong>Notes:</strong> {selectedOrder.notes}
                </p>
              </div>
            )}

            {/* Action Buttons */}
            <div className="flex justify-end gap-3 mt-6">
              {selectedOrder.status === "Processing" && (
                <Button onClick={() => openShipForm(selectedOrder)}>
                  <Truck className="w-4 h-4 mr-2" />
                  Ship Order
                </Button>
              )}

              {selectedOrder.status === "Shipped" && (
                <Button onClick={() => handleDeliverOrder(selectedOrder.id)}>
                  <CheckCircle className="w-4 h-4 mr-2" />
                  Mark as Delivered
                </Button>
              )}

              {["Pending", "Processing"].includes(selectedOrder.status) && (
                <Button
                  variant="outline"
                  onClick={() => handleCancelOrder(selectedOrder.id)}
                  className="text-red-600"
                >
                  <XCircle className="w-4 h-4 mr-2" />
                  Cancel Order
                </Button>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Ship Order Modal */}
      {openShipModal && selectedOrder && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
          <div className="bg-white w-full max-w-md rounded-lg shadow-lg p-6">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-xl font-semibold">Ship Order</h3>
              <button
                onClick={() => setOpenShipModal(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X size={24} />
              </button>
            </div>

            <p className="text-sm text-gray-600 mb-4">
              Order: <strong>{selectedOrder.orderNumber}</strong>
            </p>

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">
                  Tracking Number (optional)
                </label>
                <Input
                  value={trackingNumber}
                  onChange={(e) => setTrackingNumber(e.target.value)}
                  placeholder="Enter tracking number"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">
                  Carrier (optional)
                </label>
                <Input
                  value={carrier}
                  onChange={(e) => setCarrier(e.target.value)}
                  placeholder="e.g., Giao Hàng Nhanh, Viettel Post"
                />
              </div>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <Button variant="outline" onClick={() => setOpenShipModal(false)}>
                Cancel
              </Button>
              <Button onClick={handleShipOrder}>
                <Truck className="w-4 h-4 mr-2" />
                Ship Order
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}