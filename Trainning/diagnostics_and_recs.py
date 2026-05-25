#!/usr/bin/env python3
import argparse
import json
from datetime import datetime
from pathlib import Path

import numpy as np
import pandas as pd


def compute_errors(pred_df: pd.DataFrame, ratings_df: pd.DataFrame) -> dict:
    # Flatten predictions to long
    preds_long = pred_df.reset_index().melt(id_vars=[pred_df.index.name or 'user_id'], var_name='product_id', value_name='pred_score')
    preds_long.columns = ['user_id', 'product_id', 'pred_score']
    preds_long['user_id'] = preds_long['user_id'].astype(int)
    preds_long['product_id'] = preds_long['product_id'].astype(str)
    ratings_df = ratings_df.copy()
    ratings_df['product_id'] = ratings_df['product_id'].astype(str)

    merged = ratings_df.merge(preds_long, on=['user_id', 'product_id'], how='inner')
    if merged.empty:
        return {'mae': None, 'rmse': None, 'count': 0}
    diff = merged['pred_score'] - merged['overall_rating']
    mae = float(np.mean(np.abs(diff)))
    rmse = float(np.sqrt(np.mean(diff ** 2)))
    return {'mae': mae, 'rmse': rmse, 'count': int(len(merged))}


def per_category_stats(pred_df: pd.DataFrame, products_df: pd.DataFrame, ratings_df: pd.DataFrame) -> pd.DataFrame:
    preds_long = pred_df.reset_index().melt(id_vars=[pred_df.index.name or 'user_id'], var_name='product_id', value_name='pred_score')
    preds_long.columns = ['user_id', 'product_id', 'pred_score']
    preds_long['product_id'] = preds_long['product_id'].astype(str)
    products_df = products_df.copy()
    products_df['product_id'] = products_df['product_id'].astype(str)
    merged = preds_long.merge(products_df, on='product_id', how='left')
    stats = merged.groupby('category')['pred_score'].agg(['mean','count']).rename(columns={'mean':'pred_mean','count':'pred_count'})

    # actuals per category
    ratings_df = ratings_df.copy()
    ratings_df['product_id'] = ratings_df['product_id'].astype(str)
    actual = ratings_df.merge(products_df, on='product_id', how='left')
    actual_stats = actual.groupby('category')['overall_rating'].agg(['mean','count']).rename(columns={'mean':'actual_mean','count':'actual_count'})

    df = stats.join(actual_stats, how='outer').fillna(0)
    return df.reset_index()


def group_topn_composition(topn_csv: Path, products_df: pd.DataFrame) -> pd.DataFrame:
    df = pd.read_csv(topn_csv)
    df = df.merge(products_df, left_on='item_id', right_on='product_id', how='left')
    comp = df.groupby(['user_group','category']).size().reset_index(name='count')
    total = df.groupby('user_group').size().reset_index(name='total')
    comp = comp.merge(total, on='user_group')
    comp['pct'] = comp['count'] / comp['total']
    return comp


def produce_recommendations(diagnostics: dict) -> list:
    recs = []
    # Simple heuristic recommendations
    if diagnostics['errors']['mae'] is None:
        recs.append('No overlapping (predictions vs ratings) found to compute MAE/RMSE.')
        return recs

    mae = diagnostics['errors']['mae']
    if mae > 0.8:
        recs.append('High MAE (>0.8): consider increasing model capacity or training epochs, and ensure per-user split is preserving signal.')
    elif mae > 0.4:
        recs.append('Moderate MAE: try adding category embeddings as features or rebalancing training by category.')
    else:
        recs.append('Low MAE: model reconstruction looks good; inspect recommendation diversity and popularity bias.')

    # category mismatch suggestions
    cat_df = diagnostics['per_category']
    if not cat_df.empty:
        # find categories where pred_mean - actual_mean large negative (model undervalues)
        cat_df['delta'] = cat_df['pred_mean'] - cat_df['actual_mean']
        under = cat_df.sort_values('delta').head(3)
        for _, r in under.iterrows():
            if abs(r['delta']) > 0.25:
                recs.append(f"Model underpredicts category {r['category']} (pred_mean={r['pred_mean']:.2f} vs actual={r['actual_mean']:.2f}). Consider upweighting category signals or increasing examples for this category.")

    # diversity suggestion
    comp = diagnostics.get('topn_comp')
    if comp is not None and not comp.empty:
        # if any user_group has >70% in one category, suggest diversity
        for group, gdf in comp.groupby('user_group'):
            top = gdf.sort_values('pct', ascending=False).iloc[0]
            if top['pct'] > 0.7:
                recs.append(f"Top-N for group '{group}' is dominated by category '{top['category']}' ({top['pct']:.0%}). Consider post-filtering or boosting other categories for diversity.")

    if not recs:
        recs.append('No specific recommendations detected; consider manual review of category balance and generator parameters.')
    return recs


def main():
    p = argparse.ArgumentParser()
    p.add_argument('--prediction-csv', required=True)
    p.add_argument('--ratings-csv', required=True)
    p.add_argument('--products-csv', required=True)
    p.add_argument('--users-csv', required=True)
    p.add_argument('--topn-csv', required=True)
    p.add_argument('--output-dir', default='outputs')
    args = p.parse_args()

    pred_df = pd.read_csv(args.prediction_csv, index_col=0)
    ratings_df = pd.read_csv(args.ratings_csv)
    products_df = pd.read_csv(args.products_csv)
    users_df = pd.read_csv(args.users_csv)

    diagnostics = {}
    diagnostics['errors'] = compute_errors(pred_df, ratings_df)
    diagnostics['per_category'] = per_category_stats(pred_df, products_df, ratings_df)
    diagnostics['topn_comp'] = group_topn_composition(Path(args.topn_csv), products_df)

    # persist diagnostics
    out = Path(args.output_dir)
    out.mkdir(parents=True, exist_ok=True)
    ts = datetime.now().strftime('%Y%m%d_%H%M%S')
    diag_json = out / f'diagnostics_{ts}.json'
    # convert per_category and topn_comp to serializable
    serial = {
        'errors': diagnostics['errors'],
        'per_category': diagnostics['per_category'].to_dict(orient='records'),
        'topn_comp': diagnostics['topn_comp'].to_dict(orient='records'),
    }
    diag_json.write_text(json.dumps(serial, indent=2))

    # produce textual recommendations
    recs = produce_recommendations({'errors': diagnostics['errors'], 'per_category': diagnostics['per_category'], 'topn_comp': diagnostics['topn_comp']})
    rec_file = out / f'recommendations_{ts}.txt'
    rec_file.write_text('\n'.join(recs))

    print('Diagnostics written to:', diag_json)
    print('Recommendations written to:', rec_file)


if __name__ == '__main__':
    main()
