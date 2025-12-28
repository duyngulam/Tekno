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
          ))}
        </div>
      )}
    </div>
  );
}
