Setup

1. Create a virtual environment (Windows PowerShell):

```powershell
python -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r requirements.txt
```

2. Run training (will save models in `Trainning/models`):

```powershell
python train.py
```

3. Demo: predictions for Customer User id=2 are printed after training; to pass specific product ids:

```powershell
python train.py --demo-ids 99 100 101
```

Notes
- Script trains two classifiers: predicting `CategoryId` and `BrandId` from product `Name`+`Specs` text.
- `product.csv` must be located at `Trainning/product.csv` (script default). Adjust `--product-csv` if needed.
- For a proper user-personalized demo you can add user-product interactions and extend the pipeline to predict user preferences.
Additional features
- Synthetic orders: the script now generates synthetic orders (`Trainning/orders.csv`) used as user-product interactions.
- Collaborative filtering: a simple SVD-based CF model is trained on the interaction matrix and saved to `Trainning/models/cf_model.joblib`.
- Content-based recommendations: product TF-IDF vectors are saved and used to compute cosine-similarity based recommendations per user.

Usage examples

Generate synthetic orders and train everything (default 200 orders):

```powershell
python train.py --n-orders 300
```

After running, demo recommendations for Customer User id=2 are printed: both CF and content-based top picks (product ids).

Files produced
- `Trainning/orders.csv` — synthetic orders with `OrderId`, `UserId`, `ProductIds`.
- `Trainning/models` — saved models: TF-IDF vectorizer (`vect_products.joblib`), product vectors, CF model, classifiers.

API
---
You can run a small HTTP API that exposes prediction and recommendation endpoints.

1. Install API dependencies (inside the same venv):
```powershell
pip install -r requirements.txt
```

2. Start the API (from `Trainning` folder):
```powershell
uvicorn api:app --host 0.0.0.0 --port 8000
```

3. Endpoints:
- `POST /predict` JSON body `{ "product_id": 10 }` or `{ "text": "product title specs" }` → returns `category` and `brand`.
- `GET /recommend/cf/{user_id}?k=10` → returns CF recommendations (product ids).
- `GET /recommend/content/{user_id}?k=10` → returns content-based recommendations (product ids).

The API loads models from `Trainning/models` and uses `Trainning/orders.csv` to build user purchase lists for content recommendations.

Docker
------
The repository `docker-compose.yml` now includes a `trainning_api` service that runs the FastAPI app. To run everything with Docker Compose:

```powershell
docker compose build
docker compose up
```

The .NET API will receive the environment variable `TRAINING_API_URL` and can call the training API at `http://trainning_api:8000` inside the Docker network.

Next steps
- You can improve CF by using the `implicit` library (ALS) or training a neural model. If you want, I can integrate `implicit` and generate better metrics.
