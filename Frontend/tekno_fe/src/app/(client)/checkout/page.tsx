import { Container } from "@/components/MainLayout/Container";
import Stepper from "@/components/share/Stepper";
import React from "react";

export default function CheckoutPage() {
  return (
    <Container className="flex flex-col space-y-5 my-10">
      <Stepper isActive={2} />
    </Container>
  );
}
