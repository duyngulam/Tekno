"use client";

import React, { useEffect, useState } from "react";
import { Textarea } from "@/components/ui/textarea";
import CommentItemView from "./CommentItemView";

import {
  ProductReview,
  ProductReviewsResponse,
  ReviewSummary,
} from "@/type/review";
import { getProductReviews } from "@/services/review";

export default function Comments({ productId }: { productId: number }) {
  const [response, setResponse] = useState<ProductReviewsResponse | null>(null);
  const [reviews, setReviews] = useState<ProductReview[]>([]);
  const [summary, setSummary] = useState<ReviewSummary | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        setLoading(true);
        const res = await getProductReviews({ productId });
        if (mounted) setResponse(res.data);
      } catch (e) {
        console.error("fetch product reviews error:", e);
      } finally {
        if (mounted) setLoading(false);
      }
    })();
    return () => {
      mounted = false;
    };
  }, [productId]);

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
        {loading ? (
          <div className="py-4 text-sm text-gray-500">Loading…</div>
        ) : reviews.length === 0 ? (
          <div className="py-4 text-sm text-gray-500">No comments yet</div>
        ) : (
          reviews.map((rv) => (
            <CommentItemView
            // key={rv.id} review={rv}
            />
          ))
        )}
      </div>
    </div>
  );
}
