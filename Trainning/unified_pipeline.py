#!/usr/bin/env python3
"""Unified pipeline: regenerate seedata, build long CSV, train AEMC, export top-N.

Usage: run from the AEMC project root (where other scripts live).
Example (dry-run):
    python unified_pipeline.py --dry-run --users 2000 --products 500
Example (skip seeddata regeneration and just retrain/export):
    python unified_pipeline.py --skip-seeddata --tmp-dir seedata_run_500x100 --out-dir outputs_test_500x100
"""
import argparse
import subprocess
import sys
from pathlib import Path
import shutil

ROOT = Path(__file__).parent

def run_cmd(cmd, dry_run=False, cwd=None):
    print('RUN:', ' '.join(cmd))
    if dry_run:
        return
    subprocess.run(cmd, check=True, cwd=cwd)

def find_latest_overall(out_dir):
    files = sorted(Path(out_dir).glob('aemc_overall_predictions_*.csv'), key=lambda p: p.stat().st_mtime)
    return files[-1] if files else None

def main():
    p = argparse.ArgumentParser()
    p.add_argument('--users', type=int, default=2000)
    p.add_argument('--products', type=int, default=500)
    p.add_argument('--seed', type=int, default=42)
    p.add_argument('--seed-source-dir', default='seeddata2.0', help='Base users.csv/products.csv source directory')
    p.add_argument('--out-dir', default='outputs')
    p.add_argument('--tmp-dir', default='seedata_run_tmp')
    p.add_argument('--epochs', type=int, default=8)
    p.add_argument('--hidden-depth', type=int, default=3)
    p.add_argument('--learning-rate', type=float, default=0.005)
    p.add_argument('--weight-decay', type=float, default=0.0005)
    p.add_argument('--batch-size', type=int, default=128)
    p.add_argument('--test-ratio', type=float, default=0.2)
    p.add_argument('--top-n', type=int, default=10)
    p.add_argument('--users-per-group', type=int, default=3)
    p.add_argument('--skip-seeddata', action='store_true', help='Skip expand/generate and reuse an existing tmp-dir with matrices/long_ratings.csv')
    p.add_argument('--dry-run', action='store_true')
    args = p.parse_args()

    python = sys.executable
    tmp = ROOT / args.tmp_dir
    out = ROOT / args.out_dir
    out.mkdir(parents=True, exist_ok=True)

    if not args.skip_seeddata:
        # ensure clean tmp when regenerating seed data from scratch
        if tmp.exists():
            shutil.rmtree(tmp)
        tmp.mkdir(parents=True, exist_ok=True)
    elif not tmp.exists():
        raise FileNotFoundError(f"skip-seeddata was requested but tmp-dir does not exist: {tmp}")

    long_out = tmp / 'long_ratings.csv'

    if not args.skip_seeddata:
        seed_source = ROOT / args.seed_source_dir

        # 1) expand seedata into tmp (wrapper; keeps original seed files intact)
        expand_cmd = [
            python,
            str(ROOT / 'expand_seeddata.py'),
            '--seedata-dir',
            str(seed_source),
            '--out-dir',
            str(tmp),
            '--users',
            str(args.users),
            '--products',
            str(args.products),
            '--seed',
            str(args.seed),
            '--include-low-range',
        ]
        run_cmd(expand_cmd, dry_run=args.dry_run, cwd=ROOT)

        # 2) generate wide criterion matrices into tmp
        gen_cmd = [python, str(ROOT / 'generate_seeddata.py'), '--seedata-dir', str(tmp), '--seed', str(args.seed)]
        run_cmd(gen_cmd, dry_run=args.dry_run, cwd=ROOT)

        # 3) convert to long format
        tolong_cmd = [python, str(ROOT / 'seedata_to_long.py'), '--seedata-dir', str(tmp), '--output', str(long_out)]
        run_cmd(tolong_cmd, dry_run=args.dry_run, cwd=ROOT)
    elif not long_out.exists():
        raise FileNotFoundError(f"skip-seeddata was requested but long_ratings.csv is missing: {long_out}")

    # 4) train AEMC (kaggle long-runner)
    train_cmd = [
        python,
        str(ROOT / 'kaggle_run_aemc_standalone.py'),
        '--input',
        str(long_out),
        '--output-dir',
        str(out),
        '--epochs',
        str(args.epochs),
        '--hidden-depth',
        str(args.hidden_depth),
        '--learning-rate',
        str(args.learning_rate),
        '--weight-decay',
        str(args.weight_decay),
        '--batch-size',
        str(args.batch_size),
        '--test-ratio',
        str(args.test_ratio),
    ]
    run_cmd(train_cmd, dry_run=args.dry_run, cwd=ROOT)

    # 5) locate overall prediction and export top-N
    if args.dry_run:
        print('Dry-run complete; no export performed.')
        return

    overall = find_latest_overall(out)
    if overall is None:
        print('No overall prediction file found in', out)
        sys.exit(1)

    export_cmd = [
        python,
        str(ROOT / 'export_topn_recommendations.py'),
        '--prediction-csv',
        str(overall),
        '--users-csv',
        str(tmp / 'users.csv'),
        '--ratings-csv',
        str(long_out),
        '--top-n',
        str(args.top_n),
        '--exclude-seen',
        '--all-users',
        '--products-csv',
        str(tmp / 'products.csv'),
        '--diversify',
        '--max-per-category',
        '5',
    ]
    run_cmd(export_cmd, dry_run=False, cwd=ROOT)

    # Always produce diagnostics and recommendations for this run
    # Use the export file we just wrote as topn-csv by locating the latest export.
    exports = sorted(Path(out).glob('topn_export_*.csv'), key=lambda p: p.stat().st_mtime)
    if exports:
        diag_cmd = [
            python,
            str(ROOT / 'diagnostics_and_recs.py'),
            '--prediction-csv',
            str(overall),
            '--ratings-csv',
            str(long_out),
            '--products-csv',
            str(tmp / 'products.csv'),
            '--users-csv',
            str(tmp / 'users.csv'),
            '--topn-csv',
            str(exports[-1]),
            '--output-dir',
            str(out),
        ]
        run_cmd(diag_cmd, dry_run=False, cwd=ROOT)

    print('Unified pipeline finished. Predictions:', overall)

if __name__ == '__main__':
    main()
