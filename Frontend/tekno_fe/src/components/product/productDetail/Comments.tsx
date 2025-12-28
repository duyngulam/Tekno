"use client";

import React, { useEffect, useState } from "react";
import { Textarea } from "@/components/ui/textarea";
import CommentItemView from "./CommentItemView";

import {
  ProductReview,
  ProductReviewsResponse,
  ReviewSummary,
  SubmitReviewPayload,
} from "@/type/review";
import {
  getProductReviews,
  canReviewProduct,
  createReview,
} from "@/services/review";
import { toast } from "sonner";
import { Star } from "lucide-react";

export default function Comments({ productId }: { productId: number }) {
  const [response, setResponse] = useState<ProductReviewsResponse | null>(null);
  const [reviews, setReviews] = useState<ProductReview[]>([]);
  const [summary, setSummary] = useState<ReviewSummary | null>(null);
  const [loading, setLoading] = useState(true);

  // permission + input
  const [canComment, setCanComment] = useState<boolean>(false);
  const [commentText, setCommentText] = useState<string>("");
  const [commentError, setCommentError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState<boolean>(false);
  const [rating, setRating] = useState<number>(5); // rating UI state
  const [hoverRating, setHoverRating] = useState<number>(0); // for hover preview

  useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        setLoading(true);
        const res = await getProductReviews({ productId });
        if (mounted) {
          setResponse(res.data);
          setReviews(res.data?.reviews ?? []);
          setSummary(res.data?.summary ?? null);
        }
      } catch (e) {
        console.error("fetch product reviews error:", e);
      } finally {
        if (mounted) setLoading(false);
      }
    })();

    // check permission to comment
    (async () => {
      try {
        const token = localStorage.getItem("token") || "";
        if (!token) {
          setCanComment(false);
          return;
        }
        const perm = await canReviewProduct(token, productId);
        setCanComment(Boolean(perm?.data?.canReview));
      } catch (e) {
        console.error("canReviewProduct error:", e);
        setCanComment(false);
      }
    })();

    return () => {
      mounted = false;
    };
  }, [productId]);

  const handleSubmitComment = async () => {
    if (!canComment || submitting) return;

    // reset lỗi cũ
    setCommentError(null);

    if (!commentText.trim()) {
      setCommentError("Vui lòng nhập nội dung bình luận");
      return;
    }

    try {
      setSubmitting(true);

      const token = localStorage.getItem("token") || "";
      if (!token) throw new Error("Missing auth token");

      const payload: SubmitReviewPayload = {
        rating,
        comment: commentText.trim(),
      };

      const resCreate = await createReview(token, productId, payload);

      toast.success(resCreate.message || "Bình luận thành công");

      // reset form
      setCommentText("");
      setRating(5);

      // reload reviews
      const res = await getProductReviews({ productId });
      setResponse(res.data);
      setReviews(res.data?.reviews ?? []);
      setSummary(res.data?.summary ?? null);
    } catch (error: any) {
      /**
       * error.errors dạng:
       * {
       *   Comment: ["Comment must be between 10 and 2000 characters"]
       * }
       */
      if (error?.errors?.Comment?.length) {
        setCommentError(error.errors.Comment[0]);
      } else {
        toast.error(error?.message || "Gửi bình luận thất bại");
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="flex flex-col md:flex-row gap-10">
      <div className=" flex flex-col w-full md:w-1/4 gap-2">
        <div className="font-bold text-xl">Comments</div>
        <p className="text-gray-500 font-normal">
          Leave your comments here for other customers
        </p>

        {/* Rating stars */}
        <div className="flex items-center gap-1 mb-2">
          {[1, 2, 3, 4, 5].map((v) => {
            const active = (hoverRating || rating) >= v;
            return (
              <button
                key={v}
                type="button"
                aria-label={`Chọn ${v} sao`}
                className="p-1"
                disabled={!canComment}
                onMouseEnter={() => setHoverRating(v)}
                onMouseLeave={() => setHoverRating(0)}
                onClick={() => setRating(v)}
              >
                <Star
                  className={`h-5 w-5 ${
                    active ? "text-yellow-400" : "text-gray-300"
                  }`}
                  fill={active ? "currentColor" : "none"}
                />
                {/* <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  className={`w-6 h-6 ${
                    active ? "text-yellow-400" : "text-gray-300"
                  }`}
                  fill="currentColor"
                >
                  <path d="M11.48 3.5a1 1 0 0 1 1.04 0l3.2 1.9 3.67.6a1 1 0 0 1 .56 1.72l-2.6 2.6.62 3.73a1 1 0 0 1-1.46 1.05L12 14.9l-3.71 2.2a1 1 0 0 1-1.46-1.05l.62-3.73-2.6-2.6a1 1 0 0 1 .56-1.72l3.67-.6 3.2-1.9Z" />
                </svg> */}
              </button>
            );
          })}
          <span className="ml-2 text-sm text-gray-600">{rating}/5</span>
        </div>

        <Textarea
          placeholder="Share your thoughts about this product here"
          value={commentText}
          onChange={(e) => {
            setCommentText(e.target.value);
            if (commentError) setCommentError(null); // clear lỗi khi user gõ lại
          }}
          disabled={!canComment}
        />

        {commentError && (
          <p className="mt-1 text-sm text-red-500">{commentError}</p>
        )}

        <button
          className="border border-primary hover:bg-gray-50 rounded-md py-2 text-primary font-normal text-2xl disabled:bg-gray-200 disabled:text-gray-500"
          onClick={handleSubmitComment}
          disabled={!canComment || submitting}
        >
          {submitting ? "Submitting..." : "Comment"}
        </button>
      </div>

      <div className="flex flex-col w-full md:w-3/4 gap-2">
        {loading ? (
          <div className="py-4 text-sm text-gray-500">Loading…</div>
        ) : reviews.length === 0 ? (
          <div className="py-4 text-sm text-gray-500">No comments yet</div>
        ) : (
          reviews.map((rv) => <CommentItemView key={rv.id} review={rv} />)
        )}
      </div>
    </div>
  );
}
