import React, { useEffect, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import { CirclePlus } from "lucide-react";
import { ProfileAddress } from "@/type/address";
import { getProfileAddresses } from "@/services/profile";
import NewAddress from "../account/NewAddress";
import EditAddress from "../account/EditAddress";
import AddressItem from "./AddressItem";

export default function MyAddress() {
  const [open, setOpen] = useState(false);
  const [addresses, setAddresses] = useState<ProfileAddress[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        setLoading(true);
        const token = localStorage.getItem("token") || "";
        if (!token) {
          if (mounted) setAddresses([]);
          return;
        }
        const res = await getProfileAddresses(token);
        const list = (res as any)?.data ?? res ?? [];
        if (mounted) setAddresses(Array.isArray(list) ? list : []);
      } catch (e) {
        console.error("Fetch addresses error:", e);
        if (mounted) setAddresses([]);
      } finally {
        if (mounted) setLoading(false);
      }
    })();
    return () => {
      mounted = false;
    };
  }, []);

  return (
    <div className="mt-2">
      <div className="flex items-center justify-between mb-3">
        <h3 className="text-lg font-semibold">My Addresses</h3>
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger className="text-md border rounded-md p-3 border-gray-400">
            <div className="flex items-center gap-2">
              <CirclePlus className="w-5 h-5" />
              New address
            </div>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Create new address</DialogTitle>
              <DialogDescription>
                <NewAddress onClose={() => setOpen(false)} />
              </DialogDescription>
            </DialogHeader>
          </DialogContent>
        </Dialog>
      </div>

      {addresses.length === 0 ? (
        <p className="text-sm text-gray-500">No addresses found.</p>
      ) : (
        <div className="flex flex-col gap-3">
          {addresses.map((addr) => (
            <AddressItem key={addr.id} addr={addr} />
            // <div
            //   key={addr.id}
            //   className="border rounded-md p-3 flex flex-col gap-1 bg-white"
            // >
            //   <div className="flex items-center justify-between">
            //     <span className="font-medium">
            //       {addr.recipientName} · {addr.phoneNumber}
            //     </span>
            //     <div className="flex items-center gap-2">
            //       {addr.isDefault && (
            //         <span className="text-xs px-2 py-1 rounded bg-green-100 text-green-700">
            //           Default
            //         </span>
            //       )}
            //       <Sheet>
            //         <SheetTrigger className="text-xs px-2 py-1 rounded bg-gray-100 text-gray-700">
            //           Edit
            //         </SheetTrigger>
            //         <SheetContent>
            //           <SheetHeader>
            //             <SheetTitle>Edit address {addr.id} </SheetTitle>
            //             <SheetDescription>
            //               Make changes to your profile here. Click save when
            //               you&apos;re done.
            //             </SheetDescription>
            //           </SheetHeader>
            //           <EditAddress address={addr} />
            //         </SheetContent>
            //       </Sheet>
            //     </div>
            //   </div>
            //   <div className="text-sm text-gray-700">{addr.addressLine}</div>
            //   <div className="text-sm text-gray-500">
            //     {addr.wardName}, {addr.districtName}, {addr.provinceName}
            //   </div>
            // </div>
          ))}
        </div>
      )}
    </div>
  );
}
