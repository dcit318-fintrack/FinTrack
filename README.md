# FinTrack

**FinTrack** is a personal finance management web application built for students and young workers who want clear visibility into their money. Track income and expenses, set monthly budgets per spending category, manage savings goals, and visualise trends with interactive charts. All amounts are in Ghana cedis (GH₵).

## Live Demo

| | Link |
|---|---|
| **App (frontend)** | https://fintrack-we.netlify.app |
| **API (backend)** | https://fintrack-web.runasp.net/scalar/ |
| **API documentation** | https://fintrack-web.runasp.net/scalar/v1 |

> The backend runs on a free hosting tier that sleeps when idle, so the first request after a quiet period can take 20–30 seconds. After that it's fast.

To try it, create an account from the sign-up page. Passwords must be at least 8 characters and include an uppercase letter, a lowercase letter and a number.

---

## Features

- **Authentication** — register, log in and stay signed in with JWT access tokens and refresh tokens
- **Dashboard** — total balance, monthly income and expenses, recent transactions and savings progress at a glance
- **Transactions** — add, edit, delete, search and filter income and expenses by category and type
- **Budgets** — set a monthly spending limit per category and track how much of it is used
- **Savings goals** — create goals with a target amount and date, and contribute towards them
- **Reports** — spending by category and income vs expenses over time, for this month, the last 6 months or the year to date

---

## Tech Stack

| Layer | Technology |
|---|---|
| **Frontend** | Blazor WebAssembly (.NET 10) |
| **Backend** | ASP.NET Core Web API (.NET 10) |
| **ORM** | Entity Framework Core 10 |
| **Database** | SQL Server (production) · in-memory (local development and tests) |
| **Auth** | ASP.NET Core Identity + JWT + refresh tokens |
| **Charts** | Chart.js (via `fintrack-charts.js`) |
| **API Docs** | Scalar / OpenAPI (.NET 10 native) |
| **Tests** | xUnit + `WebApplicationFactory` |
| **Hosting** | Netlify (frontend) · MonsterASP.NET (API and database) |

---

## Project Structure

```
FinTrack/
├── src/
│   ├── FinTrack.Server/      # ASP.NET Core Web API
│   │   ├── Controllers/      # HTTP endpoints
│   │   ├── Services/         # Business logic (Auth, Transactions, Budgets, …)
│   │   ├── Models/           # EF Core domain entities
│   │   ├── Data/             # FinTrackDbContext
│   │   ├── Migrations/       # EF Core migrations
│   │   └── Middleware/       # Global exception handling
│   ├── FinTrack.Client/      # Blazor WebAssembly app
│   │   ├── Pages/            # Dashboard, Transactions, Budgets, Reports, Login, Register
│   │   ├── Components/       # Shared UI components + modals
│   │   ├── Services/         # API client + auth state
│   │   └── wwwroot/          # CSS, chart script, Netlify headers and redirects
│   └── FinTrack.Shared/      # Shared DTOs (compiled into both projects)
│       └── DTOs/
├── tests/
│   └── FinTrack.Tests/       # xUnit integration & unit tests
├── docs/                     # Project documentation
└── netlify.toml              # Netlify build, caching and API proxy settings
```

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

No database installation is needed to run locally — without a connection string, the API uses an in-memory database.

---

## Running Locally

### 1 — Clone and restore

```bash
git clone https://github.com/dcit318-fintrack/FinTrack.git
cd FinTrack
dotnet restore FinTrack.sln
```

> The repository contains two solution files, so name `FinTrack.sln` explicitly for `restore`, `build` and `test` commands run from the root.

### 2 — Start the API server

```bash
dotnet run --project src/FinTrack.Server
```

The API starts on `http://localhost:5104`.
Interactive API explorer: `http://localhost:5104/scalar/v1`

### 3 — Start the Blazor client (separate terminal)

```bash
dotnet run --project src/FinTrack.Client
```

Open `http://localhost:5075` in your browser.

> Start the server first — the client calls the API as soon as you sign up or log in.
> The local in-memory database is cleared whenever the server restarts, so you'll need to register again after a restart.

---

## Running Tests

```bash
dotnet test FinTrack.sln
```

The tests use `WebApplicationFactory<Program>` to run the API in-process with an in-memory database — no external services needed. The suite covers authentication, transactions, budgets, savings goals, the dashboard, reports and client hosting.

---

## Deployment

The app is deployed as a separate frontend and backend.

**Frontend — Netlify.** A GitHub Actions workflow (`.github/workflows/deploy-client.yml`) publishes the Blazor client and deploys it to Netlify on every push to `main`.

**Backend — MonsterASP.NET.** The API and its SQL Server database are hosted on MonsterASP.NET. Database migrations are applied automatically when the API starts.

**How they connect.** The frontend calls the API through a Netlify proxy: `netlify.toml` forwards every request under `/api/*` to the MonsterASP backend over HTTPS. Because the browser only ever talks to the Netlify domain, there are no cross-origin (CORS) issues in production.

**Configuration.** Secrets are supplied as environment settings on the host, never committed to the repository:

| Setting | Purpose |
|---|---|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string (enables SQL Server instead of in-memory) |
| `Jwt__Secret` | Signing key for access tokens |
| `Jwt__Issuer` / `Jwt__Audience` | Token issuer and audience |
| `Cors__AllowedOrigins__0` | Allowed frontend origin |

---

## Known Limitations

- **Transactions are timestamped automatically.** A transaction is recorded with the time it's added, so earlier dates can't be backdated.
- **Savings contributions are tracked separately.** Contributing to a savings goal updates the goal's progress but isn't recorded as a transaction, so it doesn't reduce the dashboard balance.
- **Currency is fixed to Ghana cedis (GH₵).**
- **First load can be slow.** The free hosting tier sleeps when idle, and Blazor WebAssembly downloads the .NET runtime on a first visit. Later visits are faster.

---

## Documentation

| Document | Description |
|---|---|
| [User Guide](docs/user_guide.md) | Step-by-step guide for end users of the app |
| [Technical Guide](docs/technical_guide.md) | Architecture, setup, and developer reference |
| [API Contract](docs/api_contract.MD) | Request/response shapes agreed between Frontend & Backend |
| [Data Model](docs/data-model.md) | Database schema, constraints, and seed data |
| [Test Plan](docs/test_plan.md) | Full test coverage plan (automated + manual) |
| [OpenAPI Spec](docs/fintrack_openapi_spec.json) | Machine-readable API spec (import into Postman etc.) |
| [Sprint Plan](docs/sprint_plan.MD) | Two-week sprint breakdown and issue assignments |

---

## Team

| Role | Members |
|---|---|
| Project Managers | Thomas Theophilus Kwasi Kutin · Lambert Klenam Nenemse |
| UI/UX Designers | Gerald Nii Lantey Lamptey · Laudina Saifah |
| Backend Developers | Godsway Ewoenam Kwadzo Ahortor · Enoch Kwabena Boampong Amankwah |
| Database & Integration | Michael Asante-Arhin · Akyeampong Papa Kwadwo Ankama |
| Frontend Developers | Samuel Watson Tettey · Ansah Louis Kwaku Ashwere |
| QA, Docs & Deployment | Anneta Dziedzom Dzibolosu · Ashalley Matilda Adey |

---

## License

Academic project — DCIT 318, University of Ghana, 2026.
