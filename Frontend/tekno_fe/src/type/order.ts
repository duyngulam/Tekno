import { PaymentStatus } from "./payment";
import { Product, ProductVariant } from "./product";

export type OrderHistoryResponse = {
  data: Order[];
  page: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
};
export type Order = {
  id: number;
  orderNumber: string;
  status: number;
  statusName: string;
  totalAmount: number;
  createdAt: string;
  completedAt: string | null;
  payment: PaymentStatus | null;
  items: OrderItem[];
  delivery: string | null; // need fix
};
export type OrderItem = {
  id: number;
  quantity: number;
  price: number;
  totalPrice: number;
  product: Product;
  variant: ProductVariant;
};
