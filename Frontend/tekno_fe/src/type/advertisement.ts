export type AdvertisementPosition =
  | "HomeTop"
  | "HomeMiddle"
  | "HomeBottom"
  | "CategoryTop"
  | "ProductSidebar";

export type Advertisement = {
  id: number;
  productId: number;
  productName: string;
  productSlug: string;
  imageUrl: string;
  position: AdvertisementPosition;
  priority: number;
  isActive: boolean;
  startDate: string;
  endDate: string;
  createdAt: string;
  isCurrentlyActive: boolean;
};