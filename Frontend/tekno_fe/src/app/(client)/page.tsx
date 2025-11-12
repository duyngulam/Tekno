import Login from "@/components/auth/LoginForm";
import CategoryTabBar from "@/components/landing/CategoryTabBar";
import HomeBanner from "@/components/landing/HomeBanner";
import { Container } from "@/components/MainLayout/Container";
import { Breadcrumb } from "@/components/share/breadcumbCustom";

// HomePage
export default function Home() {
  return (
    <Container>
      <HomeBanner />
      <div className="space-y-10 py-10">
        <CategoryTabBar />
      </div>
    </Container>
  );
}
