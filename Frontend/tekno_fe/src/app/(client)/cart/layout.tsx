import AccountMenu from "@/components/account/AccountMenu";
import { Container } from "@/components/MainLayout/Container";
import { Breadcrumb } from "@/components/share/breadcumbCustom";
import React from "react";

export default function CartLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" className="h-full">
      <body>
        <Container className="flex flex-col space-y-5 my-10">
          <div className="mx-auto">sttep</div>
          <main className="w-full">{children}</main>
        </Container>
      </body>
    </html>
  );
}
