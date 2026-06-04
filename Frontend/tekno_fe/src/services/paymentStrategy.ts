// src/services/paymentStrategy.ts
import { paymentService } from "./payment";
import { PaymentPayload } from "@/type/payment";

export interface PaymentStrategy {
  process(payload: PaymentPayload): Promise<void>;
}

// 1. Redirection Payment Strategy (VNPay, Momo, Credit Cards, etc.)
export class RedirectPaymentStrategy implements PaymentStrategy {
  public async process(payload: PaymentPayload): Promise<void> {
    const res = await paymentService.processPayment(payload);
    if (!res || !res.paymentUrl) {
      throw new Error("Không nhận được URL thanh toán từ hệ thống.");
    }
    
    if (typeof window !== "undefined") {
      localStorage.setItem("LastPaymentUrl", res.paymentUrl);
      window.location.href = res.paymentUrl;
    }
  }
}

// 2. Cash on Delivery (COD) Strategy
export class CodPaymentStrategy implements PaymentStrategy {
  public async process(payload: PaymentPayload): Promise<void> {
    const res = await paymentService.processPayment(payload);
    
    if (typeof window !== "undefined") {
      if (res && res.paymentUrl) {
        window.location.href = res.paymentUrl;
      } else {
        // If COD does not require redirection, redirect to success result directly
        window.location.href = `${window.location.origin}/payment/result?orderId=${payload.orderId}&status=success`;
      }
    }
  }
}

// Context to maintain reference to the strategy
export class PaymentContext {
  private strategy: PaymentStrategy;

  constructor(strategy: PaymentStrategy) {
    this.strategy = strategy;
  }

  public setStrategy(strategy: PaymentStrategy): void {
    this.strategy = strategy;
  }

  public async execute(payload: PaymentPayload): Promise<void> {
    if (!this.strategy) {
      throw new Error("Chiến lược thanh toán chưa được cấu hình.");
    }
    return this.strategy.process(payload);
  }
}

// Factory to resolve the strategy based on gateway
export class PaymentStrategyFactory {
  public static getStrategy(gatewayName: string): PaymentStrategy {
    const name = gatewayName.toLowerCase();
    if (
      name.includes("cod") ||
      name.includes("cash") ||
      name.includes("tiền mặt")
    ) {
      return new CodPaymentStrategy();
    }
    return new RedirectPaymentStrategy();
  }
}
