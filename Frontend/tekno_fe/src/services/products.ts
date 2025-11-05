import { Product } from "@/type/product";

// services/products.ts
export async function getProductList() {
//   const res = await fetch("https://api.example.com/products");
    //   return res.json();
    const productList: Product[] = [
        {
          id: 1,
          name: 'MacBook Pro 16" M2 Max',
          slug: "macbook-pro-16-m2-max",
          basePrice: 2499,
          overview:
            "Powerful laptop with Apple M2 Max chip and Liquid Retina XDR display.",
          brandName: "Apple",
          primaryImageUrl:
            "https://images.unsplash.com/photo-1754928864131-21917af96dfd?w=400",
        },
        {
          id: 2,
          name: 'iPad Pro 12.9" 256GB',
          slug: "ipad-pro-12-9-256gb",
          basePrice: 1099,
          overview: "High-performance tablet with M2 chip and ProMotion display.",
          brandName: "Apple",
          primaryImageUrl:
            "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400",
        },
        {
          id: 3,
          name: "Sony WH-1000XM5 Headphones",
          slug: "sony-wh-1000xm5-headphones",
          basePrice: 349,
          overview:
            "Noise-cancelling wireless headphones with exceptional sound quality.",
          brandName: "Sony",
          primaryImageUrl:
            "https://images.unsplash.com/photo-1660391532247-4a8ad1060817?w=400",
        },
        {
          id: 4,
          name: "Logitech MX Master 3S",
          slug: "logitech-mx-master-3s",
          basePrice: 99,
          overview:
            "Ergonomic wireless mouse with advanced precision and quiet clicks.",
          brandName: "Logitech",
          primaryImageUrl:
            "https://images.unsplash.com/photo-1660491083562-d91a64d6ea9c?w=400",
        },
        {
          id: 5,
          name: "Samsung Galaxy S24 Ultra",
          slug: "samsung-galaxy-s24-ultra",
          basePrice: 1199,
          overview:
            "Flagship smartphone with pro-grade camera and AI-powered performance.",
          brandName: "Samsung",
          primaryImageUrl:
            "https://images.unsplash.com/photo-1675953935267-e039f13ddd79?w=400",
        },
        {
          id: 6,
          name: 'Dell UltraSharp 27" 4K',
          slug: "dell-ultrasharp-27-4k",
          basePrice: 599,
          overview: "27-inch 4K monitor with ultra-thin bezels and color accuracy.",
          brandName: "Dell",
          primaryImageUrl:
            "https://images.unsplash.com/photo-1593833210845-d9935371664e?w=400",
        },
    ];
    
    return productList;
}

export async function getProductDetail(id: string | number) {
//   const res = await fetch(`https://api.example.com/products/${id}`);
//   return res.json();
}
