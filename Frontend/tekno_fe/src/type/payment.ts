
export type PaymentGateway = {
  id: number;
  name: string;
  description: string;
  available: boolean;
};

export type PaymentHistory = {
  paymentId: number;
  orderId: number;
  orderNumber: string;
  transactionId: string;
  gateway: number;
  gatewayName: string;
  method: number;
  methodName: string;
  status: number;
  statusName: string;
  amount: number;
  currency: string;
  createdAt: string;
  completedAt: string | null;
  errorMessage: string | null;
};

export type PaymentStatus = {
  paymentId: number;
  orderId: number;
  orderNumber: string;
  transactionId: string;
  gateway: number;
  gatewayName: string;
  method: number;
  methodName: string;
  status: number;
  statusName: string;
  amount: number;
  currency: string;
  createdAt: string;
  completedAt: string | null;
  errorMessage: string | null;
};

export type MyPaymentsResponse = {
  data: PaymentHistory[];
  page: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
};

export type PaymentPayload = {
  shippingAddressId: number;
  gateway: number;        // ví dụ: 0 = VNPay, 1 = Momo (tuỳ backend)
  method: number;         // ví dụ: 1 = Credit Card
  couponCode?: string;    // optional
  note?: string;          // optional
  returnUrl: string;
  selectedItems: {
    variantId: number;
    quantity: number;
  }[];
};
export type PaymentProcessResponse = {
  orderId: number;
  orderNumber: string;
  paymentId: number;
  transactionId: string;
  paymentUrl: string;
  paymentToken: string | null;
  qrCodeUrl: string | null;
  status: number;
  totalAmount: number;
  itemsCount: number;
};
