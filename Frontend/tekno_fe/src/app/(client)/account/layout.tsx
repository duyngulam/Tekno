import AccountMenu from "@/components/account/AccountMenu";
import { Container } from "@/components/MainLayout/Container";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import React from "react";

export default function AccountLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" className="h-full">
      <body>
        <Container className="flex flex-col space-y-5 my-10">
          <Breadcrumb />
          <div className="flex gap-2">
            <div className="w-1/4 hidden md:inline-flex">
              <AccountMenu />
            </div>
            <main className="w-full md:w-3/4">{children}</main>
          </div>
        </Container>
      </body>
    </html>
  );
}
