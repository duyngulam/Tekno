// layout cho client
import AdminHeader from "@/components/admin/AdminHeader";
import "@/styles/globals.css";
import { AuthProvider } from "@/context/AuthContext";
import AdminSidebar from "@/components/admin/AdminSidebar";

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
    <html lang="en" >
      <body className="flex flex-col h-screen bg-gray-50 text-gray-900">
        <AuthProvider>
          <AdminHeader />
          <div className="flex flex-1 overflow-hidden">
            <AdminSidebar />
            <main className="flex-1 p-6 overflow-y-auto">
              {children}
            </main>
          </div>
        </AuthProvider>
      </body>
    </html>
  );
}
