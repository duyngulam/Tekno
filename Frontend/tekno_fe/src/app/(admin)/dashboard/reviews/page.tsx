"use client";

import React, { useEffect, useState } from "react";
import {
  approveReview,
  rejectReview,
  getAdminProductReviews,
} from "@/services/review";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  CheckCircle,
  XCircle,
  Star,
  Eye,
  Search,
  Filter,
  ShieldCheck,
} from "lucide-react";

interface AdminReview {
  id: number;
  productId: number;
  productName: string;
  userId: number;
  userName: string;
  userEmail: string;
  rating: number;
  comment: string;
  status: "Pending" | "Approved" | "Rejected";
  isVerifiedPurchase: boolean;
  createdAt: string;
  updatedAt?: string;
}

interface ReviewSummary {
  productId: number;
  totalReviews: number;
  averageRating: number;
  ratingDistribution: {
    "1": number;
    "2": number;
    "3": number;
    "4": number;
    "5": number;
  };
  verifiedPurchaseCount: number;
}

interface AdminReviewsResponse {
  reviews: AdminReview[];
  summary: ReviewSummary;
  totalCount: number;
  page: number;
  pageSize: number;
}

export default function AdminReviewsPage() {
  const [reviews, setReviews] = useState<AdminReview[]>([]);
  const [summary, setSummary] = useState<ReviewSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);

  // Product ID selector
  const [productId, setProductId] = useState<number>(1);

  // Filters
  const [statusFilter, setStatusFilter] = useState<"All" | "Pending" | "Approved" | "Rejected">("All");
  const [verifiedFilter, setVerifiedFilter] = useState<"All" | "Verified" | "Unverified">("All");
  const [searchQuery, setSearchQuery] = useState("");
  const [ratingFilter, setRatingFilter] = useState<number | "All">("All");

  // Detail Modal
  const [openDetail, setOpenDetail] = useState(false);
  const [selectedReview, setSelectedReview] = useState<AdminReview | null>(null);

  const getToken = () => {
    if (typeof window !== "undefined") {
      return (
        localStorage.getItem("token") ||
        localStorage.getItem("accessToken") ||
        sessionStorage.getItem("token") ||
        ""
      );
    }
    return "";
  };

  useEffect(() => {
    loadReviews();
  }, [page, productId]);

  const loadReviews = async () => {
    try {
      setLoading(true);
      const token = getToken();

      if (!token) {
        alert("Please login to view reviews");
        setLoading(false);
        return;
      }

      const response = await getAdminProductReviews(token, productId, page, pageSize);
      
      const reviewsData = response?.data?.reviews || [];
      const summaryData = response?.data?.summary || null;
      const total = response?.data?.totalCount || 0;
      
      setReviews(reviewsData);
      setSummary(summaryData);
      setTotalCount(total);
    } catch (error: any) {
      console.error("Failed to load reviews:", error);
      const errorMessage = error?.message || "Failed to load reviews";
      alert(errorMessage);
      setReviews([]);
      setSummary(null);
      setTotalCount(0);
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async (reviewId: number) => {
    if (!confirm("Are you sure you want to approve this review?")) return;

    try {
      const token = getToken();
      
      if (!token) {
        alert("Please login to perform this action");
        return;
      }

      await approveReview(token, reviewId);
      alert("Review approved successfully!");
      await loadReviews();
    } catch (error: any) {
      console.error("Failed to approve review:", error);
      const errorMessage = error?.message || "Failed to approve review";
      alert(errorMessage);
    }
  };

  const handleReject = async (reviewId: number) => {
    if (!confirm("Are you sure you want to reject this review?")) return;

    try {
      const token = getToken();
      
      if (!token) {
        alert("Please login to perform this action");
        return;
      }

      await rejectReview(token, reviewId);
      alert("Review rejected successfully!");
      await loadReviews();
    } catch (error: any) {
      console.error("Failed to reject review:", error);
      const errorMessage = error?.message || "Failed to reject review";
      alert(errorMessage);
    }
  };

  const handleViewDetail = (review: AdminReview) => {
    setSelectedReview(review);
    setOpenDetail(true);
  };

  // Filtered reviews
  const filteredReviews = reviews.filter((review) => {
    // Status filter
    if (statusFilter !== "All" && review.status !== statusFilter) return false;

    // Verified filter
    if (verifiedFilter === "Verified" && !review.isVerifiedPurchase) return false;
    if (verifiedFilter === "Unverified" && review.isVerifiedPurchase) return false;

    // Rating filter
    if (ratingFilter !== "All" && review.rating !== ratingFilter) return false;

    // Search filter
    if (searchQuery) {
      const query = searchQuery.toLowerCase();
      return (
        review.productName.toLowerCase().includes(query) ||
        review.userName.toLowerCase().includes(query) ||
        review.userEmail.toLowerCase().includes(query) ||
        review.comment.toLowerCase().includes(query)
      );
    }

    return true;
  });

  const renderStars = (rating: number) => {
    return (
      <div className="flex gap-1">
        {[1, 2, 3, 4, 5].map((star) => (
          <Star
            key={star}
            size={16}
            className={star <= rating ? "fill-yellow-400 text-yellow-400" : "text-gray-300"}
          />
        ))}
      </div>
    );
  };

  const getStatusBadge = (status: string) => {
    const styles = {
      Pending: "bg-yellow-100 text-yellow-700",
      Approved: "bg-green-100 text-green-700",
      Rejected: "bg-red-100 text-red-700",
    };
    return (
      <span className={`px-2 py-1 rounded text-xs font-medium ${styles[status as keyof typeof styles]}`}>
        {status}
      </span>
    );
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Loading reviews...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 bg-gray-50 min-h-screen">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Reviews Management</h1>
          <p className="text-sm text-gray-600 mt-1">
            Manage and moderate customer reviews
          </p>
        </div>
        <div className="text-sm text-gray-600">
          Total Reviews: <span className="font-bold">{totalCount}</span>
        </div>
      </div>

      {/* Product Selector & Summary */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 mb-6">
        {/* Product ID Input */}
        <div className="bg-white rounded-lg shadow-sm p-4">
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Product ID
          </label>
          <div className="flex gap-2">
            <input
              type="number"
              min="1"
              className="flex-1 border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
              value={productId}
              onChange={(e) => setProductId(Number(e.target.value))}
            />
            <Button onClick={loadReviews}>Load</Button>
          </div>
        </div>

        {/* Summary Cards */}
        {summary && (
          <>
            <div className="bg-white rounded-lg shadow-sm p-4">
              <div className="flex items-center justify-between mb-2">
                <p className="text-sm font-medium text-gray-600">Average Rating</p>
                <Star className="w-5 h-5 text-yellow-400 fill-yellow-400" />
              </div>
              <p className="text-2xl font-bold text-gray-900">
                {summary.averageRating.toFixed(1)}/5
              </p>
              <p className="text-xs text-gray-500 mt-1">
                {summary.totalReviews} total reviews
              </p>
            </div>

            <div className="bg-white rounded-lg shadow-sm p-4">
              <div className="flex items-center justify-between mb-2">
                <p className="text-sm font-medium text-gray-600">Verified Purchases</p>
                <ShieldCheck className="w-5 h-5 text-green-600" />
              </div>
              <p className="text-2xl font-bold text-gray-900">
                {summary.verifiedPurchaseCount}
              </p>
              <p className="text-xs text-gray-500 mt-1">
                {summary.totalReviews > 0 
                  ? `${((summary.verifiedPurchaseCount / summary.totalReviews) * 100).toFixed(1)}%` 
                  : '0%'} of total
              </p>
            </div>
          </>
        )}
      </div>

      {/* Rating Distribution */}
      {summary && summary.totalReviews > 0 && (
        <div className="bg-white rounded-lg shadow-sm p-6 mb-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Rating Distribution</h3>
          <div className="space-y-2">
            {[5, 4, 3, 2, 1].map((rating) => {
              const count = summary.ratingDistribution[rating.toString() as keyof typeof summary.ratingDistribution];
              const percentage = summary.totalReviews > 0 ? (count / summary.totalReviews) * 100 : 0;
              return (
                <div key={rating} className="flex items-center gap-3">
                  <div className="flex items-center gap-1 w-16">
                    <span className="text-sm font-medium">{rating}</span>
                    <Star size={14} className="fill-yellow-400 text-yellow-400" />
                  </div>
                  <div className="flex-1 bg-gray-200 rounded-full h-2">
                    <div
                      className="bg-yellow-400 h-2 rounded-full transition-all"
                      style={{ width: `${percentage}%` }}
                    />
                  </div>
                  <span className="text-sm text-gray-600 w-16 text-right">
                    {count} ({percentage.toFixed(0)}%)
                  </span>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Filters */}
      <div className="bg-white rounded-lg shadow-sm p-4 mb-6">
        <div className="flex flex-wrap items-center gap-4">
          {/* Search */}
          <div className="flex-1 min-w-[250px]">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
              <input
                type="text"
                placeholder="Search by product, user, or comment..."
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
            </div>
          </div>

          {/* Status Filter */}
          <select
            className="border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value as any)}
          >
            <option value="All">All Status</option>
            <option value="Pending">Pending</option>
            <option value="Approved">Approved</option>
            <option value="Rejected">Rejected</option>
          </select>

          {/* Verified Filter */}
          <select
            className="border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={verifiedFilter}
            onChange={(e) => setVerifiedFilter(e.target.value as any)}
          >
            <option value="All">All Purchases</option>
            <option value="Verified">Verified Only</option>
            <option value="Unverified">Unverified Only</option>
          </select>

          {/* Rating Filter */}
          <select
            className="border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={ratingFilter}
            onChange={(e) => setRatingFilter(e.target.value === "All" ? "All" : Number(e.target.value))}
          >
            <option value="All">All Ratings</option>
            <option value="5">5 Stars</option>
            <option value="4">4 Stars</option>
            <option value="3">3 Stars</option>
            <option value="2">2 Stars</option>
            <option value="1">1 Star</option>
          </select>

          {/* Reset Filters */}
          <Button
            variant="outline"
            onClick={() => {
              setStatusFilter("All");
              setVerifiedFilter("All");
              setRatingFilter("All");
              setSearchQuery("");
            }}
            className="flex items-center gap-2"
          >
            <Filter className="w-4 h-4" />
            Reset
          </Button>
        </div>

        {/* Active Filters Summary */}
        <div className="mt-3 text-sm text-gray-600">
          Showing <span className="font-bold">{filteredReviews.length}</span> of{" "}
          <span className="font-bold">{totalCount}</span> reviews
        </div>
      </div>

      {/* Reviews Table */}
      <div className="bg-white rounded-lg shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="bg-gray-100 border-b">
              <tr>
                <th className="text-left py-3 px-4 font-medium text-gray-600">Product</th>
                <th className="text-left py-3 px-4 font-medium text-gray-600">Customer</th>
                <th className="text-left py-3 px-4 font-medium text-gray-600">Rating</th>
                <th className="text-left py-3 px-4 font-medium text-gray-600">Comment</th>
                <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                <th className="text-left py-3 px-4 font-medium text-gray-600">Date</th>
                <th className="text-center py-3 px-4 font-medium text-gray-600">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredReviews.length === 0 ? (
                <tr>
                  <td colSpan={7} className="text-center py-12 text-gray-500">
                    No reviews found
                  </td>
                </tr>
              ) : (
                filteredReviews.map((review) => (
                  <tr key={review.id} className="border-b hover:bg-gray-50">
                    <td className="py-3 px-4">
                      <div>
                        <p className="font-medium text-gray-900">{review.productName}</p>
                        <p className="text-xs text-gray-500">ID: {review.productId}</p>
                      </div>
                    </td>
                    <td className="py-3 px-4">
                      <div>
                        <p className="font-medium text-gray-900 flex items-center gap-1">
                          {review.userName}
                          {review.isVerifiedPurchase && (
                            <ShieldCheck className="w-4 h-4 text-green-600" aria-label="Verified Purchase" />
                          )}
                        </p>
                        <p className="text-xs text-gray-500">{review.userEmail}</p>
                      </div>
                    </td>
                    <td className="py-3 px-4">
                      <div className="flex items-center gap-2">
                        {renderStars(review.rating)}
                        <span className="text-xs font-medium text-gray-600">
                          {review.rating}/5
                        </span>
                      </div>
                    </td>
                    <td className="py-3 px-4 max-w-xs">
                      <p className="text-gray-700 line-clamp-2">{review.comment}</p>
                    </td>
                    <td className="py-3 px-4">{getStatusBadge(review.status)}</td>
                    <td className="py-3 px-4 text-xs text-gray-500">
                      {new Date(review.createdAt).toLocaleDateString("vi-VN")}
                    </td>
                    <td className="py-3 px-4">
                      <div className="flex items-center justify-center gap-2">
                        <button
                          onClick={() => handleViewDetail(review)}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded"
                          aria-label="View Details"
                        >
                          <Eye size={16} />
                        </button>

                        {review.status !== "Approved" && (
                          <button
                            onClick={() => handleApprove(review.id)}
                            className="p-2 text-green-600 hover:bg-green-50 rounded"
                            aria-label="Approve"
                          >
                            <CheckCircle size={16} />
                          </button>
                        )}

                        {review.status !== "Rejected" && (
                          <button
                            onClick={() => handleReject(review.id)}
                            className="p-2 text-red-600 hover:bg-red-50 rounded"
                            aria-label="Reject"
                          >
                            <XCircle size={16} />
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalCount > pageSize && (
          <div className="flex items-center justify-between px-4 py-3 border-t">
            <div className="text-sm text-gray-600">
              Page {page} of {Math.ceil(totalCount / pageSize)}
            </div>
            <div className="flex gap-2">
              <Button
                variant="outline"
                size="sm"
                disabled={page === 1}
                onClick={() => setPage(page - 1)}
              >
                Previous
              </Button>
              <Button
                variant="outline"
                size="sm"
                disabled={page >= Math.ceil(totalCount / pageSize)}
                onClick={() => setPage(page + 1)}
              >
                Next
              </Button>
            </div>
          </div>
        )}
      </div>

      {/* Detail Modal */}
      {openDetail && selectedReview && (
        <Dialog open={openDetail} onOpenChange={setOpenDetail}>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Review Details</DialogTitle>
            </DialogHeader>

            <div className="space-y-4 mt-4">
              {/* Product Info */}
              <div className="bg-gray-50 rounded-lg p-4">
                <p className="text-sm font-medium text-gray-600 mb-1">Product</p>
                <p className="text-lg font-semibold text-gray-900">
                  {selectedReview.productName}
                </p>
                <p className="text-xs text-gray-500">Product ID: {selectedReview.productId}</p>
              </div>

              {/* Customer Info */}
              <div className="bg-gray-50 rounded-lg p-4">
                <p className="text-sm font-medium text-gray-600 mb-2">Customer</p>
                <div className="flex items-center gap-2 mb-1">
                  <p className="font-medium text-gray-900">{selectedReview.userName}</p>
                  {selectedReview.isVerifiedPurchase && (
                    <span className="flex items-center gap-1 text-xs bg-green-100 text-green-700 px-2 py-1 rounded">
                      <ShieldCheck className="w-3 h-3" />
                      Verified Purchase
                    </span>
                  )}
                </div>
                <p className="text-sm text-gray-600">{selectedReview.userEmail}</p>
                <p className="text-xs text-gray-500 mt-1">User ID: {selectedReview.userId}</p>
              </div>

              {/* Rating & Comment */}
              <div className="bg-gray-50 rounded-lg p-4">
                <div className="flex items-center justify-between mb-3">
                  <p className="text-sm font-medium text-gray-600">Rating & Review</p>
                  {getStatusBadge(selectedReview.status)}
                </div>
                <div className="flex items-center gap-2 mb-3">
                  {renderStars(selectedReview.rating)}
                  <span className="text-lg font-bold text-gray-900">
                    {selectedReview.rating}/5
                  </span>
                </div>
                <p className="text-gray-700 whitespace-pre-wrap">{selectedReview.comment}</p>
              </div>

              {/* Timestamps */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm font-medium text-gray-600 mb-1">Created At</p>
                  <p className="text-sm text-gray-900">
                    {new Date(selectedReview.createdAt).toLocaleString("vi-VN")}
                  </p>
                </div>
                {selectedReview.updatedAt && (
                  <div>
                    <p className="text-sm font-medium text-gray-600 mb-1">Updated At</p>
                    <p className="text-sm text-gray-900">
                      {new Date(selectedReview.updatedAt).toLocaleString("vi-VN")}
                    </p>
                  </div>
                )}
              </div>

              {/* Actions */}
              <div className="flex justify-end gap-3 pt-4 border-t">
                <Button variant="outline" onClick={() => setOpenDetail(false)}>
                  Close
                </Button>
                {selectedReview.status !== "Approved" && (
                  <Button
                    onClick={() => {
                      handleApprove(selectedReview.id);
                      setOpenDetail(false);
                    }}
                    className="bg-green-600 hover:bg-green-700"
                  >
                    <CheckCircle className="w-4 h-4 mr-2" />
                    Approve
                  </Button>
                )}
                {selectedReview.status !== "Rejected" && (
                  <Button
                    variant="outline"
                    onClick={() => {
                      handleReject(selectedReview.id);
                      setOpenDetail(false);
                    }}
                    className="text-red-600 border-red-600 hover:bg-red-50"
                  >
                    <XCircle className="w-4 h-4 mr-2" />
                    Reject
                  </Button>
                )}
              </div>
            </div>
          </DialogContent>
        </Dialog>
      )}
    </div>
  );
}