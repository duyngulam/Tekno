import TitleAccount from "@/components/account/TitleAccount";
import React from "react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import NoOrderAvailable from "@/asset/NoOrderAvailable.svg";
import Image from "next/image";

export default function page() {
  return (
    <div className="flex flex-col gap-4">
      <TitleAccount
        title="Order History"
        des="Track, return or purchase items"
      />

      <Tabs defaultValue="account" className="w-full">
        <TabsList className="">
          <TabsTrigger value="account">Account</TabsTrigger>
          <TabsTrigger value="password">Password</TabsTrigger>
        </TabsList>
        <TabsContent value="account">
          <div className="flex flex-col gap-2 items-center justify-center">
            <div>
              <Image
                src={NoOrderAvailable}
                alt="banner"
                className="md:inline-flex w-82"
              />
            </div>
            <p>You have not placed any orders yet</p>
          </div>
        </TabsContent>
        <TabsContent value="password">Change your password here.</TabsContent>
      </Tabs>
    </div>
  );
}
