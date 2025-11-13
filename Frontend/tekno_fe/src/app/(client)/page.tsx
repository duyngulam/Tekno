import Login from "@/components/auth/LoginForm";
import HomeBanner from "@/components/landing/HomeBanner";
import { Container } from "@/components/MainLayout/Container";
import HomeCategoryTabBar from "@/components/landing/HomeCategoryTabBar";
import ProductsOnSale from "@/components/landing/ProductsOnSale";
import NewProducts from "@/components/landing/NewProducts";
import BestSell from "@/components/landing/BestSell";
import TopBrand from "@/components/landing/TopBrand";
import FooterTop from "@/components/MainLayout/Footer/FooterTop";
import OurBlogs from "@/components/landing/OurBlogs";

// HomePage
export default function Home() {
  return (
    <Container>
      <HomeBanner />
      <div className="space-y-10 py-10">
        <HomeCategoryTabBar />
        <ProductsOnSale />
        <NewProducts />
        <BestSell />
        <TopBrand />
        <OurBlogs />
      </div>
      <FooterTop />
    </Container>
  );
}
