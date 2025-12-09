"use client";
import React, { useEffect, useState } from "react";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupInput,
} from "@/components/ui/input-group";
import { Label } from "@/components/ui/label";
import {
  MailIcon,
  MapPinHouse,
  Milestone,
  Phone,
  SquarePen,
  UserRound,
} from "lucide-react";
import TitleAccount from "@/components/account/TitleAccount";
import { getProfile, Profile } from "@/services/profile";
import { updateProfileAll } from "@/services/profile";

export default function Page() {
  const [profile, setProfile] = useState<Profile>();
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);

  // Local state để edit
  const [fullname, setFullname] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (!token) {
      setLoading(false);
      return;
    }

    getProfile(token)
      .then((res) => {
        setProfile(res);

        // Set vào form
        setFullname(res.fullname);
        setEmail(res.email);
        setPhoneNumber(res.phoneNumber);
      })
      .catch((err) => {
        console.error("Fetch profile error:", err);
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  if (loading) return <p>Loading...</p>;

  const handleSave = async () => {
    const token = localStorage.getItem("token");
    if (!token) return;

    try {
      const payload = {
        fullname,
        newEmail: email,
        phoneNumber,
      };

      await updateProfileAll(token, payload);

      setEditing(false);
    } catch (err) {
      console.error("Update failed:", err);
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
        <div className="grid w-full items-center gap-3">
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
        </div>

        {/* Postal Code - không cho sửa */}
        <div className="grid w-full items-center gap-3">
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
        </div>
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
    </div>
  );
}
