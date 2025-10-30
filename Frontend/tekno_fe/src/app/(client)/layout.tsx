// layout cho client
import Header from "@/components/MainLayout/Header";
import "@/styles/globals.css";
import Footer from "@/components/MainLayout/Footer";
import { AuthProvider } from "@/context/AuthContext";

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
      <body className="bg-white text-gray-900 h-full flex flex-col">
        <AuthProvider>
          <Header />
          <main className="flex-1 mx-auto max-w-screen">
            <div className="py-3 md:py-3">{children}</div>
          </main>
          <Footer />
        </AuthProvider>
      </body>
    </html>
  );
}
