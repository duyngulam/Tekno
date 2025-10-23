// layout cho client
import Header from "@/components/MainLayout/Header";
import Footer from "@/components/MainLayout/Footer";
import "../../../styles/globals.css";
import { AuthProvider } from "@/context/AuthContext";

// Đặt metadata cho layout
export const metadata = {
  title: "Tekno Admin",
};

export default function AdminLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" className="h-full">
      <AuthProvider>
        <body className="bg-white text-gray-900 h-full flex flex-col min-h-screen">
          <Header />
          <main className="mx-auto max-w-screen">{children}</main>
        </body>
      </AuthProvider>
    </html>
  );
}
