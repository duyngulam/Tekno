import React from "react";

export default function page() {
  return (
    <div className="max-w-[1200px] mx-auto grid grid-cols-12 gap-6 px-4 py-8">
      {/* Table of contents (sidebar) */}
      <aside className="col-span-12 md:col-span-3 bg-gray-50 p-4 rounded">
        <h3 className="font-semibold mb-2">Table of Contents</h3>
        <ul className="space-y-2 text-yellow-700">
          <li>Payment</li>
          <li>Trust & Safety</li>
          <li>Services</li>
          <li>Billing</li>
        </ul>
      </aside>

      {/* FAQ content */}
      <section className="col-span-12 md:col-span-9 space-y-6">
        <div>
          <h4 className="text-yellow-700 font-semibold">
            Can I purchase products from Tech Heim using installment payments?
          </h4>
          <p className="text-gray-700">
            Yes, Tech Heim offers the option to purchase products using both
            cash and installment payments. This allows you to choose the payment
            method that suits your needs and budget.
          </p>
        </div>

        {/* Các câu hỏi khác tương tự */}
      </section>
    </div>
  );
}
