# Distributor Management System (DMS)

Production-oriented full-stack DMS reference implementation:
- **Frontend:** React + Vite + Tailwind + Redux Toolkit + React Router
- **Backend:** ASP.NET Core Web API + Clean Architecture + JWT RBAC
- **Database:** PostgreSQL + EF Core with partition/index/read-replica strategy

## Repository Structure

```text
DMS/
├── backend/
│   └── src/
│       ├── Dms.Api/             # Controllers, Program.cs, Swagger
│       ├── Dms.Application/     # DTOs, interfaces, contracts
│       ├── Dms.Domain/          # Entities and enums
│       ├── Dms.Infrastructure/  # Services (auth, dashboard, reports), DI
│       └── Dms.Persistence/     # DbContext, EF configs, repositories
└── frontend/
    └── src/
        ├── app/                 # Redux store/hooks
        ├── components/          # Main layout, header, sidebar
        ├── features/            # Dashboard, master, transactions modules
        ├── pages/               # Login, distributor selection
        ├── router/              # Route map
        └── services/            # Axios API client
```

## Backend Architecture

### Clean flow
`Controller -> Service -> Repository -> EF Core/PostgreSQL`

### Implemented APIs
- `POST /api/auth/login`
- `POST /api/auth/forgot-password`
- `POST /api/auth/reset-password`
- `GET /api/distributors/my`
- `GET /api/dashboard/sales`
- `GET/POST/PUT/DELETE /api/master/items`
- `GET/POST /api/transactions/sales-orders`
- `GET /api/reports/sales`

Swagger is enabled in all environments.

### Security
- JWT authentication + role-based authorization (Admin, Distributor, Dealer)
- CAPTCHA validation contract in login flow
- Forgot/reset password workflow with token store

## Database Schema & Scalability

### Primary tables
- `users`
- `distributors`
- `user_distributors`
- `items`
- `sales_orders`

### Indexing strategy
- unique: `users.username`, `users.email`, `distributors.code`, `items.item_code`, `sales_orders.order_number`
- reporting/query indexes: `sales_orders(distributor_id, order_date)`, `items(name)`

### Partitioning strategy (PostgreSQL)
- `sales_orders` designed as a partition parent using `RANGE(order_date)`
- monthly partitions (`sales_orders_YYYY_MM`) via SQL migration and scheduler/pg_partman

### Read-replica strategy
- Separate `ReadReplicaConnection` for report-heavy queries
- `SalesOrderRepository` routes reporting reads through read-only DbContext factory

## Frontend Features

- Login page with username/password/CAPTCHA and show/hide password toggle
- Distributor selection after login (global Redux state)
- Collapsible left sidebar with hover labels/tooltips style
- Header with centered search and user dropdown actions
- Sales dashboard with KPI cards + line/bar charts
- Master module (Item Master CRUD list view + search/pagination-ready API)
- Transactions module (Sales Order create flow + loading state)
- Scaffolded routes/pages for Reports, Sync, Configuration, Security, Claims, Tools

## Async and Performance

- Async API interactions end-to-end (`Task` in backend, async calls in frontend)
- Pagination contracts in master/transactions endpoints
- Distributed cache abstraction in password reset flow (replaceable by Redis)
- Read/write query separation strategy for future scaling

## Run (local)

### Frontend
```bash
cd frontend
npm install
npm run dev
```

### Backend
```bash
cd backend/src/Dms.Api
dotnet restore
dotnet run
```

## Next Production Steps

1. Add database migrations and seed scripts.
2. Replace CAPTCHA stub with provider verification API.
3. Replace in-memory cache with Redis and integrate queue processor (Hangfire/RabbitMQ).
4. Add full modules for Purchase, Claims workflow, Import/Export field mapping UI and log persistence.
5. Add observability (OpenTelemetry, central logs, metrics dashboards).
