export type RatingDistribution = {
  1: number;
  2: number;
  3: number;
  4: number;
  5: number;
};

export type ReviewSummary = {
  productId: number;
  totalReviews: number;
  averageRating: number;
  ratingDistribution: RatingDistribution;
  verifiedPurchaseCount: number;
};

export type ProductReview = {
  id: number;
  rating: number;
  comment: string;
  userName: string;
  createdAt: string;
  isVerifiedPurchase: boolean;
};

export type ProductReviewsResponse = {
    reviews: ProductReview[];
    summary: ReviewSummary;
    totalCount: number;
    page: number;
    pageSize: number;
};
