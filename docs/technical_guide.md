# FinTrack — Technical Guide

**Version:** 1.0 (Week 2 draft)  
**Audience:** Developers working on the FinTrack codebase  
**Last updated:** August 2026

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Repository Structure](#2-repository-structure)
3. [Prerequisites & Local Setup](#3-prerequisites--local-setup)
4. [Architecture Overview](#4-architecture-overview)
5. [Authentication Flow](#5-authentication-flow)
6. [Key Design Decisions](#6-key-design-decisions)
7. [Running the Tests](#7-running-the-tests)
8. [Configuration Reference](#8-configuration-reference)
9. [Deployment](#9-deployment)
10. [Reference Documents](#10-reference-documents)

---

## 1. Project Overview

FinTrack is a full-stack personal finance tracker:

- **Backend:** ASP.NET Core 10 Web API with EF Core 10 and ASP.NET Core Identity
- **Frontend:** Blazor WebAssembly (.NET 10), communicating with the backend via HTTP
- **Shared:** A class library (`FinTrack.Shared`) containing DTOs compiled into both the server and the client so both sides always use the same request/response types
- **Tests:** xUnit integration tests using `WebApplicationFactory<Program>` (in-process, no real HTTP)

---

## 2. Repository Structure

```
FinTrack/
├── FinTrack.sln                    # Solution file (all three projects)
├── FinTrack.slnx                   # Alternative slnx format
├── docs/
│   ├── user_guide.md               # End-user documentation (this issue)
│   ├── technical_guide.md          # Developer documentation (this file)
│   ├── api_contract.MD             # Request/response contract (Frontend ↔ Backend)
│   ├── data-model.md               # Database schema & seed data spec
│   ├── test_plan.md                # Full QA test plan
│   ├── sprint_plan.MD              # Sprint breakdown & issue assignments
│   ├── fintrack_openapi_spec.json  # Machine-readable OpenAPI 3.x spec
│   └── reference/                  # PM-authored reference docs
│       ├── 01_project_overview_and_spec.md
│       ├── 02_backend_and_database_architecture.md
│       └── 03_github_backend_issues_matrix.md
├── src/
│   ├── FinTrack.Server/
│   │   ├── Controllers/            # AuthController, TransactionsController, BudgetsController,
│   │   │                           #   SavingsGoalsController, DashboardController,
│   │   │                           #   ReportsController, CategoriesController
│   │   ├── Services/
│   │   │   ├── Auth/               # IAuthService, AuthService, IJwtTokenGenerator, JwtTokenGenerator
│   │   │   ├── Transactions/       # ITransactionService, TransactionService
│   │   │   ├── Budgets/            # IBudgetService, BudgetService
│   │   │   ├── Savings/            # ISavingsGoalService, SavingsGoalService
│   │   │   ├── Dashboard/          # IDashboardService, DashboardService
│   │   │   └── Reports/            # IReportService, ReportService
│   │   ├── Models/                 # ApplicationUser, Category, Transaction, Budget, SavingsGoal
│   │   ├── Data/                   # FinTrackDbContext (EF Core DbContext + seed)
│   │   ├── Middleware/             # ApiExceptionMiddleware (global error handler)
│   │   └── Program.cs              # DI registration, JWT config, middleware pipeline
│   ├── FinTrack.Client/
│   │   ├── Pages/                  # Dashboard.razor, Transactions.razor, Budgets.razor,
│   │   │                           #   Reports.razor, Login.razor, Register.razor
│   │   ├── Components/             # AppHeader, BottomNav, TransactionModal, BudgetModal,
│   │   │                           #   SavingsModal, DoughnutChart, IncomeExpenseChart,
│   │   │                           #   CategoryIcon
│   │   ├── Services/               # IFinTrackApiService / FinTrackApiService (HTTP client wrapper)
│   │   │                           # IAuthService / AuthService (JWT storage & state)
│   │   ├── Layout/                 # MainLayout.razor
│   │   └── wwwroot/
│   │       ├── js/fintrack-charts.js   # Chart.js integration
│   │       └── images/avatar.svg
│   └── FinTrack.Shared/
│       └── DTOs/
│           ├── Auth/               # LoginRequest, RegisterRequest, AuthResponse, RefreshTokenRequest
│           ├── Transaction/        # TransactionDto, CreateTransactionRequest, UpdateTransactionRequest, PagedResult
│           ├── Budget/             # BudgetDto, CreateBudgetRequest, UpdateBudgetRequest
│           ├── Savings/            # SavingsGoalDto, CreateSavingsGoalRequest, UpdateSavingsGoalRequest, ContributeRequest
│           ├── Dashboard/          # DashboardSummaryDto
│           ├── Report/             # ReportDtos (SpendingByCategoryDto, IncomeVsExpenseDto, …)
│           ├── Category/           # CategoryDto, CreateCategoryRequest
│           └── Common/             # ErrorResponse
└── tests/
    └── FinTrack.Tests/
        ├── IntegrationTestBase.cs      # WebApplicationFactory base class + auth helpers
        ├── Auth/                       # AuthTests.cs
        ├── Transactions/               # TransactionTests.cs (+ TransactionServiceTests.cs)
        ├── Budgets/                    # BudgetTests.cs
        ├── SavingsGoals/               # SavingsGoalTests.cs
        ├── Dashboard/                  # DashboardTests.cs (+ DashboardServiceTests.cs)
        └── Reports/                    # ReportsTests.cs (+ ReportServiceTests.cs)
```

---

## 3. Prerequisites & Local Setup

### Required tools

| Tool | Version | Install |
|---|---|---|
| .NET SDK | 10.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/10.0) |
| dotnet-ef | latest | `dotnet tool install --global dotnet-ef` |
| Git | any | [git-scm.com](https://git-scm.com) |

### Clone and restore

```bash
git clone https://github.com/dcit318-fintrack/FinTrack.git
cd FinTrack
dotnet restore
```

### Run the API (FinTrack.Server)

```bash
cd src/FinTrack.Server
dotnet run
```

- Default URL: `https://localhost:7xxx` (check the terminal output for the exact port).
- The in-memory database is seeded with the 7 default categories on startup (`EnsureCreated()` + seed logic in `FinTrackDbContext`).
- Interactive API explorer (development only): `https://localhost:7xxx/scalar/v1`

### Run the Blazor client (FinTrack.Client)

Open a **second** terminal:

```bash
cd src/FinTrack.Client
dotnet run
```

- The client app URL is displayed in the terminal (typically `https://localhost:5xxx`).
- The client reads the API base URL from its configuration. If the server port changes, update `appsettings.json` in `FinTrack.Client` accordingly.

---

## 4. Architecture Overview

```
Browser
  │  (HTTPS)
  ▼
FinTrack.Client  (Blazor WASM — runs entirely in the browser after first load)
  │  HttpClient (JSON, Bearer token attached by AuthService)
  ▼
FinTrack.Server  (ASP.NET Core Web API)
  ├── ApiExceptionMiddleware  ─── catches all unhandled exceptions → { message, errors } JSON
  ├── JWT Bearer Middleware   ─── validates Authorization header
  ├── Controllers             ─── thin; delegate to Services
  ├── Services                ─── business logic; return DTOs
  └── FinTrackDbContext       ─── EF Core; SQLite (dev) or SQL Server (prod)
```

### Layering rules

- **Controllers** must not contain business logic. They call a service method and return the result.
- **Services** own all business logic and data access. They never return EF entity objects — they map to/from DTOs.
- **DTOs** live in `FinTrack.Shared` so the Blazor client compiles against the same types without duplicating code.

### Database

The `FinTrackDbContext` uses:
- **In-memory EF Core** (current dev default, configured in `Program.cs`)
- Can be switched to SQLite or SQL Server by changing the `AddDbContext` call and providing a connection string

Categories are seeded once via `HasData` / `EnsureCreated` at startup. All other data is per-user.

---

## 5. Authentication Flow

FinTrack uses **JWT Bearer tokens** with **Refresh Tokens**.

```
Client                          Server
  │                               │
  │── POST /api/auth/register ───▶│  Create user (Identity), hash password
  │◀─ 201 { accessToken,          │  Generate JWT (15-min default) + refresh token
  │         refreshToken,         │  Return both to client
  │         expiresAt }           │
  │                               │
  │── POST /api/auth/login ──────▶│  Verify credentials via UserManager
  │◀─ 200 { same shape }          │
  │                               │
  │── GET /api/dashboard ────────▶│  Middleware reads Authorization: Bearer <token>
  │  (Authorization: Bearer …)    │  Validates signature, issuer, audience, expiry
  │◀─ 200 { … }                   │  ClaimsPrincipal available via User.FindFirst(…)
  │                               │
  │── POST /api/auth/refresh ────▶│  Validate refresh token, issue new JWT
  │◀─ 200 { new accessToken,      │
  │         refreshToken }        │
```

### JWT configuration

Set in `appsettings.json` (or environment variables for production):

```json
{
  "Jwt": {
    "Secret": "<long random string — min 64 chars>",
    "Issuer": "FinTrackServer",
    "Audience": "FinTrackClient"
  }
}
```

The `Program.cs` fallback secret is for development only and **must be overridden in production**.

### User identity in controllers

Every controller action that needs the current user calls:

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
```

All service methods receive `userId` as a parameter and scope every query to it.

---

## 6. Key Design Decisions

### Data isolation — 404 not 403

When a user requests a resource that belongs to another user, the API returns **404 Not Found**, not 403 Forbidden. This prevents leaking the existence of other users' records. This is enforced in every service method by filtering on `UserId` before returning or throwing.

### Money is `decimal`, never `float`

All monetary fields (`Amount`, `Limit`, `TargetAmount`, `CurrentAmount`) are `decimal(18,2)` in the database and `decimal` in C#. Using `float` or `double` for money causes rounding drift and is explicitly banned in this codebase.

### Dates are UTC ISO 8601

All date/time values are stored and returned as UTC. The API rejects transaction dates in the future (>UTC now). The month filter for budgets and dashboard uses the `"YYYY-MM"` string format, not a full datetime.

### Spent / Remaining computed server-side

Budget `spent` and `remaining` are **never stored** — they are calculated at query time by summing transactions for the matching `userId`, `categoryId`, and `month`. The client does not calculate these.

### Zero-filled report periods

The income-vs-expense report endpoint fills in zero-value data points for any period within the requested range that has no transactions. This ensures charts are continuous with no gaps.

### Error envelope

All non-2xx responses follow this shape:

```json
{
  "message": "Human-readable summary",
  "errors": {
    "fieldName": ["Validation message"]
  }
}
```

`errors` is only present when there are field-level validation problems. `ApiExceptionMiddleware` catches all unhandled exceptions and returns this shape — the API must never return a raw 500 error to the client.

---

## 7. Running the Tests

```bash
# Run all tests
dotnet test tests/FinTrack.Tests

# Run with verbose output
dotnet test tests/FinTrack.Tests --logger "console;verbosity=detailed"

# Run a specific test class
dotnet test tests/FinTrack.Tests --filter "FullyQualifiedName~AuthTests"
```

### Test infrastructure

- `IntegrationTestBase` spins up `FinTrack.Server` in-process using `WebApplicationFactory<Program>`.
- Each test run gets a fresh in-memory database — no state bleeds between test classes.
- To authenticate: call `RegisterAndGetTokenAsync()` from the base class, then use `AuthenticatedClient(token)` to get a pre-configured `HttpClient`.

### Test project layout

| Folder | What it tests |
|---|---|
| `Auth/` | Register, login, refresh, protected-endpoint guard |
| `Transactions/` | CRUD, pagination, filters, validation, data isolation |
| `Budgets/` | CRUD, duplicate constraint, spent/remaining calculation |
| `SavingsGoals/` | CRUD, contribution, achieved flag |
| `Dashboard/` | Balance calculation, recent transactions cap, at-risk threshold |
| `Reports/` | Category breakdown, income-vs-expense, zero-fill, granularity options |

Tests marked `[Skip]` are written but waiting for their endpoint to be implemented. Remove the `Skip` attribute when the endpoint lands.

---

## 8. Configuration Reference

### FinTrack.Server — appsettings.json

```json
{
  "Jwt": {
    "Secret": "<min 64-char random string>",
    "Issuer": "FinTrackServer",
    "Audience": "FinTrackClient"
  },
  "ConnectionStrings": {
    "DefaultConnection": "<SQL Server or SQLite connection string>"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### FinTrack.Server — appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

> The current `Program.cs` uses `UseInMemoryDatabase` regardless of the connection string. To switch to SQLite, replace `options.UseInMemoryDatabase(...)` with `options.UseSqlite(connectionString)` and run `dotnet ef migrations add InitialCreate`.

### Switching from In-Memory to SQLite (dev)

1. Add the EF Core SQLite package:
   ```bash
   dotnet add src/FinTrack.Server package Microsoft.EntityFrameworkCore.Sqlite
   ```
2. In `Program.cs`, replace:
   ```csharp
   options.UseInMemoryDatabase("FinTrackInMemoryDb")
   ```
   with:
   ```csharp
   options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
   ```
3. Add the connection string to `appsettings.Development.json`:
   ```json
   { "ConnectionStrings": { "DefaultConnection": "Data Source=fintrack.db" } }
   ```
4. Run migrations:
   ```bash
   cd src/FinTrack.Server
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

---

## 9. Deployment

The target deployment environment is **Azure App Service** (or equivalent).

### Server

1. Set the following **App Settings** (environment variables) on the App Service:
   - `Jwt__Secret` — a long, random production secret
   - `Jwt__Issuer` — `FinTrackServer`
   - `Jwt__Audience` — `FinTrackClient`
   - `ConnectionStrings__DefaultConnection` — SQL Server connection string
2. Ensure `UseInMemoryDatabase` is replaced with `UseSqlServer` for production builds.
3. Run EF migrations as part of the deployment pipeline:
   ```bash
   dotnet ef database update --connection "<prod connection string>"
   ```

### Client

The Blazor WASM client is a set of static files (HTML, CSS, JS, WASM). It can be served:
- Directly from the ASP.NET Core server (`app.UseBlazorFrameworkFiles()` / hosted model), or
- From a static host (Azure Static Web Apps, Netlify, etc.)

Update the API base URL in the client's `appsettings.json` to point at the deployed server URL before building.

### CI (GitHub Actions)

See `.github/workflows/` for the CI pipeline. On every pull request it:
- Restores packages
- Builds the solution
- Runs `dotnet test`

---

## 10. Reference Documents

| Document | Path |
|---|---|
| User Guide | [docs/user_guide.md](user_guide.md) |
| API Contract | [docs/api_contract.MD](api_contract.MD) |
| Data Model | [docs/data-model.md](data-model.md) |
| Test Plan | [docs/test_plan.md](test_plan.md) |
| Sprint Plan | [docs/sprint_plan.MD](sprint_plan.MD) |
| OpenAPI Spec | [docs/fintrack_openapi_spec.json](fintrack_openapi_spec.json) |
| Project Overview | [docs/reference/01_project_overview_and_spec.md](reference/01_project_overview_and_spec.md) |
| Backend Architecture | [docs/reference/02_backend_and_database_architecture.md](reference/02_backend_and_database_architecture.md) |
| Issues Matrix | [docs/reference/03_github_backend_issues_matrix.md](reference/03_github_backend_issues_matrix.md) |

---

## Change Log

| Date | Change | By |
|---|---|---|
| August 2026 (Week 2) | Initial draft | Anneta Dziedzom |
