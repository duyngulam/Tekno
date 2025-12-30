import { ProfileAddress } from "@/type/address";
import React from "react";

import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import EditAddress from "../account/EditAddress";
import DeleteAddress from "../account/DeleteAddress";

export default function AddressItem({
  addr,
  className,
}: {
  addr: ProfileAddress;
  className?: string;
}) {
  return (
    <div
      className={`rounded-md p-3 flex flex-col gap-1 bg-white ${
        className || ""
      }  `}
    >
      <div className="flex items-center justify-between">
        <span className="font-medium">
          {addr.recipientName} · {addr.phoneNumber}
        </span>
        <div className="flex items-center gap-2">
          {addr.isDefault && (
            <span className="text-xs px-2 py-1 rounded bg-green-100 text-green-700">
              Default
            </span>
          )}
          <Sheet>
            <SheetTrigger className="text-xs px-2 py-1 rounded bg-gray-100 text-gray-700 hover:bg-gray-200">
              Edit
            </SheetTrigger>
            <SheetContent>
              <SheetHeader>
                <SheetTitle>Edit address {addr.id} </SheetTitle>
                <SheetDescription>
                  Make changes to your profile here. Click save when you&apos;re
                  done.
                </SheetDescription>
              </SheetHeader>
              <EditAddress address={addr} />
            </SheetContent>
          </Sheet>

          <Sheet>
            <SheetTrigger className="text-xs px-2 py-1 rounded bg-red-100 text-red-700 hover:bg-red-200">
              Delete
            </SheetTrigger>
            <SheetContent>
              <SheetHeader>
                <SheetTitle>Delete address</SheetTitle>
                <SheetDescription>
                  Are you sure you want to delete this address? This action
                  cannot be undone.
                </SheetDescription>
              </SheetHeader>
              <DeleteAddress address={addr} />
            </SheetContent>
          </Sheet>
        </div>
      </div>
      <div className="text-sm text-gray-700">{addr.addressLine}</div>
      <div className="text-sm text-gray-500">
        {addr.wardName}, {addr.districtName}, {addr.provinceName}
      </div>
    </div>
  );
}
