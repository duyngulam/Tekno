import React from "react";
import {
  InputGroup,
  InputGroupAddon,
  InputGroupButton,
  InputGroupInput,
} from "@/components/ui/input-group";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import {
  KeyRound,
  MailIcon,
  MapPinHouse,
  Milestone,
  Phone,
  SquarePen,
  UserRound,
} from "lucide-react";
import TitleAccount from "@/components/account/TitleAccount";

export default function page() {
  return (
    <div className="flex flex-col gap-4">
      {/* title */}
      <TitleAccount title="Identification" des="Verify your identity" />
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="name">Full Name</Label>
          <InputGroup>
            <InputGroupInput
              disabled
              type="text"
              id="name"
              placeholder="Email"
            />
            <InputGroupAddon>
              <UserRound />
            </InputGroupAddon>
          </InputGroup>
        </div>
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="email">Email Address</Label>
          <InputGroup>
            <InputGroupInput
              disabled
              type="email"
              placeholder="Enter your email"
            />
            <InputGroupAddon>
              <MailIcon />
            </InputGroupAddon>
          </InputGroup>
        </div>
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="phone">Phone number</Label>

          <InputGroup>
            <InputGroupInput
              disabled
              type="number"
              id="phone"
              placeholder="Email"
            />
            <InputGroupAddon>
              <Phone />
            </InputGroupAddon>
          </InputGroup>
        </div>
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="password">Password</Label>

          <InputGroup>
            <InputGroupInput
              disabled
              type="password"
              id="password"
              placeholder="Email"
            />
            <InputGroupAddon>
              <KeyRound />
            </InputGroupAddon>
          </InputGroup>
        </div>
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="address">Address</Label>

          <InputGroup>
            <InputGroupInput
              disabled
              type="text"
              id="address"
              placeholder="Email"
            />
            <InputGroupAddon>
              <MapPinHouse />
            </InputGroupAddon>
          </InputGroup>
        </div>
        <div className="grid w-full items-center gap-3">
          <Label htmlFor="postal"></Label>

          <InputGroup>
            <InputGroupInput
              disabled
              type="text"
              id="postal"
              placeholder="Postal code"
            />
            <InputGroupAddon>
              <Milestone />
            </InputGroupAddon>
          </InputGroup>
        </div>
      </div>
      <div>
        <button className="w-full px-10 py-2 flex items-center justify-center gap-2 my-2 hover:border hover:border-primary hoverEffects rounded-md font-normal">
          <SquarePen />
          Chỉnh sửa
        </button>
      </div>
    </div>
  );
}
