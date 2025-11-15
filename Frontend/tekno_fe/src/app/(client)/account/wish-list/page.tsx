import TitleAccount from "@/components/account/TitleAccount";
import React from "react";

export default function page() {
  return (
    <div className="flex flex-col gap-4">
      <TitleAccount title="Wish list" des="See your favorites list here" />
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3">
        <div>product</div>
        <div>product</div>
        <div>product</div>
      </div>
    </div>
  );
}
