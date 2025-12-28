"use client";

import { Container } from "@/components/MainLayout/Container";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import React, { useState } from "react";
import { MapPin, Mail, Phone as PhoneIcon } from "lucide-react";

export default function page() {
  // mock data
  const office = {
    title: "Office",
    addressLine1: "123 Main Street,",
    addressLine2: "Anytown, USA",
  };
  const email = "info@techehim.com";
  const phone = "+1 (555) 123-4567";

  const [name, setName] = useState("");
  const [mail, setMail] = useState("");
  const [message, setMessage] = useState("");

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // mock submit
    alert(`Submitted:\nName: ${name}\nEmail: ${mail}\nMessage: ${message}`);
  };

  return (
    <Container className="grid grid-cols-12 gap-6 py-6">
      <div className="col-span-12">
        <Breadcrumb />
      </div>

      {/* top contact cards */}
      <div className="col-span-12 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="flex items-start gap-3 p-4 rounded-xl bg-white border">
          <div className="w-10 h-10 rounded-full bg-yellow-100 text-yellow-500 flex items-center justify-center">
            <MapPin className="w-5 h-5" />
          </div>
          <div>
            <div className="text-sm font-semibold">Office</div>
            <div className="text-xs text-gray-600">
              {office.addressLine1}
              <br />
              {office.addressLine2}
            </div>
          </div>
        </div>

        <div className="flex items-start gap-3 p-4 rounded-xl bg-white border">
          <div className="w-10 h-10 rounded-full bg-yellow-100 text-yellow-500 flex items-center justify-center">
            <Mail className="w-5 h-5" />
          </div>
          <div>
            <div className="text-sm font-semibold">Email</div>
            <div className="text-xs text-gray-600">{email}</div>
          </div>
        </div>

        <div className="flex items-start gap-3 p-4 rounded-xl bg-white border">
          <div className="w-10 h-10 rounded-full bg-yellow-100 text-yellow-500 flex items-center justify-center">
            <PhoneIcon className="w-5 h-5" />
          </div>
          <div>
            <div className="text-sm font-semibold">Phone</div>
            <div className="text-xs text-gray-600">{phone}</div>
          </div>
        </div>
      </div>

      {/* body: message text + form */}
      <div className="col-span-12 grid grid-cols-1 md:grid-cols-2 gap-8 mt-2">
        {/* left description */}
        <div className="border rounded-xl p-4">
          <div className="text-base font-semibold mb-2">Message us</div>
          <div className="text-sm text-gray-700 leading-6">
            We're here to assist you every step of the way. Whether you have a
            question, need technical support, or simply want to share your
            feedback, our dedicated team is ready to listen and provide prompt
            assistance.
          </div>
        </div>

        {/* right form */}
        <form
          onSubmit={onSubmit}
          className="flex flex-col gap-3 border rounded-xl p-4 bg-white"
        >
          <input
            type="text"
            placeholder="* Your name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full border rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
            required
          />
          <input
            type="email"
            placeholder="* Email"
            value={mail}
            onChange={(e) => setMail(e.target.value)}
            className="w-full border rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
            required
          />
          <textarea
            placeholder="Message"
            rows={6}
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            className="w-full border rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
          />
          <button
            type="submit"
            className="mt-2 w-32 rounded-lg bg-yellow-400 px-4 py-2 text-sm font-medium text-white hover:bg-yellow-500"
          >
            Submit
          </button>
        </form>
      </div>
    </Container>
  );
}
