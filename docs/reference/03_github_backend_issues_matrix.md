# GitHub Issues & Task Assignment Matrix — FinTrack (Backend & Database)

> **Sprint Duration:** 14 Days  
> **Backend Leads:** Godsway Ewoenam Kwadzo Ahortor & Enoch Kwabena Boampong Amankwah  
> **Database Leads:** Michael Asante-Arhin & Akyeampong Papa Kwadwo Ankama

---

## 1. Backend Role Issues (#16 – #23)

| Issue # | Title | Target Days | Dependencies | Key Technical Deliverables |
|---|---|---|---|---|
| **#16** | Scaffold Web API structure (controllers, services, DTOs) | Days 1–2 | None | Layered folder architecture in `FinTrack.Server` & `FinTrack.Shared`, Service interfaces, Controllers, Dependency Injection registrations. |
| **#17** | Implement ASP.NET Core Identity + JWT auth | Days 3–5 | #16, #31, #32 | Registration endpoint (`/api/auth/register`), Login endpoint (`/api/auth/login`), Refresh endpoint (`/api/auth/refresh`), JWT Bearer middleware configuration, Password hashing via Identity. |
| **#18** | Build Transactions CRUD endpoints | Days 3–5 | #16, #32, #33 | Endpoints for GET, GET by ID, POST, PUT, DELETE transactions. Filter by date range, category, type. User data isolation. |
| **#19** | Build Budgets and Savings Goals CRUD endpoints | Days 6–7 | #16, #32, #33 | Endpoints for Budgets (with server-computed `spent` and `remaining`) and Savings Goals (with contribution action `/contribute`). |
| **#20** | Agree request/response DTO shapes with Frontend | Days 1–2 | #4 | Finalize DTO contracts in `FinTrack.Shared` to unblock Blazor WASM client mocking. |
| **#21** | Build Reports aggregation endpoints | Days 8–10 | #34 | Implement `/api/reports/spending-by-category` (category percentage breakdown) and `/api/reports/income-vs-expense` (time-series comparison with zero-gap handling). |
| **#22** | Add validation, error handling, and consistent API response format | Days 8–10 | #17–#21 | Global exception handling middleware, Fluent Validation / DataAnnotations, standardized `{ message, errors }` envelope for 400/404/401/500 status codes. |
| **#23** | Support integration testing and fix hookup bugs | Days 11–12 | #16–#22, Frontend | End-to-end integration debugging with Blazor WASM, CORS policies, auth token header handling, payload fixups. |

---

## 2. Database & Integration Role Issues (#31 – #36)

| Issue # | Title | Target Days | Dependencies | Key Technical Deliverables |
|---|---|---|---|---|
| **#31** | Design database schema | Days 1–2 | None | Database diagram, Entity definitions for ApplicationUser, Category, Transaction, Budget, SavingsGoal. |
| **#32** | Set up EF Core DbContext, migrations, and seed data | Days 3–5 | #31 | `FinTrackDbContext`, Initial Migration, Seed data for default Categories (`Food`, `Transport`, `Rent`, `Utilities`, `Entertainment`, `Salary`, `Other`). |
| **#33** | Finalize entity models against API DTOs | Days 3–5 | #20, #31 | Fluent API mapping, entity configuration, indexes on `UserId`, `CategoryId`, and `Date`. |
| **#34** | Optimize queries for Reports aggregations | Days 8–10 | #32, #33 | LINQ grouping queries, projection optimizations (`Select`), avoidance of N+1 query problems. |
| **#35** | Handle data integrity rules | Days 8–10 | #32 | Cascading deletes, budget-vs-transaction consistency checks, unique composite index on Budget (`UserId`, `CategoryId`, `Month`). |
| **#36** | Support integration debugging (DB side) | Days 11–12 | #32–#35 | Database connection string troubleshooting, migration verifications, query execution profiling. |
