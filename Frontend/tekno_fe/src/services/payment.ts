// src/services/payment.ts
import { httpClient } from "@/lib/httpClient";
import {
  MyPaymentsResponse,
  PaymentGateway,
  PaymentPayload,
  PaymentStatus,
} from "@/type/payment";

export class PaymentService {
  private static instance: PaymentService | null = null;

  private constructor() {}

  public static getInstance(): PaymentService {
    if (!PaymentService.instance) {
      PaymentService.instance = new PaymentService();
    }
    return PaymentService.instance;
  }

  public async getPaymentGateways(): Promise<PaymentGateway[]> {
    return httpClient.get<PaymentGateway[]>("/payment/gateways");
  }

  public async getMyPayments(): Promise<MyPaymentsResponse> {
    return httpClient.get<MyPaymentsResponse>("/payment/my-payments");
  }

  public async getPaymentStatus(transactionId: string): Promise<PaymentStatus> {
    return httpClient.get<PaymentStatus>(`/payment/status/${transactionId}`);
  }

  public async processPayment(payload: PaymentPayload): Promise<{
    paymentUrl: string;
  }> {
    return httpClient.post<{ paymentUrl: string }>("/payment/process", payload);
  }
}

export const paymentService = PaymentService.getInstance();

// Backward compatibility exports
export const getPaymentGateways = (token?: string) => paymentService.getPaymentGateways();
export const getMyPayments = (token?: string) => paymentService.getMyPayments();
export const getPaymentStatus = (transactionId: string, token?: string) => paymentService.getPaymentStatus(transactionId);
export const processPayment = (arg1: string | PaymentPayload, arg2?: PaymentPayload) => {
  if (typeof arg1 === "string") {
    return paymentService.processPayment(arg2!);
  }
  return paymentService.processPayment(arg1);
};
