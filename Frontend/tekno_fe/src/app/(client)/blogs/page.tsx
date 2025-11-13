"use client";

import React, { useState, useEffect } from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";
import {
  TrendingDown,
  DollarSign,
  FileText,
  ChevronDown,
} from "lucide-react";

// =================== TẠO DỮ LIỆU MẪU ===================
const generateLast7Days = () => {
  const data = [];
  const today = new Date();

  for (let i = 6; i >= 0; i--) {
    const date = new Date(today);
    date.setDate(today.getDate() - i);
    const day = date.getDate().toString().padStart(2, "0");
    const month = date.getMonth() + 1;
    data.push({
      day: `${day}/${month}`,
      sales: Math.floor(Math.random() * 800) + 200,
      refund: Math.floor(Math.random() * 100),
    });
  }
  return data;
};

const generateLastMonthDays = () => {
  const today = new Date();
  const lastMonth = today.getMonth() === 0 ? 11 : today.getMonth() - 1;
  const year = lastMonth === 11 ? today.getFullYear() - 1 : today.getFullYear();
  const daysInMonth = new Date(year, lastMonth + 1, 0).getDate();
  const data = [];

  for (let i = 1; i <= daysInMonth; i++) {
    data.push({
      day: i.toString().padStart(2, "0"),
      sales: Math.floor(Math.random() * 1000) + 200,
      refund: Math.floor(Math.random() * 100),
    });
  }
  return data;
};

// =================== DỮ LIỆU KHÁC ===================
const topProductsRevenue = [
  { name: "Bột bánh bao quốc tế", value: 120 },
  { name: "Dây Paracord", value: 95 },
  { name: "Gạo ST25", value: 80 },
];

const topProductsQuantity = [
  { name: "Bột bánh bao quốc tế", value: 45 },
  { name: "Gạo ST25", value: 38 },
  { name: "Dây Paracord", value: 27 },
];

const topCustomers = [
  { name: "An Giang - Kim Mỹ", value: 3.4 },
  { name: "Anh Hoàng - HN", value: 3.1 },
  { name: "Phạm Thu Hằng", value: 2.9 },
  { name: "Tuấn - Hà Nội", value: 2.6 },
  { name: "Nguyễn Văn Hải", value: 2.1 },
];

const activities = [
  { user: "Hoàng - Kinh Doanh", action: "bán đơn hàng", value: "250,000", time: "1 ngày trước" },
  { user: "Hương - Kế Toán", action: "bán đơn hàng", value: "1,169,000", time: "2 ngày trước" },
  { user: "Nguyễn Anh Thi", action: "nhập hàng", value: "389,000", time: "5 ngày trước" },
];

// =================== COMPONENT ===================
export default function AdminDashboard() {
  const [selectedPeriod, setSelectedPeriod] = useState("7 ngày qua");
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [salesData, setSalesData] = useState<any[]>([]);
  const [totalRevenue, setTotalRevenue] = useState(0);

  // Dropdown cho Top sản phẩm
  const [topBy, setTopBy] = useState("Theo doanh thu");
  const [topDropdownOpen, setTopDropdownOpen] = useState(false);

  // Tự động sinh dữ liệu theo thời gian
  useEffect(() => {
    let data: any[] = [];
    if (selectedPeriod === "7 ngày qua") {
      data = generateLast7Days();
    } else {
      data = generateLastMonthDays();
    }
    setSalesData(data);

    const total = data.reduce((sum, item) => sum + item.sales, 0);
    setTotalRevenue(total);
  }, [selectedPeriod]);

  const handleSelectPeriod = (period: string) => {
    setSelectedPeriod(period);
    setDropdownOpen(false);
  };

  const handleSelectTopBy = (option: string) => {
    setTopBy(option);
    setTopDropdownOpen(false);
  };

  const displayedProducts =
    topBy === "Theo doanh thu" ? topProductsRevenue : topProductsQuantity;

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
      {/* Cột trái: biểu đồ + top */}
      <div className="col-span-2 flex flex-col gap-6">
        {/* Tổng quan doanh thu */}
        <div className="grid grid-cols-3 gap-4">
          <div className="bg-white p-4 rounded-lg shadow border border-gray-100">
            <div className="flex items-center gap-3">
              <DollarSign className="text-primary" />
              <h3 className="font-semibold text-gray-700">2 Hóa đơn</h3>
            </div>
            <p className="text-2xl font-bold text-secondary mt-2">1,342,000 ₫</p>
            <p className="text-sm text-gray-500">Doanh thu thuần hôm nay</p>
          </div>

          <div className="bg-white p-4 rounded-lg shadow border border-gray-100">
            <div className="flex items-center gap-3">
              <FileText className="text-red-500" />
              <h3 className="font-semibold text-gray-700">0 Phiếu</h3>
            </div>
            <p className="text-2xl font-bold text-red-500 mt-2">Trả hàng</p>
            <p className="text-sm text-gray-500">So với kỳ trước</p>
          </div>

          <div className="bg-white p-4 rounded-lg shadow border border-gray-100">
            <div className="flex items-center gap-3">
              <TrendingDown className="text-red-600" />
              <h3 className="font-semibold text-gray-700">Giảm</h3>
            </div>
            <p className="text-2xl font-bold text-red-600 mt-2">-97.06%</p>
            <p className="text-sm text-gray-500">So với cùng kỳ tháng trước</p>
          </div>
        </div>

        {/* DOANH THU THUẦN */}
        <div className="bg-white p-6 rounded-lg shadow border border-gray-100 relative">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h2 className="font-semibold text-secondary text-lg">
                Doanh thu thuần ({selectedPeriod})
              </h2>
              <p className="text-sm text-gray-600 mt-1">
                Tổng doanh thu:{" "}
                <span className="text-primary font-bold text-base">
                  {totalRevenue.toLocaleString("vi-VN")} ₫
                </span>
              </p>
            </div>

            {/* Dropdown thời gian */}
            <div className="relative">
              <button
                onClick={() => setDropdownOpen(!dropdownOpen)}
                className="flex items-center gap-1 border border-gray-300 rounded-md px-3 py-1 text-sm text-gray-700 hover:border-primary transition"
              >
                {selectedPeriod}
                <ChevronDown size={16} className="text-gray-500" />
              </button>

              {dropdownOpen && (
                <div className="absolute right-0 mt-1 w-40 bg-white border border-gray-200 rounded-md shadow-lg z-10">
                  {["7 ngày qua", "Tháng trước"].map((option) => (
                    <button
                      key={option}
                      onClick={() => handleSelectPeriod(option)}
                      className={`w-full text-left px-4 py-2 text-sm hover:bg-primary/10 ${
                        selectedPeriod === option
                          ? "text-primary font-semibold"
                          : "text-gray-700"
                      }`}
                    >
                      {option}
                    </button>
                  ))}
                </div>
              )}
            </div>
          </div>

          <div className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={salesData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="day" />
                <YAxis />
                <Tooltip
                  formatter={(value: number) =>
                    value.toLocaleString("vi-VN") + " ₫"
                  }
                />
                <Legend />
                <Bar dataKey="sales" fill="#00296B" name="Doanh thu" />
                <Bar dataKey="refund" fill="#FFD500" name="Trả hàng" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* TOP SẢN PHẨM & KHÁCH HÀNG */}
        <div className="grid grid-cols-2 gap-4">
          {/* TOP SẢN PHẨM */}
          <div className="bg-white p-5 rounded-lg shadow border border-gray-100 relative">
            <div className="flex items-center justify-between mb-4">
              <h3 className="font-semibold text-secondary">
                Top sản phẩm bán chạy
              </h3>

              {/* Dropdown Theo doanh thu / Số lượng */}
              <div className="relative">
                <button
                  onClick={() => setTopDropdownOpen(!topDropdownOpen)}
                  className="flex items-center gap-1 border border-gray-300 rounded-md px-3 py-1 text-sm text-gray-700 hover:border-primary transition"
                >
                  {topBy}
                  <ChevronDown size={16} className="text-gray-500" />
                </button>

                {topDropdownOpen && (
                  <div className="absolute right-0 mt-1 w-48 bg-white border border-gray-200 rounded-md shadow-lg z-10">
                    {["Theo doanh thu", "Theo số lượng"].map((option) => (
                      <button
                        key={option}
                        onClick={() => handleSelectTopBy(option)}
                        className={`w-full text-left px-4 py-2 text-sm hover:bg-primary/10 ${
                          topBy === option
                            ? "text-primary font-semibold"
                            : "text-gray-700"
                        }`}
                      >
                        {option}
                      </button>
                    ))}
                  </div>
                )}
              </div>
            </div>

            {displayedProducts.map((p) => (
              <div key={p.name} className="flex justify-between text-sm mb-2">
                <span className="text-gray-700">{p.name}</span>
                <span className="font-semibold text-secondary">{p.value}</span>
              </div>
            ))}
          </div>

          {/* TOP KHÁCH HÀNG */}
          <div className="bg-white p-5 rounded-lg shadow border border-gray-100">
            <h3 className="font-semibold text-secondary mb-4">
              Top khách hàng mua nhiều nhất
            </h3>
            {topCustomers.map((c) => (
              <div key={c.name} className="flex justify-between text-sm mb-2">
                <span className="text-gray-700">{c.name}</span>
                <span className="font-semibold text-secondary">{c.value}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Cột phải: Hoạt động gần đây */}
      <div className="bg-white p-5 rounded-lg shadow border border-gray-100">
        <h3 className="font-semibold text-secondary mb-4">
          Hoạt động gần đây
        </h3>
        <ul className="divide-y divide-gray-100">
          {activities.map((a, i) => (
            <li key={i} className="py-3 text-sm">
              <p className="text-gray-700">
                <span className="font-medium text-secondary">{a.user}</span>{" "}
                vừa {a.action} với giá trị{" "}
                <span className="text-primary font-semibold">{a.value} ₫</span>
              </p>
              <p className="text-xs text-gray-500">{a.time}</p>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
