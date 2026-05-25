import argparse
import pandas as pd


def analyze(user_id: int, pred_csv: str, products_csv: str, ratings_csv: str, top_k: int = 20):
    preds = pd.read_csv(pred_csv, index_col=0)
    prods = pd.read_csv(products_csv)
    ratings = pd.read_csv(ratings_csv)

    preds.index = preds.index.astype(int)
    row = preds.loc[user_id].reset_index()
    row.columns = ["product_id", "score"]
    merged = row.merge(prods, on="product_id", how="left")

    print(f"--- Summary for user_id={user_id} ---")
    print("Average predicted score by category:")
    print(merged.groupby("category")["score"].mean().sort_values(ascending=False))
    print()

    print(f"Top {top_k} predicted items for user {user_id}:")
    print(merged.sort_values("score", ascending=False).head(top_k)[["product_id","category","tier","score"]])
    print()

    seen = ratings[ratings["user_id"] == user_id][["product_id"]]
    print(f"Seen items count: {len(seen)}")
    if not seen.empty:
        seen = seen.merge(prods, on="product_id", how="left")
        print("Seen items sample (category,tier):")
        print(seen.groupby(["category","tier"]).size())


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--user", type=int, required=True)
    parser.add_argument("--predictions", type=str, default="AEMC/outputs_test_500x100/aemc_overall_predictions_20260518_025012.csv")
    parser.add_argument("--products", type=str, default="AEMC/seedata_run_500x100/products.csv")
    parser.add_argument("--ratings", type=str, default="AEMC/seedata_run_500x100/long_ratings.csv")
    parser.add_argument("--top-k", type=int, default=20)
    args = parser.parse_args()
    analyze(args.user, args.predictions, args.products, args.ratings, args.top_k)


if __name__ == "__main__":
    main()
