import { Hand, Star, ThumbsDown, ThumbsUp } from "lucide-react";
import React from "react";

export default function CommentItemView() {
  return (
    <div className="flex flex-col gap-2 border border-gray-300 bg-gray-50 p-3 rounded-2xl">
      <div className="flex justify-between">
        {/* thong tin ng cmt */}
        <div className="flex items-center gap-4">
          <div className="rounded-full">avt</div>
          <div className="flex flex-col">
            <div className="text-xl font-bold">Name</div>
            <div className="text-gray-500 font-normal text-sm"> Date</div>
          </div>
        </div>
        {/* danh gia */}
        <div className="flex items-center justify-center gap-1 rounded-xl bg-primary text-white px-3 py-1">
          <Star fill="white" className="h-5 w-5" />
          <p className="text-md font-normal">5.0</p>
        </div>
      </div>
      <div className="text-black font-normal">
        The MacBook air is the best laptop for most people , thanks to its
        blazing speed ,slim design and reasonable price , while the 14-inch and
        16-inch MacBook Pros provideslots of ports,more advanced displayed and
        even more power for creative professional
      </div>
      <div className="flex items-center justify-end gap-2">
        <button className="flex items-center justify-center gap-1 hover:text-primary">
          <ThumbsUp />
          <span>19</span>
        </button>
        <div className="bg-gray-500 rounded-2xl w-0.5 h-full" />
        <button className="flex items-center justify-center gap-1 hover:text-primary">
          <ThumbsDown />
          <span>19</span>
        </button>
      </div>
    </div>
  );
}
