import { Container } from "@/components/MainLayout/Container";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { getProductDetail } from "@/services/products";
import ImageView from "@/components/product/productDetail/ImageView";
import {
  Check,
  Heart,
  HousePlug,
  LineChart,
  LineChartIcon,
  Star,
} from "lucide-react";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
  TableFooter,
} from "@/components/ui/table";
import SimilarProducts from "@/components/product/productDetail/SimilarProducts";
import FrequentlyBoughtTogether from "@/components/product/productDetail/FrequentlyBoughtTogether";
import Comments from "@/components/product/productDetail/Comments";
import TechnicalDetails from "@/components/product/productDetail/TechnicalDetails";
import { Button } from "@/components/ui/button";
import AddToFavorButton from "@/components/product/AddToFavorButton";
import AddToCartButton from "@/components/product/AddToCartButton";
import NotFoundPage from "../../not-found";
import ProductVariantSelectorDynamic from "@/components/product/productDetail/ProductVariantSelectorDynamic";

export default async function SingleProductPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const product = await getProductDetail(slug);
  const isStock = product?.variants[0].stock > 0;

  if (!product) return NotFoundPage;

  return (
    <Container className="flex flex-col space-y-5 my-5 ">
      <Breadcrumb />
      {/* product */}
      <div className="flex flex-col md:flex-row gap-10 ">
        {product?.images && (
          <ImageView images={product?.images} isStock={isStock} />
        )}
        <div className="w-full md:w-1/2 flex flex-col gap-4">
          <h2 className="text-2xl">{product.name}</h2>
          <div className="flex items-center gap-2 ">
            {/* sao */}
            <div className="flex items-center text-white gap-2 rounded-md bg-primary px-2 py-1">
              <Star fill="white" className="h-5 w-5" />
              <span>4.9</span>
            </div>
            {/* sold */}
            <div className="border-l border-gray-500 px-2">
              sold <span>125</span>
            </div>
          </div>
          {/* 3 */}
          <div className="flex items-center justify-between border-b border-gray-500 md:border-none pb-2">
            <div className="flex items-center gap-2">
              <HousePlug className="text-primary" />
              <span>In stock</span>
            </div>
            <div className="flex items-center gap-2">
              <HousePlug className="text-primary" />
              <span>Guaranteed</span>
            </div>
            <div className="flex items-center gap-2">
              <HousePlug className="text-primary" />
              <span>Free Delivery</span>
            </div>
          </div>

          {/* Select color */}
          <div className="flex flex-col md:flex-row md:items-center gap-4 border-b border-gray-500 md:border-none pb-2">
            <p>Select color</p>
            <div className="flex flex-wrap gap-4">
              <div className="flex items-center rounded-md border border-gray-500/80 hover:border-gray-500/100 hover:bg-gray-100 py-1 px-5 gap-2">
                <div className="h-7 w-7 rounded-full bg-gray-800 flex items-center justify-center">
                  <Check className="h-5 w-5 text-white" />
                </div>
                <p>gray</p>
              </div>
              <button className="rounded-md border border-gray-500/80 hover:border-gray-500/100 hover:bg-gray-100 py-2 px-1">
                <div className="rounded-full bg-gray-50"></div>
                <p>gray</p>
              </button>
            </div>
          </div>

          {/* table */}
          <Table>
            <TableBody>
              <TableRow>
                <TableCell className="text-gray-500 before:content-['•'] before:mr-2">
                  Brand
                </TableCell>
                <TableCell>{product.brandName}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell className="text-gray-500 before:content-['•'] before:mr-2">
                  Model Name{" "}
                </TableCell>
                <TableCell>{product.name}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell className="text-gray-500 before:content-['•'] before:mr-2">
                  Screen Size
                </TableCell>
                <TableCell>
                  {product.variants[0]?.attributes[0]?.value}
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell className="text-gray-500 before:content-['•'] before:mr-2">
                  Hard Disk Size
                </TableCell>
                <TableCell>{product.brandName}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell className="text-gray-500 before:content-['•'] before:mr-2">
                  CPU Model
                </TableCell>
                <TableCell>{product.brandName}</TableCell>
              </TableRow>
            </TableBody>
          </Table>

          <ProductVariantSelectorDynamic product={product} />

          {/* button add to cart and favor */}
          {/* <div className="flex items-center justify-center gap-5">
            <AddToCartButton product={product} />
            <AddToFavorButton product={product} className="relative" />
          </div> */}
        </div>
      </div>

      {/* Technical Details*/}
      <TechnicalDetails specs={product.specs} />

      {/* Similar Products */}
      <SimilarProducts />

      {/* Comments */}
      <Comments />

      {/* SFrequently bought together */}
      <FrequentlyBoughtTogether />
      {/* Reviews */}
      <div className="col-span-12 bg-amber-100">
        <div className="font-medium py-2 ">Reviews</div>
      </div>
    </Container>
  );
}
