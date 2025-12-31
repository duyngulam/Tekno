from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from pathlib import Path
import joblib
import numpy as np
import pandas as pd
import json
from sklearn.metrics.pairwise import cosine_similarity

ROOT = Path(__file__).resolve().parent
MODELS = ROOT / 'models'
PRODUCT_CSV = ROOT / 'product.csv'
ORDERS_CSV = ROOT / 'orders.csv'

app = FastAPI(title='Trainning Model API')


class PredictRequest(BaseModel):
    product_id: int = None
    text: str = None


@app.on_event('startup')
def load_models():
    global df_products, vect_cat, le_cat, clf_cat, le_brand, clf_brand
    global vect_products, prod_vectors, cf_model
    df_products = pd.read_csv(PRODUCT_CSV)
    df_products['Name'] = df_products['Name'].fillna('')
    df_products['Specs'] = df_products['Specs'].fillna('')
    df_products['text'] = df_products['Name'].astype(str) + ' ' + df_products['Specs'].astype(str)

    vect_cat = joblib.load(MODELS / 'vect_category.joblib')
    le_cat = joblib.load(MODELS / 'le_category.joblib')
    clf_cat = joblib.load(MODELS / 'clf_category.joblib')

    le_brand = joblib.load(MODELS / 'le_brand.joblib')
    clf_brand = joblib.load(MODELS / 'clf_brand.joblib')

    vect_products = joblib.load(MODELS / 'vect_products.joblib')
    prod_vectors = joblib.load(MODELS / 'prod_vectors.joblib')

    cf_model = joblib.load(MODELS / 'cf_model.joblib')


def _predict_from_text(text: str):
    Xv = vect_cat.transform([text])
    cat = le_cat.inverse_transform(clf_cat.predict(Xv))[0]
    brand = le_brand.inverse_transform(clf_brand.predict(Xv))[0]
    return {'category': str(cat), 'brand': str(brand)}


@app.post('/predict')
def predict(req: PredictRequest):
    if req.product_id is None and not req.text:
        raise HTTPException(status_code=400, detail='Provide product_id or text')
    if req.product_id is not None:
        row = df_products[df_products['Id'] == req.product_id]
        if row.empty:
            raise HTTPException(status_code=404, detail='product_id not found')
        text = row['text'].iloc[0]
    else:
        text = req.text
    return _predict_from_text(text)


@app.get('/recommend/cf/{user_id}')
def recommend_cf_api(user_id: int, k: int = 10):
    if 'user_index' not in cf_model or 'item_index' not in cf_model:
        raise HTTPException(status_code=500, detail='CF model missing indexes')
    if user_id not in cf_model['user_index']:
        return {'recommendations': []}
    uidx = cf_model['user_index'][user_id]
    scores = cf_model['item_factors'] @ cf_model['user_factors'][uidx]
    item_list = sorted(cf_model['item_index'].items(), key=lambda x: x[1])
    item_ids = [p for p, _ in item_list]
    ranked = sorted(zip(item_ids, scores), key=lambda x: -x[1])
    return {'recommendations': [int(pid) for pid, _ in ranked[:k]]}


@app.get('/recommend/content/{user_id}')
def recommend_content_api(user_id: int, k: int = 10):
    # read orders and aggregate purchased by user
    try:
        orders = pd.read_csv(ORDERS_CSV)
    except Exception:
        return {'recommendations': []}
    # ProductIds stored as JSON strings
    bought = []
    for _, r in orders[orders['UserId'] == user_id].iterrows():
        try:
            pids = json.loads(r['ProductIds'])
        except Exception:
            pids = []
        bought.extend(pids)
    bought = list(set([int(x) for x in bought]))
    if not bought:
        return {'recommendations': []}

    purchased_idx = df_products[df_products['Id'].isin(bought)].index.tolist()
    pv = prod_vectors[purchased_idx]
    user_profile = pv.mean(axis=0)
    if hasattr(user_profile, 'toarray'):
        user_profile = user_profile.toarray()
    else:
        user_profile = np.asarray(user_profile)
    if hasattr(prod_vectors, 'toarray'):
        pvecs = prod_vectors.toarray()
    else:
        pvecs = np.asarray(prod_vectors)
    sims = cosine_similarity(user_profile, pvecs).flatten()
    ranked = np.argsort(-sims)
    rec_ids = df_products.iloc[ranked]['Id'].tolist()
    rec_filtered = [int(pid) for pid in rec_ids if pid not in set(bought)]
    return {'recommendations': rec_filtered[:k]}


@app.get('/health')
def health():
    return {'status': 'ok'}
