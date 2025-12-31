import argparse
from pathlib import Path
import json
import random
import pandas as pd
import numpy as np
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import LogisticRegression
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import classification_report
from sklearn.decomposition import TruncatedSVD
from sklearn.metrics.pairwise import cosine_similarity
import joblib

ROOT = Path(__file__).resolve().parent
PRODUCT_CSV = ROOT / 'product.csv'
MODELS_DIR = ROOT / 'models'
MODELS_DIR.mkdir(exist_ok=True)
ORDERS_CSV = ROOT / 'orders.csv'


def load_data(product_csv=PRODUCT_CSV):
    df = pd.read_csv(product_csv)
    for c in ['Id', 'Name', 'Specs', 'CategoryId', 'BrandId']:
        if c not in df.columns:
            df[c] = None
    df['Id'] = df['Id'].astype(int)
    df['Name'] = df['Name'].fillna('')
    df['Specs'] = df['Specs'].fillna('')
    df['text'] = (df['Name'].astype(str) + ' ' + df['Specs'].astype(str))
    return df


def generate_synthetic_orders(df_products, user_ids, n_orders=200, min_items=1, max_items=5, seed=42):
    random.seed(seed)
    prod_ids = df_products['Id'].tolist()
    orders = []
    order_id = 1
    # Ensure user 2 has some orders for demo
    demo_user = 2
    for _ in range(10):
        items = random.sample(prod_ids, k=random.randint(1, 5))
        orders.append({'OrderId': order_id, 'UserId': demo_user, 'ProductIds': items})
        order_id += 1
    # rest of random orders
    for _ in range(n_orders - 10):
        user = random.choice(user_ids)
        k = random.randint(min_items, max_items)
        items = random.sample(prod_ids, k=k)
        orders.append({'OrderId': order_id, 'UserId': user, 'ProductIds': items})
        order_id += 1
    orders_df = pd.DataFrame(orders)
    orders_df.to_csv(ORDERS_CSV, index=False)
    print(f"Saved synthetic orders to {ORDERS_CSV}")
    return orders_df


def expand_orders_to_interactions(orders_df):
    rows = []
    for _, r in orders_df.iterrows():
        pids = r['ProductIds'] if isinstance(r['ProductIds'], list) else json.loads(r['ProductIds'].replace("'", '"'))
        for pid in pids:
            rows.append({'UserId': int(r['UserId']), 'ProductId': int(pid), 'Count': 1})
    inter = pd.DataFrame(rows)
    agg = inter.groupby(['UserId', 'ProductId']).sum().reset_index()
    return agg


def train_classifier(X, y, vect=None):
    if vect is None:
        vect = TfidfVectorizer(max_features=5000, ngram_range=(1, 2))
        Xv = vect.fit_transform(X)
    else:
        Xv = vect.transform(X)
    le = LabelEncoder()
    y_enc = le.fit_transform(y.astype(str))
    if len(set(y_enc)) > 1:
        # If any class has fewer than 2 samples, stratify will fail. Fall back to non-stratified split in that case.
        unique, counts = np.unique(y_enc, return_counts=True)
        if counts.min() >= 2:
            X_train, X_val, y_train, y_val = train_test_split(
                Xv, y_enc, test_size=0.2, random_state=42, stratify=y_enc
            )
        else:
            print("Warning: some classes have fewer than 2 samples — using non-stratified split")
            X_train, X_val, y_train, y_val = train_test_split(
                Xv, y_enc, test_size=0.2, random_state=42
            )
    else:
        X_train, X_val, y_train, y_val = Xv, Xv, y_enc, y_enc
    clf = LogisticRegression(max_iter=2000)
    clf.fit(X_train, y_train)
    preds = clf.predict(X_val)
    report = classification_report(y_val, preds, zero_division=0)
    return {'vectorizer': vect, 'label_encoder': le, 'clf': clf, 'report': report}


def train_collaborative(interactions_df, n_components=32):
    # Build user-item matrix
    users = interactions_df['UserId'].unique()
    items = interactions_df['ProductId'].unique()
    user_idx = {u: i for i, u in enumerate(sorted(users))}
    item_idx = {p: i for i, p in enumerate(sorted(items))}
    M = np.zeros((len(users), len(items)), dtype=float)
    for _, r in interactions_df.iterrows():
        M[user_idx[r['UserId']], item_idx[r['ProductId']]] = r['Count']
    svd = TruncatedSVD(n_components=min(n_components, min(M.shape)-1), random_state=42)
    user_factors = svd.fit_transform(M)
    item_factors = svd.components_.T
    return {'svd': svd, 'user_index': user_idx, 'item_index': item_idx, 'user_factors': user_factors, 'item_factors': item_factors}


def recommend_cf(model, user_id, k=10):
    if user_id not in model['user_index']:
        return []
    uidx = model['user_index'][user_id]
    scores = model['item_factors'] @ model['user_factors'][uidx]
    item_list = sorted(model['item_index'].items(), key=lambda x: x[1])
    item_ids = [p for p, _ in item_list]
    ranked = sorted(zip(item_ids, scores), key=lambda x: -x[1])
    return [pid for pid, _ in ranked[:k]]


def recommend_content(vect, product_vectors, df_products, user_purchased_ids, k=10):
    purchased_idx = df_products[df_products['Id'].isin(user_purchased_ids)].index.tolist()
    if not purchased_idx:
        return []
    # Compute user profile as the mean of purchased product vectors.
    # Handle sparse matrices and numpy.matrix by converting to numpy arrays.
    pv = product_vectors[purchased_idx]
    user_profile = pv.mean(axis=0)
    # convert user_profile to ndarray
    try:
        # scipy sparse has .toarray(); numpy.matrix has .A
        if hasattr(user_profile, 'toarray'):
            user_profile = user_profile.toarray()
        elif hasattr(user_profile, 'A'):
            user_profile = np.asarray(user_profile.A)
        else:
            user_profile = np.asarray(user_profile)
    except Exception:
        user_profile = np.asarray(user_profile)

    # convert product_vectors to ndarray if sparse
    try:
        if hasattr(product_vectors, 'toarray'):
            pvecs = product_vectors.toarray()
        else:
            pvecs = np.asarray(product_vectors)
    except Exception:
        pvecs = np.asarray(product_vectors)

    sims = cosine_similarity(user_profile, pvecs).flatten()
    ranked = np.argsort(-sims)
    rec_ids = df_products.iloc[ranked]['Id'].tolist()
    # exclude purchased
    rec_filtered = [pid for pid in rec_ids if pid not in set(user_purchased_ids)]
    return rec_filtered[:k]


def save_model(prefix, obj):
    path = MODELS_DIR / f"{prefix}.joblib"
    joblib.dump(obj, path)
    print(f"Saved: {path}")


def main(args):
    df = load_data(args.product_csv)
    print(f"Products loaded: {len(df)}")

    # Generate synthetic orders
    users = list(range(2, 34))
    orders_df = generate_synthetic_orders(df, users, n_orders=args.n_orders)
    # Persist orders with ProductIds as JSON strings
    orders_df['ProductIds'] = orders_df['ProductIds'].apply(lambda x: json.dumps(x))
    orders_df.to_csv(ORDERS_CSV, index=False)

    # Expand to interactions
    interactions = expand_orders_to_interactions(orders_df)

    # Train content vectorizer on all product text
    vect = TfidfVectorizer(max_features=5000, ngram_range=(1, 2))
    prod_vectors = vect.fit_transform(df['text'])
    save_model('vect_products', vect)
    save_model('prod_vectors', prod_vectors)

    # Train CF
    cf_model = train_collaborative(interactions)
    save_model('cf_model', cf_model)

    # Train Category & Brand classifiers
    df_cat = df.dropna(subset=['CategoryId'])
    df_brand = df.dropna(subset=['BrandId'])
    print('\nTraining CategoryId classifier...')
    cat_res = train_classifier(df_cat['text'], df_cat['CategoryId'])
    print('Category classification report:\n', cat_res['report'])
    save_model('vect_category', cat_res['vectorizer'])
    save_model('le_category', cat_res['label_encoder'])
    save_model('clf_category', cat_res['clf'])

    print('\nTraining BrandId classifier...')
    brand_res = train_classifier(df_brand['text'], df_brand['BrandId'], vect=cat_res['vectorizer'])
    print('Brand classification report:\n', brand_res['report'])
    save_model('le_brand', brand_res['label_encoder'])
    save_model('clf_brand', brand_res['clf'])

    # Demo for Customer User id=2
    demo_user = 2
    user_purchased = interactions[interactions['UserId'] == demo_user]['ProductId'].tolist()
    print(f"\nCustomer User {demo_user} purchased {len(user_purchased)} unique products: {user_purchased[:10]}")

    cf_recs = recommend_cf(cf_model, demo_user, k=10)
    print('\nTop CF recommendations (product ids):', cf_recs)

    content_recs = recommend_content(vect, prod_vectors, df, user_purchased, k=10)
    print('\nTop Content-based recommendations (product ids):', content_recs)


if __name__ == '__main__':
    p = argparse.ArgumentParser()
    p.add_argument('--product-csv', default=str(PRODUCT_CSV))
    p.add_argument('--n-orders', type=int, default=200)
    args = p.parse_args()
    main(args)
