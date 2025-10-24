"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { FaHome, FaUser, FaCog } from "react-icons/fa";
import {
  FaTachometerAlt,
  FaBoxOpen,
  FaClipboardList,
  FaHeart,
  FaUserFriends,
  FaStore,
  FaBell,
  FaImages,
} from "react-icons/fa";

export interface SidebarProps {
  icon: React.ReactNode;
  label: string;
  href: string;
}

export const sidebarData: SidebarProps[] = [
  {
    icon: <FaTachometerAlt />,
    label: "Dashboard",
    href: "/admin/dashboard",
  },
  {
    icon: <FaBoxOpen />,
    label: "Products",
    href: "/admin/products",
  },
  {
    icon: <FaClipboardList />,
    label: "Product Catalog",
    href: "/admin/catalog",
  },
  {
    icon: <FaHeart />,
    label: "Order",
    href: "/admin/orders",
  },
  {
    icon: <FaUserFriends />,
    label: "Customer",
    href: "/admin/customers",
  },
  {
    icon: <FaStore />,
    label: "Branches",
    href: "/admin/branches",
  },
  {
    icon: <FaBell />,
    label: "Notification",
    href: "/admin/notifications",
  },
  {
    icon: <FaImages />,
    label: "Contact us",
    href: "/admin/contact",
  },
];

export default function Sidebar() {
  const pathname = usePathname();

  return (
    <aside className="w-56 bg-white shadow-md flex flex-col py-6">
      <nav className="flex flex-col gap-1 text-gray-600">
        {sidebarData.map((item) => {
          const isActive = pathname === item.href;

          return (
            <Link
              key={item.href}
              href={item.href}
              className={`flex items-center gap-3 px-6 py-2 border-l-4 transition-colors ${
                isActive
                  ? "text-yellow-500 border-yellow-500 bg-yellow-50"
                  : "border-transparent hover:border-yellow-400 hover:text-yellow-500 hover:bg-yellow-50"
              }`}
            >
              <span className="text-lg">{item.icon}</span>
              <span>{item.label}</span>
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}
