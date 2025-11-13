"use client";

import { Container } from "@/components/MainLayout/Container";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useParams } from "next/navigation";

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
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { useEffect, useState } from "react";
import { getProductDetail } from "@/services/products";
import { ProductDetail } from "@/type/product";

export default function ProductDetailPage() {
  const { slug } = useParams(); // 👈 lấy slug từ URL

  const [product, setProduct] = useState<ProductDetail | null>(null);

  useEffect(() => {
    const fetchProductDetail = async () => {
      try {
        const data = await getProductDetail(slug?.toString() || "");
        setProduct(data);
      } catch (error) {
        console.error("Error fetching product detail:", error);
      }
    };

    fetchProductDetail();
  }, [slug]); //Thay doi khi slug thay doi

  if (!product) return <p>Đang tải sản phẩm...</p>;
  console.log("Product Detail:", product);

  return (
    <>
      <Container>
        <div className="col-span-12">
          <Breadcrumb />
        </div>

        {/* Product detail */}
        <div className="col-span-9">
          <div className="flex gap-4">
            <div className="flex-5">Image</div>
            <div className="flex-4 border-amber-700">
              <p>{product.name}</p>
            </div>
          </div>
          {/* Technical Details*/}
          <div className="">
            <div className="font-medium py-2 ">Technical Details</div>
            <Table>
              <TableCaption>show more</TableCaption>
              <TableBody className="[&_tr:nth-child(odd)]:bg-white [&_tr:nth-child(even)]:bg-gray-50">
                <TableRow>
                  <TableCell className="w-2/6">iPhone 15 Pro</TableCell>
                  <TableCell className="w-4/6">30.000.000₫</TableCell>
                </TableRow>
                <TableRow>
                  <TableCell>MacBook Air M3</TableCell>
                  <TableCell>28.000.000₫</TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </div>
        </div>
        {/* installments */}
        <div className="col-span-3">installments</div>
        {/* Similar Products */}
        <div className="col-span-12 bg-amber-100">
          <div className="font-medium py-2 ">Similar Products</div>
        </div>
        {/* Comments */}
        <div className="col-span-12 bg-amber-100 flex">
          <div className="flex-3">
            <div className="font-medium py-2 ">Comments</div>
            <p>leave your comments here for other customers</p>
            <div className="grid w-full gap-2">
              <Textarea placeholder="Share your thoughts about this product here" />
              <Button>Comment</Button>
            </div>
            <p className="h6">By feature</p>
          </div>
          <div className="flex-9 font-medium py-2 ">Comments</div>
        </div>
        {/* SFrequently bought together */}
        <div className="col-span-12 bg-amber-100">
          <div className="font-medium py-2 ">Frequently bought together</div>
        </div>
        {/* Reviews */}
        <div className="col-span-12 bg-amber-100">
          <div className="font-medium py-2 ">Reviews</div>
        </div>
      </Container>
    </>
  );
}
