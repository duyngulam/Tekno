export interface Product {
  id: number;
  name: string;
  slug: string;
  basePrice: number;
  overview: string;
  brandName: string;
  categoryName: string;
  finalPrice: number;
  discountPercent: number | null;
  primaryImagePath: string;
}



export interface ProductDetail  {
  id: number;
  name: string;
  slug: string;
  brandName: string;
  categoryName: string;
  basePrice: number;
  discountPercent: number | null;
  finalPrice: number;
  overview: string;
  description: string;
  warrantyInfo: string | null;
  specs: {
    name: string;
    value: string[];
  }[];
  images: string[];
  variants: {
    id: number;
    sku: string;
    price: number;
    stock: boolean;
    attributes: {
      name?: string;
      value?: string;
    }[];
  }[];
};

