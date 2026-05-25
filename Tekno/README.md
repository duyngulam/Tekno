# Tekno
open terminal at solution and run this command:
docker compose up --build
and open
http://localhost:5000/swagger
on browser to test API with swagger
testing account:
admin@tekno.com
admin123
customer@tekno.com
customer123
Turn off the Api in docker before running with any other method.
VN pay test card:
#	Thông tin thẻ	Ghi chú
1	
Ngân hàng: NCB
Số thẻ: 9704198526191432198
Tên chủ thẻ:NGUYEN VAN A
Ngày phát hành:07/15
Mật khẩu OTP:123456
# Tekno

Tekno is an open-source e-commerce implementing a full-featured shopping platform (catalog, cart, checkout, payments, promotions, reviews, admin reporting). It follows a layered/clean architecture with separated projects for API, application logic, domain models and infrastructure.
Summary
- Backend: ASP.NET Core Web API implementing products, cart, orders, payments, promotions and admin endpoints.
- Frontend: Next.js + React + TypeScript + Tailwind (located at `Frontend/tekno_fe`).

Tech stack
- Backend: .NET (ASP.NET Core), Entity Framework Core, Npgsql
- Search: Elasticsearch (Nest client)
- Cache: Redis
- Auth: JWT (symmetric secret)
- Payments: VNPay (sandbox) + mock/other providers
- Frontend: Next.js (React) + TypeScript + Tailwind CSS
- Dev & Ops: Docker, Docker Compose

- Clean architecture: separation between `Api` (controllers), `Application` (use-cases), `Domain` (entities) and `Infrastructure` (persistence & external services).
- Robust integrations: PostgreSQL for persistence, Redis for caching, Elasticsearch for search, Cloudinary for media, VNPay & other payment options.
- Production-focused: Docker compose orchestration, hosted background services, structured logging, JWT auth and Swagger for API discoverability.

Project layout
- `Tekno.Api` — Web API, authentication, Swagger, DI setup and host configuration.
- `Tekno.Application` — business use-cases, DTOs, validators and application services.
- `Tekno.Domain` — domain entities, enums and value objects.
- `Tekno.Infrastructure` — EF Core persistence, repositories, external API clients, background workers.
- `Tekno.Database` — CSV seed data and DB scripts.
- `Frontend/tekno_fe` — Next.js client application.

Key design notes (short)
- Configuration is read from `appsettings.json` and may be overridden by environment variables. Environment overrides use either `DB_CONNECTION_STRING` or double-underscore naming (e.g. `VNPay__TmnCode`).
- Hosted/background services are used for periodic tasks (province fetch, coupon expiration, promotion management).
- Swagger has integrated JWT support to exercise protected endpoints during interviews or demos.

Getting started

Prerequisites
- Docker & Docker Compose
- .NET SDK (7+ recommended)
- Node.js & npm (for frontend)

Run everything (recommended)
```powershell
docker compose up --build
```

Run backend locally (API only)
1. Copy `.env.example` to `.env` and update secrets if needed.
2. From repository root run:
```powershell
dotnet run --project Tekno.Api/Tekno.Api.csproj
```

Run frontend locally
```powershell
cd Frontend/tekno_fe
npm install
npm run dev
```

Useful URLs
- Swagger (API explorer): http://localhost:5000/swagger
- Frontend (Next.js dev): http://localhost:3000 (when running locally)

Environment variables (see `.env.example`)
- `DB_CONNECTION_STRING` or `ConnectionStrings__DefaultConnection` — Postgres
- `JWT_SECRET_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_EXPIRY_MINUTES`
- `Redis__ConnectionString`
- `VNPay__TmnCode`, `VNPay__HashSecret`, `VNPay__PaymentUrl`, `VNPay__ReturnUrl`, `VNPay__IpnUrl`

VNPay sandbox test card (kept minimal)
- Ngân hàng: NCB
- Số thẻ: 9704198526191432198
- Tên chủ thẻ: NGUYEN VAN A
- Ngày phát hành: 07/15
- Mật khẩu OTP: 123456

Quick demo endpoints to mention in interviews
- `POST /api/auth/register` — create user
- `POST /api/auth/login` — get JWT token
- `GET /api/products` — list products
- `POST /api/cart/items` — add item to cart
- `POST /api/payment/process` — start payment flow (VNPay)
Contact
- Repo reference / maintainer: see project metadata or Git history for author details.
