import TitleAccount from "@/components/account/TitleAccount";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Eye } from "lucide-react";
import React from "react";

export default function page() {
  return (
    <div className="flex flex-col gap-4">
      <TitleAccount
        title="Discounts & Voucher"
        des="Add discount code to apply a discount in your purchase"
      />
      <InputGroup>
        <InputGroupInput placeholder="Input Discount or your voucher" />
        <InputGroupAddon align="inline-end">
          <Eye />
        </InputGroupAddon>
      </InputGroup>

      <Accordion type="single" collapsible className="m-0 p-0">
        <AccordionItem value="item-1" className="border-none">
          <AccordionTrigger className="font-normal hover:text-black">
            Where can I find the discount code ?
          </AccordionTrigger>
          <AccordionContent>
            Yes. It adheres to the WAI-ARIA design pattern.
          </AccordionContent>
        </AccordionItem>
      </Accordion>
    </div>
  );
}
