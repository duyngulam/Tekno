import argparse
import os
from collections import Counter
import numpy as np
import pandas as pd


PRODUCT_PREFIXES = {
    "Ultrabook": "U",
    "Gaming": "G",
    "Workstation": "W",
    "Convertible": "C",
    "Chromebook": "B",
}


def load_existing(seedata_dir: str):
    users = pd.read_csv(os.path.join(seedata_dir, "users.csv"))
    products = pd.read_csv(os.path.join(seedata_dir, "products.csv"))
    return users, products


def expand_users(users: pd.DataFrame, target: int, rng: np.random.Generator) -> pd.DataFrame:
    if len(users) >= target:
        return users.iloc[:target].reset_index(drop=True)

    groups = users["user_group"].tolist()
    counts = Counter(groups)
    group_names = list(counts.keys())
    freqs = np.array([counts[g] for g in group_names], dtype=float)
    probs = freqs / freqs.sum()

    new_rows = []
    start = len(users)
    for uid in range(start, target):
        group = rng.choice(group_names, p=probs)
        new_rows.append({"user_id": uid, "user_group": group})

    new_df = pd.DataFrame(new_rows)
    out = pd.concat([users, new_df], ignore_index=True)
    return out


def expand_products(products: pd.DataFrame, target: int, rng: np.random.Generator, include_low_range: bool = False) -> pd.DataFrame:
    if len(products) >= target:
        return products.iloc[:target].reset_index(drop=True)

    categories = products["category"].unique().tolist()
    if include_low_range:
        tiers = ["Low-range", "Mid-range", "High-end"]
        tier_probs = [0.2, 0.6, 0.2]
    else:
        tiers = ["Mid-range", "High-end"]
        tier_probs = [0.7, 0.3]

    base_props = products[["base_performance", "base_value", "base_portability", "base_durability"]].to_numpy()
    mean_props = base_props.mean(axis=0).astype(int).tolist()

    next_suffix_by_prefix = {}
    for category, prefix in PRODUCT_PREFIXES.items():
        existing = products.loc[products["category"] == category, "product_id"].astype(str)
        suffixes = []
        for product_id in existing:
            if product_id.startswith(prefix):
                tail = product_id[len(prefix) :]
                if tail.isdigit():
                    suffixes.append(int(tail))
        next_suffix_by_prefix[prefix] = (max(suffixes) + 1) if suffixes else 1

    start = len(products)
    new_rows = []
    for i in range(start, target):
        cat = rng.choice(categories)
        tier = rng.choice(tiers, p=tier_probs)
        # Create some variety around mean props depending on category
        noise = rng.integers(-1, 2, size=4)
        if cat == "Gaming":
            props = [mean_props[0] + 2, mean_props[1], max(1, mean_props[2] - 2), mean_props[3]]
        elif cat == "Workstation":
            props = [mean_props[0] + 1, mean_props[1], max(1, mean_props[2] - 1), mean_props[3] + 1]
        elif cat == "Chromebook":
            props = [max(1, mean_props[0] - 1), mean_props[1] + 1, mean_props[2] + 1, mean_props[3]]
        elif cat == "Convertible":
            props = [mean_props[0], mean_props[1], mean_props[2] + 1, mean_props[3]]
        else:
            props = mean_props.copy()

        # If this new product is low-range, reduce base attributes further to keep low-range behavior
        if tier == "Low-range":
            props = [max(1, p - rng.integers(1, 3)) for p in props]

        props = [max(1, min(5, p + int(n))) for p, n in zip(props, noise)]

        prefix = PRODUCT_PREFIXES.get(cat, cat[:1].upper())
        pid = f"{prefix}{next_suffix_by_prefix[prefix]:03d}"
        next_suffix_by_prefix[prefix] += 1
        new_rows.append(
            {
                "product_id": pid,
                "category": cat,
                "tier": tier,
                "base_performance": props[0],
                "base_value": props[1],
                "base_portability": props[2],
                "base_durability": props[3],
            }
        )

    new_df = pd.DataFrame(new_rows)
    out = pd.concat([products, new_df], ignore_index=True).reset_index(drop=True)
    return out


def main():
    parser = argparse.ArgumentParser(description="Expand users/products for seed data testing")
    parser.add_argument("--seedata-dir", default="AEMC/seedata", help="Existing seedata dir")
    parser.add_argument("--out-dir", default="AEMC/seedata_expanded", help="Output seedata dir")
    parser.add_argument("--users", type=int, default=2000, help="Target number of users")
    parser.add_argument("--products", type=int, default=500, help="Target number of products")
    parser.add_argument("--include-low-range", action="store_true", help="Include Low-range tier products when expanding")
    parser.add_argument("--force", action="store_true", help="Allow overwriting seedata or seedata_expanded directories")
    parser.add_argument("--seed", type=int, default=42)
    args = parser.parse_args()

    rng = np.random.default_rng(args.seed)
    seedata_dir = os.path.abspath(args.seedata_dir)
    out_dir = os.path.abspath(args.out_dir)
    os.makedirs(out_dir, exist_ok=True)

    users, products = load_existing(seedata_dir)

    out_dir_abs = os.path.abspath(args.out_dir)
    seedata_dir_abs = os.path.abspath(seedata_dir)
    expanded_default = os.path.abspath(os.path.join(os.path.dirname(__file__), "seedata_expanded"))
    if (out_dir_abs == seedata_dir_abs or out_dir_abs == expanded_default) and not args.force:
        raise RuntimeError("Refusing to write into existing seedata or seedata_expanded. Use --out-dir to target a temporary folder or pass --force to override.")

    users_exp = expand_users(users, args.users, rng)
    # include low-range only if requested
    products_exp = expand_products(products, args.products, rng, include_low_range=args.include_low_range)

    users_exp.to_csv(os.path.join(out_dir, "users.csv"), index=False)
    products_exp.to_csv(os.path.join(out_dir, "products.csv"), index=False)

    print(f"Expanded seedata written to: {out_dir} (users={len(users_exp)}, products={len(products_exp)})")


if __name__ == "__main__":
    main()
