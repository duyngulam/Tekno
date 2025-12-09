import React from "react";
import { Card, CardContent, CardHeader, CardTitle } from "../ui/card";
import SignIn from "../MainLayout/Header/SignIn";
import { Button } from "../ui/button";

export default function NoAccess({
  detail = "Log in to view your cart item and checkout. Don't miss out on your favorite products",
}: {
  detail?: string;
}) {
  return (
    <div className="flex items-center justify-center py-12 md:py-32 bg-gray-100 p-4">
      <Card className="w-full max-w-md">
        <CardHeader className="flex items-center flex-col">
          <p>Tekno</p>
          <CardTitle className="text-2xl font-bold text-center">
            Welcome Back
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-center font-medium text-secondary/80">{detail}</p>

          <SignIn />
        </CardContent>
      </Card>
    </div>
  );
}
