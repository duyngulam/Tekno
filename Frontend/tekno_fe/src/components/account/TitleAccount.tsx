import React from "react";

export default function TitleAccount({
  title,
  des,
}: {
  title: string;
  des: string;
}) {
  return (
    <div className="flex flex-col">
      <p className="text-2xl font-bold">{title}</p>
      <p>{des}</p>
    </div>
  );
}
