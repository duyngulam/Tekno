import argparse
from dataclasses import dataclass
from datetime import datetime
from pathlib import Path
from typing import Dict, List, Tuple

import numpy as np
import pandas as pd

try:
    import tensorflow as tf
except ModuleNotFoundError as exc:
    raise ModuleNotFoundError(
        "TensorFlow is required. Install it in Kaggle with: pip install tensorflow"
    ) from exc


ID_COLUMNS = {"user_id", "item_id", "product_id", "overall_rating"}


@dataclass
class SplitData:
    train_matrix: np.ndarray
    test_matrix: np.ndarray
    train_mask: np.ndarray
    test_mask: np.ndarray


class KerasAutoencoder:
    def __init__(
        self,
        input_dim: int,
        hidden_depth: int,
        learning_rate: float = 0.01,
        weight_decay: float = 0.1,
        seed: int = 42,
    ) -> None:
        tf.keras.utils.set_random_seed(seed)

        hidden_layers = build_hidden_layers(input_dim=input_dim, hidden_depth=hidden_depth)
        self.model = self._build_model(
            input_dim=input_dim,
            hidden_layers=hidden_layers,
            weight_decay=weight_decay,
            seed=seed,
        )

        optimizer = tf.keras.optimizers.RMSprop(learning_rate=learning_rate)
        self.model.compile(optimizer=optimizer, loss=self._masked_mae)

    @staticmethod
    def _masked_mae(y_true: tf.Tensor, y_pred: tf.Tensor) -> tf.Tensor:
        mask = tf.cast(y_true > 0.0, tf.float32)
        abs_error = tf.abs(y_true - y_pred) * mask
        denom = tf.reduce_sum(mask, axis=-1) + tf.keras.backend.epsilon()
        return tf.reduce_sum(abs_error, axis=-1) / denom

    @staticmethod
    def _build_model(
        input_dim: int,
        hidden_layers: List[int],
        weight_decay: float,
        seed: int,
    ) -> tf.keras.Model:
        kernel_initializer = tf.keras.initializers.RandomNormal(
            mean=0.0,
            stddev=float(np.sqrt(0.02)),
            seed=seed,
        )
        bias_initializer = tf.keras.initializers.Zeros()
        regularizer = tf.keras.regularizers.L2(weight_decay) if weight_decay > 0 else None

        inputs = tf.keras.layers.Input(shape=(input_dim,), name="ratings_input")
        x = inputs

        for idx, units in enumerate(hidden_layers):
            x = tf.keras.layers.Dense(
                units,
                activation="relu",
                kernel_initializer=kernel_initializer,
                bias_initializer=bias_initializer,
                kernel_regularizer=regularizer,
                name=f"hidden_{idx + 1}",
            )(x)

        outputs = tf.keras.layers.Dense(
            input_dim,
            activation="sigmoid",
            kernel_initializer=kernel_initializer,
            bias_initializer=bias_initializer,
            kernel_regularizer=regularizer,
            name="output",
        )(x)

        return tf.keras.Model(inputs=inputs, outputs=outputs, name="aemc_autoencoder")

    def fit(self, train_matrix: np.ndarray, epochs: int = 200, batch_size: int = 100) -> None:
        history = self.model.fit(
            train_matrix,
            train_matrix,
            epochs=epochs,
            batch_size=batch_size,
            shuffle=True,
            verbose=0,
        )

        for epoch_idx in range(epochs):
            if epoch_idx == 0 or (epoch_idx + 1) % 20 == 0:
                print(f"Epoch {epoch_idx + 1:03d}/{epochs} - Train MAE: {history.history['loss'][epoch_idx]:.4f}")

    def reconstruct(self, matrix: np.ndarray, batch_size: int = 100) -> np.ndarray:
        return self.model.predict(matrix, batch_size=batch_size, verbose=0)


def normalize_ratings(matrix: np.ndarray) -> np.ndarray:
    normalized = matrix.astype(np.float32).copy()
    mask = normalized > 0
    normalized[mask] = normalized[mask] / 5.0
    return normalized


def denormalize_ratings(matrix: np.ndarray) -> np.ndarray:
    return np.clip(matrix * 5.0, 1.0, 5.0)


def build_hidden_layers(input_dim: int, hidden_depth: int) -> List[int]:
    if hidden_depth not in {3, 5, 7}:
        raise ValueError("hidden_depth must be one of: 3, 5, 7")

    encoder_len = (hidden_depth + 1) // 2
    ratios = np.linspace(0.8, 0.35, encoder_len)

    encoder_units: List[int] = []
    prev_units = input_dim
    for ratio in ratios:
        units = max(4, int(round(input_dim * float(ratio))))
        units = min(units, max(4, prev_units - 1))
        encoder_units.append(units)
        prev_units = units

    return encoder_units + encoder_units[-2::-1]


def evaluate(pred: np.ndarray, truth: np.ndarray, mask: np.ndarray) -> Dict[str, float]:
    diff = (pred - truth) * mask
    count = np.sum(mask)
    if count == 0:
        return {"mae": 0.0, "rmse": 0.0}

    return {
        "mae": float(np.sum(np.abs(diff)) / count),
        "rmse": float(np.sqrt(np.sum(diff**2) / count)),
    }


def infer_criteria_columns(df: pd.DataFrame) -> List[str]:
    criteria: List[str] = []
    for column in df.columns:
        if column in ID_COLUMNS:
            continue

        numeric_values = pd.to_numeric(df[column], errors="coerce")
        if numeric_values.notna().any():
            criteria.append(column)

    if not criteria:
        raise ValueError("CSV must contain at least one numeric criterion column besides IDs.")

    return criteria


def load_long_format(csv_path: Path) -> Tuple[pd.DataFrame, List[str]]:
    df = pd.read_csv(csv_path)

    if "item_id" not in df.columns and "product_id" in df.columns:
        df = df.rename(columns={"product_id": "item_id"})

    required = {"user_id", "item_id"}
    if not required.issubset(df.columns):
        raise ValueError("CSV must contain user_id and item_id/product_id columns.")

    criteria = infer_criteria_columns(df)

    if "overall_rating" not in df.columns:
        df["overall_rating"] = df[criteria].mean(axis=1).round().astype(int)

    df = df.sort_values(["user_id", "item_id"]).reset_index(drop=True)
    return df, criteria


def split_train_test(df: pd.DataFrame, test_ratio: float, seed: int) -> Tuple[pd.DataFrame, pd.DataFrame]:
    rng = np.random.default_rng(seed)
    train_parts = []
    test_parts = []

    for _, group in df.groupby("user_id", sort=False):
        group_indices = group.index.to_numpy().copy()
        rng.shuffle(group_indices)

        if len(group_indices) == 1:
            train_parts.append(df.loc[group_indices])
            continue

        n_test = max(1, int(round(len(group_indices) * test_ratio)))
        n_test = min(n_test, len(group_indices) - 1)

        test_idx = group_indices[:n_test]
        train_idx = group_indices[n_test:]

        train_parts.append(df.loc[train_idx])
        test_parts.append(df.loc[test_idx])

    train_df = pd.concat(train_parts, axis=0).sample(frac=1.0, random_state=seed).reset_index(drop=True)
    test_df = pd.concat(test_parts, axis=0).sample(frac=1.0, random_state=seed).reset_index(drop=True)
    return train_df, test_df


def build_user_item_matrix(df: pd.DataFrame, users: np.ndarray, items: np.ndarray, criterion: str) -> np.ndarray:
    user_index = {u: i for i, u in enumerate(users)}
    item_index = {it: j for j, it in enumerate(items)}
    matrix = np.zeros((len(users), len(items)), dtype=np.float32)

    for row in df[["user_id", "item_id", criterion]].itertuples(index=False):
        matrix[user_index[int(row.user_id)], item_index[row.item_id]] = float(getattr(row, criterion))

    return matrix


def save_matrix(matrix: np.ndarray, users: np.ndarray, items: np.ndarray, path: Path) -> None:
    df = pd.DataFrame(matrix, index=users, columns=items)
    df.index.name = "user_id"
    df.to_csv(path)


def train_and_evaluate(
    train_df: pd.DataFrame,
    test_df: pd.DataFrame,
    criteria: List[str],
    hidden_depth: int,
    epochs: int,
    learning_rate: float,
    weight_decay: float,
    batch_size: int,
    seed: int,
    output_dir: Path,
) -> Dict[str, Dict[str, float]]:
    users = np.sort(np.unique(np.concatenate([train_df["user_id"].values, test_df["user_id"].values])))
    items = np.sort(np.unique(np.concatenate([train_df["item_id"].values, test_df["item_id"].values])))

    print(f"Users: {len(users)}, Items: {len(items)}, Criteria: {len(criteria)}")

    predictions = []
    truths = []
    masks = []
    metrics: Dict[str, Dict[str, float]] = {}

    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")

    for idx, criterion in enumerate(criteria):
        print(f"\n=== Training criterion: {criterion} ===")
        train_matrix = build_user_item_matrix(train_df, users, items, criterion)
        test_matrix = build_user_item_matrix(test_df, users, items, criterion)
        test_mask = (test_matrix > 0).astype(np.float32)

        model = KerasAutoencoder(
            input_dim=len(items),
            hidden_depth=hidden_depth,
            learning_rate=learning_rate,
            weight_decay=weight_decay,
            seed=seed + idx,
        )

        train_norm = normalize_ratings(train_matrix)
        model.fit(train_norm, epochs=epochs, batch_size=batch_size)

        pred = denormalize_ratings(model.reconstruct(train_norm, batch_size=batch_size))
        metric = evaluate(pred, test_matrix, test_mask)
        metrics[criterion] = metric

        print(f"Criterion '{criterion}' - Test MAE: {metric['mae']:.4f}, RMSE: {metric['rmse']:.4f}")

        # save per-criterion prediction for debugging
        save_matrix(pred, users, items, output_dir / f"aemc_pred_{criterion}_{timestamp}.csv")
        print(f"Saved per-criterion predictions: aemc_pred_{criterion}_{timestamp}.csv")
        predictions.append(pred)
        truths.append(test_matrix)
        masks.append(test_mask)

    overall_pred = np.mean(np.stack(predictions, axis=0), axis=0)
    overall_truth = np.sum(np.stack(truths, axis=0), axis=0) / np.maximum(np.sum(np.stack(masks, axis=0), axis=0), 1.0)
    overall_mask = (np.sum(np.stack(masks, axis=0), axis=0) > 0).astype(np.float32)

    metrics["overall"] = evaluate(overall_pred, overall_truth, overall_mask)
    print("\n=== Overall prediction ===")
    print(f"Overall - Test MAE: {metrics['overall']['mae']:.4f}, RMSE: {metrics['overall']['rmse']:.4f}")

    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    save_matrix(overall_pred, users, items, output_dir / f"aemc_overall_predictions_{timestamp}.csv")
    return metrics


def save_metrics(metrics: Dict[str, Dict[str, float]], output_dir: Path) -> None:
    rows = [{"criterion": name, "mae": values["mae"], "rmse": values["rmse"]} for name, values in metrics.items()]
    pd.DataFrame(rows).to_csv(output_dir / "aemc_metrics.csv", index=False)


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Generic AEMC runner for long-format rating datasets")
    parser.add_argument("--input", type=str, default="seedata/long_ratings.csv", help="Path to a long-format CSV")
    parser.add_argument("--output-dir", type=str, default="AEMC/outputs", help="Directory for outputs")
    parser.add_argument("--test-ratio", type=float, default=0.2)
    parser.add_argument("--seed", type=int, default=42)
    parser.add_argument("--hidden-depth", type=int, choices=[3, 5, 7], default=3)
    parser.add_argument("--epochs", type=int, default=200)
    parser.add_argument("--learning-rate", type=float, default=0.001)
    parser.add_argument("--weight-decay", type=float, default=0.0001)
    parser.add_argument("--batch-size", type=int, default=100)
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    input_csv = Path(args.input).resolve()
    output_dir = Path(args.output_dir).resolve()
    output_dir.mkdir(parents=True, exist_ok=True)

    df, criteria = load_long_format(input_csv)
    print(f"Loaded CSV: {input_csv}")
    print(f"Detected criteria: {criteria}")
    print(f"Rows: {len(df)}")

    train_df, test_df = split_train_test(df, test_ratio=args.test_ratio, seed=args.seed)
    train_df.to_csv(output_dir / "aemc_train.csv", index=False)
    test_df.to_csv(output_dir / "aemc_test.csv", index=False)

    metrics = train_and_evaluate(
        train_df=train_df,
        test_df=test_df,
        criteria=criteria,
        hidden_depth=args.hidden_depth,
        epochs=args.epochs,
        learning_rate=args.learning_rate,
        weight_decay=args.weight_decay,
        batch_size=args.batch_size,
        seed=args.seed,
        output_dir=output_dir,
    )

    save_metrics(metrics, output_dir)
    print(f"\nRun completed. Output directory: {output_dir}")


if __name__ == "__main__":
    main()
