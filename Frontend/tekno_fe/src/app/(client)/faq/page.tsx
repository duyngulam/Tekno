import React from "react";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import faqBanner from "./../../../../public/faq.png"; // ✅ đổi tên biến cho đúng chuẩn
import Image from "next/image";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import { Container } from "@/components/MainLayout/Container";

export default function Page() {
  return (
    <>
      <div className="max-w-[1200px] mx-auto grid grid-cols-12 gap-6 px-4 py-8">
        <Breadcrumb />
        {/* Banner */}
        <Image
          src={faqBanner}
          alt="FAQ Banner"
          className="col-span-12 rounded-lg mb-6 w-full object-cover"
        />

        {/* Table of contents (sidebar) */}
        <aside className="col-span-12 md:col-span-3 bg-gray-50 p-4 rounded-lg shadow-sm">
          <h3 className="font-semibold mb-3 text-gray-800">
            Table of Contents
          </h3>
          <ul className="space-y-2 text-yellow-700">
            <li className="hover:underline cursor-pointer">Payment</li>
            <li className="hover:underline cursor-pointer">Trust & Safety</li>
            <li className="hover:underline cursor-pointer">Services</li>
            <li className="hover:underline cursor-pointer">Billing</li>
          </ul>
        </aside>

        {/* FAQ content */}
        <section className="col-span-12 md:col-span-9 space-y-6">
          <Accordion type="multiple">
            <AccordionItem value="item-1">
              <AccordionTrigger className="text-yellow-700 font-medium">
                Is it accessible?
              </AccordionTrigger>
              <AccordionContent className="text-gray-700">
                Yes. It adheres to the WAI-ARIA design pattern.
              </AccordionContent>
            </AccordionItem>

            <AccordionItem value="item-2">
              <AccordionTrigger className="text-yellow-700 font-medium">
                How secure is my information?
              </AccordionTrigger>
              <AccordionContent className="text-gray-700">
                Your data is encrypted and stored following industry best
                practices.
              </AccordionContent>
            </AccordionItem>
          </Accordion>
        </section>
      </div>
    </>
  );
}
