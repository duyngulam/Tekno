export type Coupon = {
    id: number;
    code: string;
    name: string;
    type: 'Percentage' | 'FixedAmount';
    value: number;
    quantity: number;
    usedCount: number;
    remainingQuantity: number;
    maxUsagePerUser: number | null;
    minPurchaseAmount: number | null;
    maxDiscountAmount: number | null;
    startDate: string;
    endDate: string;
    status: 'Active' | 'Inactive' | 'Expired';
    note: string | null;
    createdAt: string;
    updatedAt: string;
    applicableCategoryIds: number[];
    applicableProductIds: number[];
}

export type CouponResponse = {
    data: Coupon[];
    page: number;
    pageSize: number;
    totalRecords: number;
    totalPages: number;
}

