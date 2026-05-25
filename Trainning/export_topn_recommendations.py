import argparse
from datetime import datetime
from pathlib import Path
from typing import Dict, Iterable, List, Optional

import numpy as np
import pandas as pd


def find_latest_prediction_file(output_dir: Path) -> Path:
    candidates = sorted(output_dir.glob("aemc_overall_predictions_*.csv"))
    if not candidates:
        raise FileNotFoundError(f"No prediction files found in {output_dir}")
    return candidates[-1]


def load_inputs(prediction_csv: Path, users_csv: Path, ratings_csv: Path):
    pred_df = pd.read_csv(prediction_csv, index_col=0)
    users_df = pd.read_csv(users_csv)
    ratings_df = pd.read_csv(ratings_csv)

    pred_df.index = pred_df.index.astype(int)
    pred_df.columns = pred_df.columns.astype(str)
    users_df["user_id"] = users_df["user_id"].astype(int)
    ratings_df["user_id"] = ratings_df["user_id"].astype(int)
    ratings_df["product_id"] = ratings_df["product_id"].astype(str)
    return pred_df, users_df, ratings_df


def build_category_map(products_csv: Optional[Path]) -> Dict[str, str]:
    if not products_csv:
        return {}
    products_df = pd.read_csv(products_csv)
    products_df["product_id"] = products_df["product_id"].astype(str)
    return dict(zip(products_df["product_id"], products_df["category"]))


def parse_groups(raw_groups: Optional[List[str]]) -> Optional[List[str]]:
    if not raw_groups:
        return None
    return [group.strip() for group in raw_groups if group.strip()]


def select_users(
    users_df: pd.DataFrame,
    groups: Optional[List[str]],
    users_per_group: int,
    strategy: str,
    seed: int,
) -> pd.DataFrame:
    selected_frames = []
    rng = np.random.default_rng(seed)

    available_groups = users_df["user_group"].dropna().astype(str).unique().tolist()
    target_groups = groups if groups else available_groups

    for group in target_groups:
        group_users = users_df.loc[users_df["user_group"] == group].copy()
        if group_users.empty:
            continue

        if strategy == "random":
            sample_size = min(users_per_group, len(group_users))
            sampled = group_users.sample(n=sample_size, random_state=int(rng.integers(0, 2**31 - 1)))
        else:
            sampled = group_users.head(users_per_group)

        selected_frames.append(sampled)

    if not selected_frames:
        raise ValueError("No users matched the requested groups.")

    return pd.concat(selected_frames, ignore_index=True)


def topn_for_user(
    user_id: int,
    group_name: str,
    pred_df: pd.DataFrame,
    ratings_df: pd.DataFrame,
    top_n: int,
    exclude_seen: bool,
    category_map: Dict[str, str],
    diversify: bool,
    max_per_category: int,
) -> pd.DataFrame:
    user_scores = pred_df.loc[user_id].copy()

    if exclude_seen:
        seen_items = ratings_df.loc[ratings_df["user_id"] == user_id, "product_id"].astype(str)
        user_scores = user_scores.drop(labels=[item for item in seen_items if item in user_scores.index], errors="ignore")

    ranked = user_scores.sort_values(ascending=False)
    if diversify and category_map and max_per_category > 0:
        selected = []
        cat_counts: Dict[str, int] = {}
        for item_id, score in ranked.items():
            category = category_map.get(str(item_id), "Unknown")
            if cat_counts.get(category, 0) >= max_per_category:
                continue
            selected.append((item_id, score))
            cat_counts[category] = cat_counts.get(category, 0) + 1
            if len(selected) >= top_n:
                break
        # fallback: fill remaining slots without diversity constraint
        if len(selected) < top_n:
            for item_id, score in ranked.items():
                if any(item_id == s[0] for s in selected):
                    continue
                selected.append((item_id, score))
                if len(selected) >= top_n:
                    break
        top_items = pd.Series({k: v for k, v in selected})
    else:
        top_items = ranked.head(top_n)
    return pd.DataFrame(
        {
            "user_id": user_id,
            "user_group": group_name,
            "rank": np.arange(1, len(top_items) + 1),
            "item_id": top_items.index.astype(str),
            "score": top_items.values.astype(float),
        }
    )


def build_export(
    pred_df: pd.DataFrame,
    users_df: pd.DataFrame,
    ratings_df: pd.DataFrame,
    groups: Optional[List[str]],
    users_per_group: int,
    top_n: int,
    strategy: str,
    seed: int,
    exclude_seen: bool,
    category_map: Dict[str, str],
    diversify: bool,
    max_per_category: int,
    all_users: bool = False,
) -> pd.DataFrame:
    if all_users:
        selected_users = users_df.copy()
    else:
        selected_users = select_users(users_df, groups, users_per_group, strategy, seed)
    chunks = []

    for row in selected_users.itertuples(index=False):
        if int(row.user_id) not in pred_df.index:
            continue
        chunks.append(
            topn_for_user(
                user_id=int(row.user_id),
                group_name=str(row.user_group),
                pred_df=pred_df,
                ratings_df=ratings_df,
                top_n=top_n,
                exclude_seen=exclude_seen,
                category_map=category_map,
                diversify=diversify,
                max_per_category=max_per_category,
            )
        )

    if not chunks:
        raise ValueError("No top-N rows could be built from the selected users.")

    export_df = pd.concat(chunks, ignore_index=True)
    export_df = export_df.sort_values(["user_group", "user_id", "rank"]).reset_index(drop=True)
    return export_df


def main() -> None:
    parser = argparse.ArgumentParser(description="Export top-N recommendations for any users/groups on demand")
    parser.add_argument("--prediction-csv", type=str, default=None, help="Overall prediction CSV path")
    parser.add_argument("--predictions-dir", type=str, default="AEMC/outputs", help="Directory to search when prediction-csv is omitted")
    parser.add_argument("--users-csv", type=str, default="AEMC/seedata_expanded/users.csv", help="users.csv path")
    parser.add_argument("--ratings-csv", type=str, default="AEMC/seedata_expanded/long_ratings.csv", help="long_ratings.csv path")
    parser.add_argument("--groups", nargs="*", default=None, help="Groups to export; default = all groups")
    parser.add_argument("--users-per-group", type=int, default=3, help="How many users to export per group")
    parser.add_argument("--all-users", action="store_true", help="Export top-N for all users (ignore users-per-group)")
    parser.add_argument("--top-n", type=int, default=10, help="Number of items per user")
    parser.add_argument("--strategy", choices=["first", "random"], default="first", help="How to choose users inside each group")
    parser.add_argument("--seed", type=int, default=42, help="Random seed used when strategy=random")
    parser.add_argument("--exclude-seen", action="store_true", help="Exclude items the user already rated")
    parser.add_argument("--include-seen", action="store_true", help="Keep already-seen items in the export")
    parser.add_argument("--products-csv", type=str, default=None, help="products.csv path (required for --diversify)")
    parser.add_argument("--diversify", action="store_true", help="Apply category diversity to top-N")
    parser.add_argument("--max-per-category", type=int, default=5, help="Max items per category when --diversify is set")
    parser.add_argument("--output-csv", type=str, default=None, help="Output CSV path")
    args = parser.parse_args()

    prediction_csv = Path(args.prediction_csv).resolve() if args.prediction_csv else find_latest_prediction_file(Path(args.predictions_dir).resolve())
    users_csv = Path(args.users_csv).resolve()
    ratings_csv = Path(args.ratings_csv).resolve()
    output_csv = Path(args.output_csv).resolve() if args.output_csv else prediction_csv.parent / f"topn_export_{datetime.now().strftime('%Y%m%d_%H%M%S')}.csv"

    groups = parse_groups(args.groups)
    exclude_seen = bool(args.exclude_seen or not args.include_seen)

    products_csv = Path(args.products_csv).resolve() if args.products_csv else None
    category_map = build_category_map(products_csv)
    if args.diversify and not category_map:
        raise ValueError("--diversify requires --products-csv to map items to categories.")

    pred_df, users_df, ratings_df = load_inputs(prediction_csv, users_csv, ratings_csv)
    export_df = build_export(
        pred_df=pred_df,
        users_df=users_df,
        ratings_df=ratings_df,
        groups=groups,
        users_per_group=args.users_per_group,
        top_n=args.top_n,
        strategy=args.strategy,
        seed=args.seed,
        exclude_seen=exclude_seen,
        category_map=category_map,
        diversify=args.diversify,
        max_per_category=args.max_per_category,
        all_users=args.all_users,
    )

    output_csv.parent.mkdir(parents=True, exist_ok=True)
    export_df.to_csv(output_csv, index=False)

    print(f"Prediction source: {prediction_csv}")
    print(f"Export saved to: {output_csv}")
    print(f"Rows exported: {len(export_df)}")
    print(f"Groups exported: {sorted(export_df['user_group'].unique().tolist())}")


if __name__ == "__main__":
    main()
