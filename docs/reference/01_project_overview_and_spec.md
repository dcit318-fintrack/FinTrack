# Project Overview & Requirements Specification — FinTrack

> **Source Documents:** `DCIT318_Project_Proposal.docx` and `Fintrack_Team_Requirement_Specifications.pdf`  
> **Last Updated:** August 2026 (.NET 10 SDK standard)

---

## 1. Executive Summary

**Fintrack** is a personal finance management web application designed primarily for students, young workers, and individuals seeking clear visibility into their personal finances. The platform empowers users to track income and expenses, monitor overall account balances, create category-based monthly budgets, set and track savings goals, and analyze spending patterns using interactive visual charts.

---

## 2. Core Stack & Technical Architecture

* **Primary Category:** Web Applications & Services (Pillar 1)
* **Frontend:** Blazor WebAssembly (Interactive Client UI)
* **Backend:** ASP.NET Core Web API (.NET 10)
* **Data Access / ORM:** Entity Framework Core (EF Core 10)
* **Database Engine:** SQLite (Local Dev / Cross-Platform Testing) & SQL Server (Production)
* **Authentication & Authorization:** ASP.NET Core Identity + JWT (JSON Web Tokens) with Refresh Tokens
* **Charting & Visualization:** Chart.js / Blazor Charting Components
* **API Documentation:** OpenAPI / Swagger / Scalar (.NET 10 native `Microsoft.AspNetCore.OpenApi`)

---

## 3. Core Screens & Functionalities

The application revolves around four core client screens fed by the backend API:

### 1. Dashboard Screen
* **Overview:** High-level summary of financial standing for the current or selected month.
* **Key Components:**
  * Total Income, Total Expenses, and Net Balance cards.
  * Recent Transactions list (up to 5 latest items).
  * Budgets at Risk indicator (categories reaching ≥ 80% of limit).

### 2. Transactions Screen
* **Overview:** Complete management of financial records.
* **Key Components:**
  * Paginated list / tabular view of transactions with date, category, type (Income/Expense), description, and amount.
  * Modal/Form to **Add**, **Edit**, and **Delete** transactions.
  * Filtering by date range (`from`, `to`), category, and type.

### 3. Budget & Savings Screen
* **Overview:** Planning and goal-tracking interface.
* **Key Components:**
  * Category spending limits for specified months.
  * Spent vs. Remaining progress bars per category.
  * Savings Goals tracker showing target amount, current saved amount, progress percentage, target date, and an interactive contribution action.

### 4. Reports Screen
* **Overview:** Visual analytics and financial breakdown over selected time periods.
* **Key Components:**
  * **Spending by Category:** Doughnut/Pie chart breaking down total expenses by category with percentages.
  * **Income vs. Expense:** Bar/Line trend chart comparing income, expenses, and net savings over customizable granularities (daily, weekly, monthly).

---

## 4. Team Structure & Role Allocations

| Role | Members | Student IDs | Key Responsibilities |
|---|---|---|---|
| **Project Managers** | Thomas Theophilus Kwasi Kutin<br>Lambert Klenam Nenemse | 22045470<br>22165472 | Backlog management, API contract locking, Daily standups, Sprint tracking, Scope management, Demo rehearsal. |
| **UI/UX Designers** | Gerald Nii Lantey Lamptey<br>Laudina Saifah | 22044115<br>22045087 | Wireframes, Hi-fi Figma mockups, Design system (colors, typography, components), Chart mockups, Usability reviews. |
| **Backend Developers** | Godsway Ewoenam Kwadzo Ahortor<br>Enoch Kwabena Boampong Amankwah | 22044184<br>22232235 | Web API scaffolding, JWT & ASP.NET Core Identity authentication, Transactions CRUD, Budgets & Savings CRUD, Reports aggregation endpoints, Error handling. |
| **Database & Integration** | Michael Asante-Arhin<br>Akyeampong Papa Kwadwo Ankama | 22241078<br>22060947 | Schema design, EF Core DbContext & Migrations, Data seeding, Aggregation query optimization, Data integrity & cascading rules. |
| **Frontend Developers** | Samuel Watson Tettey<br>Ansah Louis Kwaku Ashwere | 22237616<br>22241242 | Blazor WASM routing & layouts, 4 core screens UI, JWT auth wiring, Chart integration, Live API consumption. |
| **QA, Docs & Deployment** | Anneta Dziedzom Dzibolosu<br>Ashalley Matilda Adey | 22130388<br>22069103 | Test plan & execution, CI build setup (GitHub Actions), Technical & user documentation, Cloud deployment (Azure App Service / similar). |

---

## 5. Non-Negotiable Success Criteria & Scope Management

* **Must-Have Core Functionalities:**
  1. Identity & JWT Authentication (Registration, Login, Refresh).
  2. Transaction CRUD & Account Balance calculations.
  3. Budget spending limits & tracking.
  4. At least one working interactive report chart.
* **Scope Cut Priorities (If time is constrained):**
  1. Savings Goals (if Budgets cover category limits).
  2. Report granularity options (fallback to monthly aggregation only).
  3. Transaction pagination (fallback to flat sorted list).
  4. Transaction editing (Add and Delete suffice for minimal demo).
