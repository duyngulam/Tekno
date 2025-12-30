"use client";
import React, { useEffect, useState } from "react";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import { Label } from "@/components/ui/label";
import {
  CirclePlus,
  Edit,
  MailIcon,
  MapPinHouse,
  Milestone,
  Phone,
  Plug,
  SquarePen,
  UserRound,
} from "lucide-react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import TitleAccount from "@/components/account/TitleAccount";
import { getProfile, getProfileAddresses, Profile } from "@/services/profile";
import { updateProfileAll, deleteProfileAddress } from "@/services/profile";
import { ProfileAddress } from "@/type/address";
import { Button } from "@/components/ui/button";
import EditAddress from "@/components/account/EditAddress";
import NewAddress from "@/components/account/NewAddress";
import { toast } from "sonner";

export default function Page() {
  const [profile, setProfile] = useState<Profile>();
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [open, setOpen] = useState(false);
  const [openAddressSheet, setOpenAddressSheet] = useState(false);
  const [addresses, setAddresses] = useState<ProfileAddress[]>([]);

  // Local state để edit
  const [fullname, setFullname] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");

  // Password modal state
  const [openPasswordModal, setOpenPasswordModal] = useState(false);
  const [currentPassword, setCurrentPassword] = useState("");
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const handleDeleteAddress = async (addressId: number) => {
    try {
      setDeleting(true);
      const token = localStorage.getItem("token") || "";
      if (!token) throw new Error("Missing token");

      await deleteProfileAddress(token, addressId);

      // Refresh addresses list
      const updatedAddresses = addresses.filter(
        (addr) => addr.id !== addressId
      );
      setAddresses(updatedAddresses);

      toast.success("Đã xóa địa chỉ thành công");
    } catch (e: any) {
      toast.error(e?.message || "Xóa địa chỉ thất bại");
    } finally {
      setDeleting(false);
    }
  };

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (!token) {
      setLoading(false);
      return;
    }

    getProfile(token)
      .then((res) => {
        setProfile(res);
        setFullname(res.fullName);
        setEmail(res.email);
        setPhoneNumber(res.phoneNumber);
      })
      .catch((err) => {
        console.error("Fetch profile error:", err);
      })
      .finally(() => {
        setLoading(false);
      });

    // fetch addresses
    (async () => {
      try {
        const list = await getProfileAddresses(token);
        setAddresses(Array.isArray(list) ? list : []);
      } catch (e) {
        console.error("Fetch addresses error:", e);
        setAddresses([]);
      }
    })();
  }, []);

  if (loading) return <p>Loading...</p>;

  const handleSave = async () => {
    // mở modal yêu cầu nhập password trước khi lưu
    setOpenPasswordModal(true);
  };

  const confirmSave = async () => {
    const token = localStorage.getItem("token");
    if (!token) return;
    if (!currentPassword.trim()) return;

    try {
      setSaving(true);
      const payload = {
        fullname,
        newEmail: email,
        phoneNumber,
        currentPassword, // truyền password vào payload
      };

      await updateProfileAll(token, payload);

      setEditing(false);
      setOpenPasswordModal(false);
      setCurrentPassword("");
    } catch (err) {
      console.error("Update failed:", err);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="flex flex-col gap-4">
      <TitleAccount title="Identification" des="Verify your identity" />

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {/* Full Name */}
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="name">Full Name</Label>
          <InputGroup>
            <InputGroupInput
              disabled={!editing}
              type="text"
              id="name"
              placeholder="Full name"
              value={fullname}
              onChange={(e) => setFullname(e.target.value)}
            />
            <InputGroupAddon>
              <UserRound />
            </InputGroupAddon>
          </InputGroup>
        </div>

        {/* Email */}
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="email">Email Address</Label>
          <InputGroup>
            <InputGroupInput
              disabled={!editing}
              type="email"
              id="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <InputGroupAddon>
              <MailIcon />
            </InputGroupAddon>
          </InputGroup>
        </div>

        {/* Phone */}
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="phone">Phone number</Label>
          <InputGroup>
            <InputGroupInput
              disabled={!editing}
              type="number"
              id="phone"
              placeholder="Phone number"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
            />
            <InputGroupAddon>
              <Phone />
            </InputGroupAddon>
          </InputGroup>
        </div>

        {/* Address - không cho sửa */}
        {/* <div className="grid w-full items-center gap-3">
          <Label htmlFor="address">Address</Label>
          <InputGroup>
            <InputGroupInput
              disabled
              type="text"
              id="address"
              placeholder="Address"
              defaultValue={profile?.addresses[0].addressLine1 ?? ""}
            />
            <InputGroupAddon>
              <MapPinHouse />
            </InputGroupAddon>
          </InputGroup>
        </div> */}

        {/* Postal Code - không cho sửa */}
        {/* <div className="grid w-full items-center gap-3">
          <Label htmlFor="postal">Postal Code</Label>
          <InputGroup>
            <InputGroupInput
              disabled
              type="text"
              id="postal"
              placeholder="Postal code"
              defaultValue={profile?.addresses[0].postalCode ?? ""}
            />
            <InputGroupAddon>
              <Milestone />
            </InputGroupAddon>
          </InputGroup>
        </div> */}
      </div>

      <div>
        {editing ? (
          <button
            onClick={handleSave}
            className="w-full px-10 py-2 flex items-center justify-center gap-2 my-2 bg-primary text-white rounded-md hover:bg-primary/90"
          >
            Lưu
          </button>
        ) : (
          <button
            onClick={() => setEditing(true)}
            className="w-full px-10 py-2 flex items-center justify-center gap-2 my-2 hover:border hover:border-primary hoverEffects rounded-md"
          >
            <SquarePen />
            Chỉnh sửa
          </button>
        )}
      </div>

      {/* Modal nhỏ yêu cầu nhập password trước khi lưu */}
      <Dialog open={openPasswordModal} onOpenChange={setOpenPasswordModal}>
        <DialogContent className="sm:max-w-[360px]">
          <DialogHeader>
            <DialogTitle>Confirm changes</DialogTitle>
            <DialogDescription>
              Enter your current password to save profile changes.
            </DialogDescription>
          </DialogHeader>

          <div className="grid w-full items-center gap-3">
            <Label htmlFor="currentPassword">Current password</Label>
            <InputGroup>
              <InputGroupInput
                id="currentPassword"
                type="password"
                placeholder="Enter password"
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
              />
              <InputGroupAddon>
                <Plug />
              </InputGroupAddon>
            </InputGroup>
          </div>

          <div className="flex justify-end gap-2 mt-4">
            <Button
              variant="outline"
              type="button"
              onClick={() => {
                setOpenPasswordModal(false);
                setCurrentPassword("");
              }}
            >
              Cancel
            </Button>
            <Button
              type="button"
              onClick={confirmSave}
              disabled={saving || !currentPassword.trim()}
            >
              {saving ? "Saving..." : "Save"}
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Addresses list */}
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
              <div
                key={addr.id}
                className="border rounded-md p-3 flex flex-col gap-1 bg-white"
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
                    <Sheet
                      open={openAddressSheet}
                      onOpenChange={setOpenAddressSheet}
                    >
                      <SheetTrigger className="text-xs px-2 py-1 rounded bg-gray-100 text-gray-700">
                        Edit
                      </SheetTrigger>
                      <SheetContent>
                        <SheetHeader>
                          <SheetTitle>Edit address {addr.id} </SheetTitle>
                          <SheetDescription>
                            Make changes to your profile here. Click save when
                            you&apos;re done.
                          </SheetDescription>
                        </SheetHeader>
                        <EditAddress
                          address={addr}
                          onClose={() => setOpenAddressSheet(false)}
                        />
                      </SheetContent>
                    </Sheet>

                    <AlertDialog>
                      <AlertDialogTrigger asChild>
                        <Button variant="destructive" size="sm">
                          Xóa
                        </Button>
                      </AlertDialogTrigger>
                      <AlertDialogContent>
                        <AlertDialogHeader>
                          <AlertDialogTitle>
                            Xác nhận xóa địa chỉ
                          </AlertDialogTitle>
                          <AlertDialogDescription>
                            Bạn có chắc chắn muốn xóa địa chỉ "
                            {addr.addressLine}, {addr.wardName},{" "}
                            {addr.districtName}, {addr.provinceName}" không?
                            Hành động này không thể hoàn tác.
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel disabled={deleting}>
                            Hủy
                          </AlertDialogCancel>
                          <AlertDialogAction
                            onClick={() => handleDeleteAddress(addr.id)}
                            disabled={deleting}
                            className="bg-red-600 hover:bg-red-700"
                          >
                            {deleting ? "Đang xóa..." : "Xóa"}
                          </AlertDialogAction>
                        </AlertDialogFooter>
                      </AlertDialogContent>
                    </AlertDialog>
                  </div>
                </div>
                <div className="text-sm text-gray-700">{addr.addressLine}</div>
                <div className="text-sm text-gray-500">
                  {addr.wardName}, {addr.districtName}, {addr.provinceName}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
