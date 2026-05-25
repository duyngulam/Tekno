from fastapi import FastAPI, HTTPException
from pathlib import Path
import pandas as pd
import os

# =========================================================
# PATH CONFIG
# =========================================================

ROOT = Path(__file__).resolve().parent

AEMC_OUTPUT_DIR = Path(
    os.environ.get(
        "AEMC_OUTPUT_DIR",
        str(ROOT / "outputs_run_2000x500_v2")
    )
)

# =========================================================
# FASTAPI
# =========================================================

app = FastAPI(
    title="CF Recommendation API"
)

# =========================================================
# CACHE
# =========================================================

_aemc_cache = {
    "path": None,
    "mtime": None,
    "recommend_map": None
}

# =========================================================
# FIND LATEST CSV
# =========================================================

def _find_latest_aemc_predictions(
    output_dir: Path
) -> Path:

    candidates = list(
        output_dir.glob(
            "topn_export_*.csv"
        )
    )

    if not candidates:
        raise FileNotFoundError(
            f"No AEMC prediction files found in {output_dir}"
        )

    latest_file = max(
        candidates,
        key=lambda p: p.stat().st_mtime
    )

    return latest_file

# =========================================================
# LOAD CSV + CACHE
# =========================================================

def _load_aemc_predictions():

    global _aemc_cache

    pred_path = _find_latest_aemc_predictions(
        AEMC_OUTPUT_DIR
    )

    mtime = pred_path.stat().st_mtime

    # =====================================
    # CACHE HIT
    # =====================================

    if (
        _aemc_cache["path"] == pred_path
        and _aemc_cache["mtime"] == mtime
    ):
        return _aemc_cache["recommend_map"]

    # =====================================
    # LOAD CSV
    # =====================================

    df = pd.read_csv(
        pred_path,
        usecols=[
            "user_id",
            "rank",
            "item_id",
            "score"
        ]
    )

    required_cols = {
        "user_id",
        "rank",
        "item_id",
        "score"
    }

    missing = required_cols - set(df.columns)

    if missing:
        raise ValueError(
            f"Missing required columns: {missing}"
        )

    # =====================================
    # TYPE CAST
    # =====================================

    df["user_id"] = (
        df["user_id"]
        .astype(int)
    )

    df["item_id"] = (
        df["item_id"]
        .astype(int)
    )

    df["rank"] = (
        df["rank"]
        .astype(int)
    )

    # =====================================
    # SORT
    # =====================================

    df = df.sort_values(
        ["user_id", "rank"],
        ascending=[True, True]
    )

    # =====================================
    # BUILD LOOKUP MAP
    # =====================================

    recommend_map = (
        df.groupby("user_id")["item_id"]
        .apply(list)
        .to_dict()
    )

    # =====================================
    # UPDATE CACHE
    # =====================================

    _aemc_cache = {
        "path": pred_path,
        "mtime": mtime,
        "recommend_map": recommend_map
    }

    return recommend_map

# =========================================================
# STARTUP
# =========================================================

@app.on_event("startup")
def startup_event():

    try:

        _load_aemc_predictions()

        print(
            "AEMC predictions loaded successfully"
        )

    except Exception as exc:

        print(
            f"Failed to preload AEMC predictions: {exc}"
        )

# =========================================================
# CF RECOMMENDATION ENDPOINT
# =========================================================

@app.get("/recommend/cf/{user_id}")
def recommend_cf_api(
    user_id: int,
    k: int = 10
):

    try:

        recommend_map = (
            _load_aemc_predictions()
        )

    except FileNotFoundError as exc:

        raise HTTPException(
            status_code=500,
            detail=str(exc)
        ) from exc

    except Exception as exc:

        raise HTTPException(
            status_code=500,
            detail=f"Failed to load CF predictions: {str(exc)}"
        ) from exc

    recommendations = (
        recommend_map.get(user_id, [])[:k]
    )

    return {
        "user_id": user_id,
        "recommendations": recommendations
    }

# =========================================================
# HEALTH CHECK
# =========================================================

@app.get("/health")
def health():

    return {
        "status": "ok"
    }