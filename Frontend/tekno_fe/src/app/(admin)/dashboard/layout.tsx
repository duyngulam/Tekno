// layout cho client
import Header from "@/components/MainLayout/admin/Header";
import Footer from "@/components/MainLayout/Footer";
import "../../../styles/globals.css";
import { AuthProvider } from "@/context/AuthContext";
import Sidebar from "@/components/MainLayout/admin/Sidebar";

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
          <div className="flex flex-col min-h-screen bg-gray-100">
            {/* Header cố định */}
            <Header />

            <div className="flex flex-1">
              {/* Sidebar bên trái */}
              <Sidebar />

              {/* Nội dung chính */}
              <main className="flex-1 p-6 overflow-y-auto">{children}</main>
            </div>
          </div>
        </body>
      </AuthProvider>
    </html>
  );
}
