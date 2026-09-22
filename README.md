# FinTrack

**FinTrack** is a personal finance management web application built for students and young workers who want clear visibility into their money. Track income and expenses, set monthly budgets per spending category, manage savings goals, and visualise trends with interactive charts.

---

## Tech Stack

| Layer | Technology |
|---|---|
| **Frontend** | Blazor WebAssembly (.NET 10) |
| **Backend** | ASP.NET Core Web API (.NET 10) |
| **ORM** | Entity Framework Core 10 |
| **Database** | SQLite (dev) · SQL Server (production) |
| **Auth** | ASP.NET Core Identity + JWT + Refresh Tokens |
| **Charts** | Chart.js (via `fintrack-charts.js`) |
| **API Docs** | Scalar / OpenAPI (`.NET 10` native) |
| **Tests** | xUnit + `WebApplicationFactory` |

---

## Project Structure

```
FinTrack/
├── src/
│   ├── FinTrack.Server/      # ASP.NET Core Web API
│   │   ├── Controllers/      # HTTP endpoints
│   │   ├── Services/         # Business logic (Auth, Transactions, Budgets, …)
│   │   ├── Models/           # EF Core domain entities
│   │   ├── Data/             # FinTrackDbContext + migrations
│   │   └── Middleware/       # Global exception handling
│   ├── FinTrack.Client/      # Blazor WebAssembly app
│   │   ├── Pages/            # Dashboard, Transactions, Budgets, Reports, Login, Register
│   │   ├── Components/       # Shared UI components + modals
│   │   └── Services/         # API client + auth state
│   └── FinTrack.Shared/      # Shared DTOs (compiled into both projects)
│       └── DTOs/
├── tests/
│   └── FinTrack.Tests/       # xUnit integration & unit tests
└── docs/                     # All project documentation
```

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- `dotnet-ef` global tool:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## Running Locally

### 1 — Clone & restore

```bash
git clone https://github.com/dcit318-fintrack/FinTrack.git
cd FinTrack
dotnet restore
```

### 2 — Start the API server

```bash
cd src/FinTrack.Server
dotnet run
```

The API starts on `https://localhost:7xxx` (port shown in terminal).  
Interactive API explorer: `https://localhost:7xxx/scalar/v1`

### 3 — Start the Blazor client (separate terminal)

```bash
cd src/FinTrack.Client
dotnet run
```

Open the URL shown in the terminal (typically `https://localhost:5xxx`).

> **Note:** The server must be running before the client, as the client calls the API on startup.

---

## Running Tests

```bash
dotnet test tests/FinTrack.Tests
```

The test project uses `WebApplicationFactory<Program>` to spin up the API in-process with an in-memory database — no external services needed.

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
| Project Managers | Thomas Kutin · Lambert Nenemse |
| UI/UX Designers | Gerald Lamptey · Laudina Saifah |
| Backend Developers | Godsway Ahortor · Enoch Amankwah |
| Database & Integration | Michael Asante-Arhin · Akyeampong Ankama |
| Frontend Developers | Samuel Tettey · Ansah Ashwere |
| QA, Docs & Deployment | Anneta Dziedzom · Ashalley Adey |

---

## License

Academic project — DCIT318, University of Ghana, 2026.
