// layout cho client
import Header from "@/components/MainLayout/Header";
import "../../styles/globals.css";
import Footer from "@/components/MainLayout/Footer";
import { AuthProvider } from "@/context/AuthContext";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { Container } from "@/components/MainLayout/Container";

// Đặt metadata cho layout
export const metadata = {
  title: "Tekno",
};

export default function ClientLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" className="h-full">
      <body className="bg-white text-gray-900 flex flex-col min-h-screen">
        <AuthProvider>
          <Header />
          <main className="flex-1">{children}</main>
          <Footer />
        </AuthProvider>
      </body>
    </html>
  );
}
