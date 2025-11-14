import { Textarea } from "@/components/ui/textarea";
import React from "react";
import CommentItemView from "./CommentItemView";

export default function Comments() {
  return (
    <div className="flex flex-col md:flex-row gap-10">
      <div className=" flex flex-col w-full md:w-1/4 gap-2">
        <div className="font-bold text-xl">Comments</div>
        <p className="text-gray-500 font-normal">
          Leave your comments here for other customers
        </p>

        <Textarea placeholder="Share your thoughts about this product here" />
        <button className="border border-primary hover:bg-gray-50 rounded-md py-2 text-primary font-normal text-2xl">
          Comment
        </button>

        <p className="h6">By feature</p>
      </div>
      <div className="flex flex-col w-full md:w-3/4 gap-2">
        <CommentItemView />
        <CommentItemView />
      </div>
    </div>
  );
}
