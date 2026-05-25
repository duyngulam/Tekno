import argparse
import os
from typing import Dict, List, Tuple

import numpy as np
import pandas as pd


def clamp_score(values: np.ndarray) -> np.ndarray:
    return np.clip(values, 1, 5).astype(int)


def build_allowed_products(products: pd.DataFrame) -> Dict[str, List[str]]:
    ultrabook = products.loc[products["category"] == "Ultrabook", "product_id"].tolist()
    gaming = products.loc[products["category"] == "Gaming", "product_id"].tolist()
    workstation = products.loc[products["category"] == "Workstation", "product_id"].tolist()
    convertible = products.loc[products["category"] == "Convertible", "product_id"].tolist()
    chromebook = products.loc[products["category"] == "Chromebook", "product_id"].tolist()

    ultrabook_mid = products.loc[
        (products["category"] == "Ultrabook") & (products["tier"] == "Mid-range"),
        "product_id",
    ].tolist()
    ultrabook_high = products.loc[
        (products["category"] == "Ultrabook") & (products["tier"] == "High-end"),
        "product_id",
    ].tolist()

    gaming_high = products.loc[
        (products["category"] == "Gaming") & (products["tier"] == "High-end"),
        "product_id",
    ].tolist()

    convertible_mid = products.loc[
        (products["category"] == "Convertible") & (products["tier"] == "Mid-range"),
        "product_id",
    ].tolist()

    chromebook_mid = products.loc[
        (products["category"] == "Chromebook") & (products["tier"] == "Mid-range"),
        "product_id",
    ].tolist()

    return {
        "Student": chromebook + ultrabook_mid,
        "Office Worker": ultrabook + convertible_mid,
        "Gamer": gaming,
        "Designer / Editor": workstation + gaming_high,
        "Reviewer / Tech Reviewer": products["product_id"].tolist(),
        "Casual User": chromebook_mid + convertible,
    }


def build_group_profiles() -> Dict[str, List[Dict[str, object]]]:
    return {
        "Student": [
            {
                "name": "budget_portable",
                "category_bonus": {"Chromebook": 2.0, "Ultrabook": 1.0, "Convertible": 0.5},
                "tier_bonus": {"Mid-range": 1.0, "High-end": -0.5},
                "criterion_bias": {"value": 1.5, "portability": 1.5},
                "anti_category": {"Gaming": -2.0, "Workstation": -1.0},
            },
            {
                "name": "balanced_student",
                "category_bonus": {"Ultrabook": 1.5, "Convertible": 1.0, "Chromebook": 1.0},
                "tier_bonus": {"Mid-range": 1.0, "High-end": -1.0},
                "criterion_bias": {"value": 1.0, "portability": 1.0},
                "anti_category": {"Gaming": -1.0},
            },
        ],
        "Office Worker": [
            {
                "name": "productivity_mobile",
                "category_bonus": {"Ultrabook": 2.0, "Convertible": 1.0},
                "tier_bonus": {"Mid-range": 0.8, "High-end": 0.8},
                "criterion_bias": {"portability": 1.5, "durability": 1.0},
                "anti_category": {"Gaming": -1.5},
            },
            {
                "name": "executive_balanced",
                "category_bonus": {"Ultrabook": 1.5, "Workstation": 0.5},
                "tier_bonus": {"High-end": 1.0},
                "criterion_bias": {"performance": 0.5, "durability": 1.0, "value": 0.5},
                "anti_category": {"Chromebook": -1.0},
            },
        ],
        "Gamer": [
            {
                "name": "performance_first",
                "category_bonus": {"Gaming": 2.5, "Workstation": 0.5},
                "tier_bonus": {"High-end": 1.5, "Mid-range": 0.5},
                "criterion_bias": {"performance": 2.0, "durability": 0.5},
                "anti_category": {"Chromebook": -2.0, "Convertible": -1.0},
            },
            {
                "name": "balanced_gamer",
                "category_bonus": {"Gaming": 2.0, "Workstation": 1.0},
                "tier_bonus": {"High-end": 1.0},
                "criterion_bias": {"performance": 1.5, "value": -0.5},
                "anti_category": {"Chromebook": -1.5},
            },
        ],
        "Designer / Editor": [
            {
                "name": "creator_power",
                "category_bonus": {"Workstation": 2.5, "Gaming": 1.0},
                "tier_bonus": {"High-end": 1.5},
                "criterion_bias": {"performance": 2.0, "durability": 1.5},
                "anti_category": {"Chromebook": -2.0},
            },
            {
                "name": "creator_portable",
                "category_bonus": {"Ultrabook": 1.0, "Convertible": 1.0, "Workstation": 1.0},
                "tier_bonus": {"Mid-range": 0.5, "High-end": 1.0},
                "criterion_bias": {"performance": 1.0, "portability": 0.5, "durability": 1.0},
                "anti_category": {"Chromebook": -1.0},
            },
        ],
        "Reviewer / Tech Reviewer": [
            {
                "name": "broad_coverage",
                "category_bonus": {"Ultrabook": 0.5, "Gaming": 0.5, "Workstation": 0.5, "Convertible": 0.5, "Chromebook": 0.5},
                "tier_bonus": {"High-end": 0.5, "Mid-range": 0.5},
                "criterion_bias": {"performance": 0.5, "value": 0.5, "portability": 0.5, "durability": 0.5},
                "anti_category": {},
            },
            {
                "name": "critical_benchmark",
                "category_bonus": {"Gaming": 1.0, "Workstation": 1.0, "Ultrabook": 0.5},
                "tier_bonus": {"High-end": 1.0, "Mid-range": -0.5},
                "criterion_bias": {"performance": 1.5, "durability": 1.0},
                "anti_category": {"Chromebook": -1.0},
            },
        ],
        "Casual User": [
            {
                "name": "easy_value",
                "category_bonus": {"Chromebook": 1.5, "Convertible": 1.0, "Ultrabook": 0.5},
                "tier_bonus": {"Mid-range": 1.0},
                "criterion_bias": {"value": 1.5, "portability": 0.5},
                "anti_category": {"Gaming": -1.0},
            },
            {
                "name": "balanced_casual",
                "category_bonus": {"Convertible": 1.0, "Ultrabook": 1.0, "Chromebook": 1.0},
                "tier_bonus": {"Mid-range": 0.8, "High-end": -0.2},
                "criterion_bias": {"value": 1.0, "portability": 0.5, "durability": 0.5},
                "anti_category": {"Workstation": -0.5},
            },
        ],
    }


def pick_products(
    rng: np.random.Generator,
    allowed: List[str],
    density_range: tuple[float, float],
) -> List[str]:
    if not allowed:
        return []
    density = rng.uniform(density_range[0], density_range[1])
    count = max(1, int(round(len(allowed) * density)))
    count = min(count, len(allowed))
    return rng.choice(allowed, size=count, replace=False).tolist()


def compute_scores(
    rng: np.random.Generator,
    products: pd.DataFrame,
    selected_indices: np.ndarray,
    profile: Dict[str, object],
) -> Dict[str, np.ndarray]:
    category_bonus = profile["category_bonus"]
    tier_bonus = profile["tier_bonus"]
    criterion_bias = profile["criterion_bias"]
    anti_category = profile.get("anti_category", {})
    # Keep the base attribute as the anchor and add only moderate preference signals.
    # This preserves the product profile better and avoids collapsing too many values into 5.
    scores: Dict[str, np.ndarray] = {}
    product_id_nums = products["product_id"].str.extract(r"(\d+)$").fillna(0).astype(int).to_numpy().ravel()

    # reduced phase-based item boosts to avoid pushing many values to 5
    phase_boost_map = {
        "performance": np.array([0.4, 0.0, 0.6, 0.1]),
        "value": np.array([0.45, 0.0, 0.5, 0.4]),
        "portability": np.array([0.0, 0.0, 0.6, 0.45]),
        "durability": np.array([0.0, 0.4, 0.55, 0.05]),
    }

    categories = products.loc[selected_indices, "category"].to_numpy()
    tiers = products.loc[selected_indices, "tier"].to_numpy()
    # tier multipliers to preserve low/mid/high characteristics from seed products
    tier_multiplier_map = {"High-end": 0.6, "Mid-range": 0.2, "Low-range": -0.3}

    for criterion in ["performance", "value", "portability", "durability"]:
        # Use floats for intermediate computation to allow subtle differences, then round/clamp
        product_base = products[f"base_{criterion}"].to_numpy(dtype=float)[selected_indices]

        # item-specific periodic boost
        item_phase = (product_id_nums[selected_indices] + rng.integers(0, 4, size=selected_indices.shape)) % 4
        item_boost = phase_boost_map[criterion][item_phase]

        # user/profile bias: keep this small so base values still dominate.
        prof_mean = float(criterion_bias.get(criterion, 0.0)) * 0.25
        prof_sd = max(0.18, abs(prof_mean) * 0.5)
        user_profile_offsets = rng.normal(loc=prof_mean, scale=prof_sd, size=selected_indices.shape)

        # category/tier additive boosts (float)
        cat_tier_adjust = np.zeros(selected_indices.shape, dtype=float)
        for idx, (category, tier) in enumerate(zip(categories, tiers)):
            cat_tier_adjust[idx] += float(category_bonus.get(category, 0.0))
            cat_tier_adjust[idx] += float(tier_bonus.get(tier, 0.0))
            cat_tier_adjust[idx] += float(anti_category.get(category, 0.0))

            # cross-criterion interactions (reduced magnitude)
            if criterion == "performance" and category in {"Gaming", "Workstation"}:
                cat_tier_adjust[idx] += 0.55
            if criterion == "value" and category in {"Chromebook", "Convertible"}:
                cat_tier_adjust[idx] += 0.45
            if criterion == "portability" and category in {"Ultrabook", "Chromebook", "Convertible"}:
                cat_tier_adjust[idx] += 0.5
            if criterion == "durability" and category in {"Workstation", "Ultrabook"}:
                cat_tier_adjust[idx] += 0.55

            if tier == "High-end" and criterion in {"performance", "durability"}:
                cat_tier_adjust[idx] += 0.35
            if tier == "Mid-range" and criterion == "value":
                cat_tier_adjust[idx] += 0.3

        # global noise per observation and a small discreteness factor.
        # The noise is deliberately modest to keep scores aligned with product base values.
        noise = rng.normal(loc=0.0, scale=0.35, size=selected_indices.shape)
        discreteness = rng.choice([0.0, 0.1, -0.1], size=selected_indices.shape, p=[0.7, 0.15, 0.15])

        # apply tier multiplier so low/mid/high characteristics remain visible
        tier_mult = np.array([tier_multiplier_map.get(t, 0.0) for t in tiers], dtype=float)
        cat_tier_adjust = cat_tier_adjust * (1.0 + tier_mult)

        # Anchor on the original base score, then add a controlled adjustment around it.
        raw = (
            product_base
            + cat_tier_adjust * 0.22
            + item_boost * 0.08
            + user_profile_offsets * 0.22
            + noise
            + discreteness
        )

        # Pull high-base items slightly back so the output is spread across 1..5 rather than saturating at 5.
        raw = np.where(product_base >= 4.0, raw - 0.18, raw)
        raw = np.where(product_base <= 2.0, raw + 0.08, raw)

        # Final rounding and clamp to integers 1..5
        scores[criterion] = clamp_score(np.rint(raw).astype(int))

    return scores


def score_for_user(
    rng: np.random.Generator,
    base: np.ndarray,
    bias: float,
) -> np.ndarray:
    # Use small gaussian noise and round; keeps base as float-compatible input
    noise = rng.normal(loc=0.0, scale=0.4, size=base.shape)
    raw = base.astype(float) + float(bias) + noise
    return clamp_score(np.rint(raw).astype(int))


def main() -> None:
    parser = argparse.ArgumentParser(description="Generate multi-criteria rating matrices.")
    parser.add_argument(
        "--seedata-dir",
        default=os.path.join(os.path.dirname(__file__), "..", "seedata"),
        help="Path to seedata folder containing users.csv and products.csv",
    )
    parser.add_argument("--seed", type=int, default=42, help="Random seed")
    args = parser.parse_args()

    seedata_dir = os.path.abspath(args.seedata_dir)
    users_path = os.path.join(seedata_dir, "users.csv")
    products_path = os.path.join(seedata_dir, "products.csv")

    users = pd.read_csv(users_path)
    products = pd.read_csv(products_path)

    allowed_map = build_allowed_products(products)
    profiles_map = build_group_profiles()

    # Density ranges by user group.
    density_map: Dict[str, tuple[float, float]] = {
        "Student": (0.12, 0.22),
        "Office Worker": (0.12, 0.22),
        "Gamer": (0.30, 0.55),
        "Designer / Editor": (0.22, 0.42),
        "Reviewer / Tech Reviewer": (0.20, 0.35),
        "Casual User": (0.15, 0.28),
    }

    product_ids = products["product_id"].tolist()
    index_by_id = {pid: idx for idx, pid in enumerate(product_ids)}

    base_perf = products["base_performance"].to_numpy(dtype=int)
    base_val = products["base_value"].to_numpy(dtype=int)
    base_port = products["base_portability"].to_numpy(dtype=int)
    base_dur = products["base_durability"].to_numpy(dtype=int)

    rng = np.random.default_rng(args.seed)

    matrices = {
        "performance": [],
        "value": [],
        "portability": [],
        "durability": [],
    }

    for _, row in users.iterrows():
        user_id = row["user_id"]
        group = row["user_group"]

        group_profiles = profiles_map.get(group, profiles_map["Casual User"])
        profile = group_profiles[int(rng.integers(0, len(group_profiles)))]

        allowed = allowed_map.get(group, [])
        density_range = density_map.get(group, (0.10, 0.15))
        chosen = pick_products(rng, allowed, density_range)

        # Initialize all-zero row and fill only chosen products.
        row_perf = np.zeros(len(product_ids), dtype=int)
        row_val = np.zeros(len(product_ids), dtype=int)
        row_port = np.zeros(len(product_ids), dtype=int)
        row_dur = np.zeros(len(product_ids), dtype=int)

        if chosen:
            idx = np.array([index_by_id[pid] for pid in chosen], dtype=int)

            selected_products = products.loc[idx].reset_index(drop=True)
            criterion_scores = compute_scores(
                rng=rng,
                products=selected_products,
                selected_indices=np.arange(len(idx)),
                profile=profile,
            )

            row_perf[idx] = criterion_scores["performance"]
            row_val[idx] = criterion_scores["value"]
            row_port[idx] = criterion_scores["portability"]
            row_dur[idx] = criterion_scores["durability"]

        matrices["performance"].append([user_id, *row_perf.tolist()])
        matrices["value"].append([user_id, *row_val.tolist()])
        matrices["portability"].append([user_id, *row_port.tolist()])
        matrices["durability"].append([user_id, *row_dur.tolist()])

    columns = ["user_id", *product_ids]
    output_files = {
        "performance": "performance.csv",
        "value": "value.csv",
        "portability": "portability.csv",
        "durability": "durability.csv",
    }

    for key, rows in matrices.items():
        df = pd.DataFrame(rows, columns=columns)
        df.to_csv(os.path.join(seedata_dir, output_files[key]), index=False)

    print("Seed data generated:")
    for name in output_files.values():
        print(f"- {name}")


if __name__ == "__main__":
    main()
