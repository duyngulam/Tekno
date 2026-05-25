import argparse
import os
from functools import reduce
from typing import List

import pandas as pd


def melt_matrix(path: str, value_name: str) -> pd.DataFrame:
    df = pd.read_csv(path)
    return df.melt(id_vars=["user_id"], var_name="product_id", value_name=value_name)


def merge_long_frames(frames: List[pd.DataFrame]) -> pd.DataFrame:
    return reduce(
        lambda left, right: pd.merge(left, right, on=["user_id", "product_id"], how="inner"),
        frames,
    )


def main() -> None:
    parser = argparse.ArgumentParser(description="Convert wide matrices to long-format ratings.")
    parser.add_argument(
        "--seedata-dir",
        default=os.path.join(os.path.dirname(__file__), "..", "seedata"),
        help="Path to seedata folder containing rating matrices",
    )
    parser.add_argument(
        "--output",
        default="long_ratings.csv",
        help="Output CSV file name (stored in seedata-dir)",
    )
    args = parser.parse_args()

    seedata_dir = os.path.abspath(args.seedata_dir)

    paths = {
        "performance": os.path.join(seedata_dir, "performance.csv"),
        "value": os.path.join(seedata_dir, "value.csv"),
        "portability": os.path.join(seedata_dir, "portability.csv"),
        "durability": os.path.join(seedata_dir, "durability.csv"),
    }

    frames = [melt_matrix(paths[key], key) for key in ["performance", "value", "portability", "durability"]]
    merged = merge_long_frames(frames)

    # Keep only rows where the user actually rated the product for all criteria.
    criteria = ["performance", "value", "portability", "durability"]
    nonzero_mask = (merged[criteria] > 0).all(axis=1)
    merged = merged.loc[nonzero_mask].copy()

    merged["overall_rating"] = merged[criteria].mean(axis=1).round().astype(int)

    merged = merged.sort_values(["user_id", "product_id"]).reset_index(drop=True)

    output_path = os.path.join(seedata_dir, args.output)
    merged.to_csv(output_path, index=False)

    print("Long-format dataset saved:")
    print(f"- {output_path}")
    print(f"- Rows: {len(merged)}")


if __name__ == "__main__":
    main()
