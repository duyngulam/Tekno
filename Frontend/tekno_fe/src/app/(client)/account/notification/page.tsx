import TitleAccount from "@/components/account/TitleAccount";
import React from "react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import NoOrderAvailable from "@/asset/NoOrderAvailable.svg";
import Image from "next/image";
import { Mail, Truck } from "lucide-react";
import { Switch } from "@/components/ui/switch";

export default function page() {
  return (
    <div className="flex flex-col gap-4">
      <TitleAccount
        title="Notification"
        des="Manage your notification settings"
      />
      <div className="grid grid-cols-1 md:grid-cols-2 gap-10">
        {/* email */}
        <div className="flex flex-col border-b border-gray-200 md:border-hidden">
          <div className="flex items-center justify-between py-2">
            <div className="flex items-center justify-center gap-2">
              <Mail />
              <p className="font-semibold">Emails</p>
            </div>
            <Switch />
          </div>
          <p className="text-gray-600 pb-2">
            We write emails to let you know what's important, like: new order,
            confirmations ETC.
          </p>
        </div>

        {/* Order Delivered */}
        <div className="flex flex-col border-b border-gray-200 md:border-hidden">
          <div className="flex items-center justify-between py-2">
            <div className="flex items-center justify-center gap-2">
              <Truck />
              <p className="font-semibold">Order Delivered</p>
            </div>
            <Switch />
          </div>
          <p className="text-gray-600 pb-2">
            You will be noticed once the order is delivered
          </p>
        </div>

        {/* Push to your Device */}
        <div className="flex flex-col border-b border-gray-200 md:border-hidden">
          <div className="flex items-center justify-between py-2">
            <div className="flex items-center justify-center gap-2">
              <Mail />
              <p className="font-semibold">Push to your Device</p>
            </div>
            <Switch />
          </div>
          <p className="text-gray-600 pb-2">
            Receive notifications about your order status, promotions and other
            updates
          </p>
        </div>

        {/* Product's availibilty */}
        <div className="flex flex-col border-b border-gray-200 md:border-hidden">
          <div className="flex items-center justify-between py-2">
            <div className="flex items-center justify-center gap-2">
              <Mail />
              <p className="font-semibold">Product's availibilty</p>
            </div>
            <Switch />
          </div>
          <p className="text-gray-600 pb-2">
            You will be noticed when product gets available.
          </p>
        </div>
      </div>
    </div>
  );
}
