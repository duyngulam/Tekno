import argparse
import os
from pathlib import Path
from typing import Dict, List

import pandas as pd


MATRIX_FILES = [
    "performance.csv",
    "value.csv",
    "portability.csv",
    "durability.csv",
]


def build_mapping(products_df: pd.DataFrame, start_id: int) -> Dict[str, int]:
    mapping: Dict[str, int] = {}
    for offset, product_id in enumerate(products_df["product_id"].astype(str).tolist()):
        mapping[product_id] = start_id + offset
    return mapping


def update_matrix(path: Path, mapping: Dict[str, int]) -> None:
    if not path.exists():
        return
    df = pd.read_csv(path)
    new_columns: List[str] = []
    for col in df.columns:
        if col == "user_id":
            new_columns.append(col)
            continue
        new_columns.append(str(mapping.get(str(col), col)))
    df.columns = new_columns
    df.to_csv(path, index=False)


def main() -> None:
    parser = argparse.ArgumentParser(description="Reconstruct seed data IDs to int IDs and write import CSV")
    parser.add_argument("--seedata-dir", default="seedata_run_2000x500_v2", help="Seed data directory")
    parser.add_argument("--output-dir", default="outputs_run_2000x500_v2", help="Output directory")
    parser.add_argument("--start-id", type=int, default=5000, help="Starting integer ID")
    parser.add_argument("--import-csv", default="import_products.csv", help="Import CSV name")
    args = parser.parse_args()

    seedata_dir = Path(args.seedata_dir).resolve()
    output_dir = Path(args.output_dir).resolve()
    output_dir.mkdir(parents=True, exist_ok=True)

    products_path = seedata_dir / "products.csv"
    if not products_path.exists():
        raise FileNotFoundError(f"products.csv not found: {products_path}")

    products_df = pd.read_csv(products_path)
    if "product_id" not in products_df.columns:
        raise ValueError("products.csv must contain product_id column")

    mapping = build_mapping(products_df, args.start_id)
    products_df["legacy_product_id"] = products_df["product_id"].astype(str)
    products_df["product_id"] = products_df["legacy_product_id"].map(mapping).astype(int)
    products_df.to_csv(products_path, index=False)

    long_ratings_path = seedata_dir / "long_ratings.csv"
    if long_ratings_path.exists():
        ratings_df = pd.read_csv(long_ratings_path)
        if "product_id" in ratings_df.columns:
            ratings_df["product_id"] = ratings_df["product_id"].astype(str).map(mapping).astype(int)
            ratings_df.to_csv(long_ratings_path, index=False)

    for file_name in MATRIX_FILES:
        update_matrix(seedata_dir / file_name, mapping)

    import_rows = []
    for _, row in products_df.iterrows():
        legacy_id = str(row.get("legacy_product_id", ""))
        category = str(row.get("category", ""))
        tier = str(row.get("tier", ""))
        name = f"{legacy_id} {category} {tier}".strip()
        import_rows.append({"id": int(row["product_id"]), "name": name})

    import_df = pd.DataFrame(import_rows)
    import_df.to_csv(output_dir / args.import_csv, index=False)

    print(f"Updated seed data in: {seedata_dir}")
    print(f"Import CSV: {output_dir / args.import_csv}")


if __name__ == "__main__":
    main()
