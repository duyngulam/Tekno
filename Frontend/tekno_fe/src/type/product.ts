// export interface Product {
//   id: number;
//   name: string;
//   slug: string;
//   basePrice: number;
//   overview: string;
//   brandName: string;
//   categoryName: string;
//   finalPrice: number;
//   discountPercent: number | null;
//   primaryImagePath: string;
// }



export interface Product {
  id: number;
  name: string;
  slug: string;
  brandName: string;
  categoryName: string;
  basePrice: number;
  discountPercent: number | null;
  primaryImagePath: string;
  finalPrice: number;
  overview: string;
  rating: number;
  description: string;
  warrantyInfo: string | null;
  totalSold: number;
  averageRating: number;
  specs: {
    name: string;
    value: string[];
  }[];
  images: string[];
  variants: ProductVariant[];
  // variants: {
  //   id: number;
  //   sku: string;
  //   price: number;
  //   stock: number; // số lượng
  //   attributes: VariantAttribute[];
  //   // attributes: {
  //   //   name: string;
  //   //   value: string[]; // mảng string
  //   // }[];
  // }[];
}

export type ProductVariant = {
  id: number;
  sku: string;
  price: number;
  stock: number;
  attributes: VariantAttribute[];
};


export type VariantAttribute = {
  id: number;
  name: string;
  value: string[];
};
