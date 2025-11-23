import { getProductsList } from "@/services/products";
import { Product } from "@/type/product";
import { ChevronRight } from "lucide-react";
import React from "react";
import ProductCard from "../product/ProductCard";

export default async function NewProducts() {
  const data = await getProductsList();
  const products: Product[] = data.data;
  console.log(products);

  // const [products, setProducts] = useState<Product[]>([]);
  // useEffect(() => {
  //   const fetchProducts = async () => {
  //     try {
  //       const res = await getProductsList(1, 10, "laptop");
  //       console.log(res);
  //       setProducts(res.data);
  //     } catch (error) {
  //       console.log("error in fetching new products", error);
  //     }
  //   };
  //   fetchProducts();
  // }, []);
  return (
    <div className="flex flex-col gap-5">
      <div className="border-b border-gray-500 flex items-center justify-between pb-2">
        <div className="font-semibold text-2xl">New Products</div>
        <button
          className="flex items-center gap-2 hoverEffect mx-10 hover:cursor-pointer"
          //onClick={() => router.push("/products")}
        >
          View all <ChevronRight className="w-5 h-5" />
        </button>
      </div>
      <div className="grid grid-col-2 md:grid-cols-4 gap-4">
        {products?.slice(0, 4).map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>
    </div>
  );
}
