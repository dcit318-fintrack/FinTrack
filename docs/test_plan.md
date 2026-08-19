# FinTrack Test Plan

**Issue:** #37  
**Owner:** Anneta, Matilda (QA pair)  
**Status:** Draft — written against the API contract (Day 1). Updated as features land.  
**Last updated:** Day 1

---

## 1. Scope

This plan covers:

- **API (integration tests)** — all endpoints defined in `docs/api_contract.MD`, run automatically in CI on every PR.
- **Manual testing** — all four screens of the Blazor client, run on the deployed build on Day 13.
- **Out of scope for v1:** performance/load testing, accessibility auditing, cross-browser matrix beyond Chrome.

The test project is `tests/FinTrack.Tests` (xUnit, targeting `net10.0`, referencing `FinTrack.Server`).

---

## 2. Test approach

| Layer | Type | When | Who |
|---|---|---|---|
| API validation rules | Integration (xUnit + `WebApplicationFactory`) | On every PR via CI | QA pair |
| Business logic | Unit (xUnit, pure functions) | As services are written | QA pair |
| Happy-path flows | Manual on deployed build | Day 13 | QA pair |
| Regression | Re-run CI + targeted manual | After every bug fix | QA pair |

**General rule (from the sprint plan):** don't wait for a complete build. Test whatever lands. Tests are marked `Skip` until their endpoint exists, then activated.

---

## 3. Test environment

| Item | Value |
|---|---|
| Framework | .NET 10, xUnit 2.9 |
| Test runner | `dotnet test` |
| Integration host | `WebApplicationFactory<Program>` (in-process) |
| Database | In-memory / SQLite for automated tests; real seeded DB for manual testing |
| Auth in tests | Generate a valid JWT in test setup; attach `Authorization: Bearer <token>` header |
| CI | GitHub Actions — see `.github/workflows/ci.yml` |
| **Currency** | **GHS (Ghanaian cedis) — fixed for v1. Resolves Open Question #5 from the API contract.** |

---

## 4. Auth tests (`POST /api/auth/*`)

### 4.1 Register — happy path
- **Given** a valid email, password, fullName  
- **Expect** `201 Created`, body contains `userId` (Guid), `email`, `fullName`, `accessToken`, `refreshToken`, `expiresAt`

### 4.2 Register — duplicate email
- **Given** the same email registered twice  
- **Expect** `400` or `409` with a `message` field (exact code to be confirmed by Backend on Day 1)

### 4.3 Register — missing required fields
| Field omitted | Expected |
|---|---|
| `email` | `400`, errors.email populated |
| `password` | `400`, errors.password populated |
| `fullName` | `400`, errors.fullName populated |

### 4.4 Register — invalid email format
- **Expect** `400`

### 4.5 Login — happy path
- **Given** correct credentials  
- **Expect** `200`, same shape as register response

### 4.6 Login — wrong password
- **Expect** `401`, message is `"Invalid email or password"` (must not say which field was wrong)

### 4.7 Login — unknown email
- **Expect** `401`, same generic message (must not leak whether the account exists)

### 4.8 Refresh token — valid token
- **Expect** `200`, new `accessToken`, `refreshToken`, `expiresAt`

### 4.9 Refresh token — expired / invalid token
- **Expect** `401`

### 4.10 Protected endpoint — no token
- **Given** a request to any protected endpoint with no `Authorization` header  
- **Expect** `401`

### 4.11 Protected endpoint — malformed token
- **Given** `Authorization: Bearer notavalidjwt`  
- **Expect** `401`

---

## 5. Categories tests (`GET /api/categories`)

### 5.1 Get all categories — authenticated
- **Expect** `200`, array of objects each with `id` (Guid), `name` (string), `type` (`"Income"` or `"Expense"`)
- **Expect** the seven seeded categories are present: Food, Transport, Rent, Utilities, Entertainment, Salary, Other

### 5.2 Get all categories — unauthenticated
- **Expect** `401`

### 5.3 Category types
- **Expect** each category's `type` is exactly `"Income"` or `"Expense"` — no other value accepted

---

## 6. Transactions tests (`/api/transactions`)

### 6.1 POST — happy path (Expense)
- **Given** valid `amount`, `type: "Expense"`, valid `categoryId`, `description`, past/today `date`  
- **Expect** `201`, `Location` header, body is the created transaction including `categoryName`

### 6.2 POST — happy path (Income)
- Same as above with `type: "Income"`

### 6.3 POST — validation failures

| Scenario | Field | Expected |
|---|---|---|
| `amount = 0.00` | amount | `400`, errors.amount |
| `amount = -5.00` | amount | `400`, errors.amount |
| `type` omitted | type | `400`, errors.type |
| `type = "Cash"` (invalid) | type | `400`, errors.type |
| `categoryId` not a valid Guid | categoryId | `400` |
| `categoryId` valid Guid but doesn't exist | categoryId | `400` |
| `description` = 201 characters | description | `400`, errors.description |
| `description` = 200 characters | description | `201` (boundary — must pass) |
| `date` set to tomorrow | date | `400`, errors.date |
| `date` set to today | date | `201` (boundary — must pass) |

### 6.4 GET list — default pagination
- **Expect** `200`, `page: 1`, `pageSize: 25`, `totalCount` >= 0, `items` array

### 6.5 GET list — filter by `from` and `to`
- Create two transactions on different dates; filter so only one is in range  
- **Expect** `items` contains only the in-range transaction

### 6.6 GET list — filter by `categoryId`
- **Expect** all returned items have the specified `categoryId`

### 6.7 GET list — filter by `type`
- **Expect** all returned items have the specified `type`

### 6.8 GET list — pagination
- Create 30 transactions; request `page=2&pageSize=10`  
- **Expect** 10 items, correct `page` and `pageSize` in response, `totalCount = 30`

### 6.9 GET single — exists
- **Expect** `200`, correct transaction object

### 6.10 GET single — not found
- **Expect** `404`

### 6.11 GET single — belongs to another user
- User A creates a transaction; User B requests it by ID  
- **Expect** `404` (not `403` — must not leak existence per the API contract)

### 6.12 PUT — happy path
- **Expect** `200`, updated fields reflected in response

### 6.13 PUT — not found
- **Expect** `404`

### 6.14 PUT — belongs to another user
- **Expect** `404`

### 6.15 PUT — same validation rules as POST apply

### 6.16 DELETE — happy path
- **Expect** `204`

### 6.17 DELETE — not found
- **Expect** `404`

### 6.18 DELETE — belongs to another user
- **Expect** `404`

---

## 7. Budgets tests (`/api/budgets`)

### 7.1 POST — happy path
- **Given** valid `categoryId`, `limit > 0`, valid `month` (e.g. `"2026-08"`)  
- **Expect** `201`

### 7.2 POST — duplicate category + month
- Create the same budget twice  
- **Expect** `409 Conflict`

### 7.3 POST — validation failures

| Scenario | Expected |
|---|---|
| `limit = 0` | `400` |
| `limit = -100` | `400` |
| `categoryId` doesn't exist | `400` |
| `month` malformed (e.g. `"08-2026"`) | `400` |

### 7.4 GET budgets — default to current month
- **Expect** `200`, array; each item has `id`, `categoryId`, `categoryName`, `limit`, `spent`, `remaining`, `month`
- Verify `spent + remaining == limit` for each item (GHS, 2 decimal places)

### 7.5 GET budgets — `spent` reflects real transactions
- Create a budget for Food (`limit: 800.00` GHS); add a Food expense of `250.00` GHS  
- **Expect** `spent: 250.00`, `remaining: 550.00`

### 7.6 GET budgets — filter by `month`
- **Expect** only budgets for the requested month are returned

### 7.7 PUT — update limit
- **Expect** `200`, `limit` changed

### 7.8 PUT — belongs to another user
- **Expect** `404`

### 7.9 DELETE — happy path
- **Expect** `204`

### 7.10 DELETE — belongs to another user
- **Expect** `404`

---

## 8. Savings goals tests (`/api/savings-goals`)

### 8.1 POST — happy path
- **Given** `name: "Laptop"`, `targetAmount: 5000.00` (GHS), `targetDate` in the future  
- **Expect** `201`, body with `id`, `name`, `targetAmount`, `currentAmount: 0.00`, `progressPercent: 0.0`, `targetDate`, `isAchieved: false`

### 8.2 POST — validation failures

| Scenario | Expected |
|---|---|
| `name` = 101 characters | `400` |
| `name` = 100 characters | `201` (boundary) |
| `targetAmount = 0.00` | `400` |
| `targetAmount = -1.00` | `400` |
| `targetDate` = yesterday | `400` |
| `targetDate` = today | `400` (must be strictly in the future, not today) |
| `targetDate` = tomorrow | `201` |

### 8.3 GET — list
- **Expect** `200`, array with correct fields

### 8.4 POST contribute — happy path
- **Given** goal with `targetAmount: 5000.00` GHS; contribute `amount: 1000.00`  
- **Expect** `200`, `currentAmount: 1000.00`, `progressPercent: 20.0`

### 8.5 POST contribute — amount <= 0
- **Expect** `400`

### 8.6 POST contribute — goal achieved
- Goal `targetAmount: 2000.00` GHS; contribute `2000.00`  
- **Expect** `isAchieved: true`, `progressPercent: 100.0`

### 8.7 POST contribute — goal not found
- **Expect** `404`

### 8.8 POST contribute — belongs to another user
- **Expect** `404`

### 8.9 PUT — update goal
- **Expect** `200`

### 8.10 DELETE — happy path
- **Expect** `204`

---

## 9. Dashboard tests (`GET /api/dashboard`)

### 9.1 Happy path
- **Expect** `200`, body with `totalIncome`, `totalExpenses`, `balance`, `month`, `recentTransactions` (<=5 items), `budgetsAtRisk`

### 9.2 Balance calculation
- Create income = `3200.00` GHS (e.g. monthly salary), expenses = `1800.00` GHS  
- **Expect** `balance: 1400.00`, `totalIncome: 3200.00`, `totalExpenses: 1800.00`

### 9.3 `recentTransactions` — count capped at 5
- Add 10 transactions  
- **Expect** `recentTransactions` has exactly 5 items

### 9.4 `budgetsAtRisk` — threshold is 80%
- Budget `limit: 800.00` GHS, `spent: 640.00` (80%) → **must be included**
- Budget `limit: 800.00` GHS, `spent: 639.00` (79.9%) → **must not be included**
- Budget `limit: 800.00` GHS, `spent: 800.00` (100%) → **must be included**

### 9.5 `budgetsAtRisk` shape
- **Expect** each item has `categoryName`, `limit`, `spent`, `percentUsed`

### 9.6 Filter by `month` param
- **Expect** only data for the specified month is returned

---

## 10. Reports tests

### 10.1 Spending by category — happy path
- **Given** `from` and `to` both provided  
- **Expect** `200`, `from`, `to`, `totalSpent`, `categories` array

### 10.2 Spending by category — sorted by amount descending
- Add expenses: Food=`620.00` GHS, Transport=`180.00` GHS, Entertainment=`350.00` GHS  
- **Expect** order: Food (620.00), Entertainment (350.00), Transport (180.00)

### 10.3 Spending by category — `percentOfTotal` adds up
- Verify all category percents sum to 100 (within 0.1% tolerance for rounding)

### 10.4 Spending by category — missing required params
- **Given** only `from` or only `to` (not both)  
- **Expect** `400`

### 10.5 Income vs expense — happy path (monthly granularity)
- **Expect** `200`, `granularity: "monthly"`, `points` array with `period`, `income`, `expense`, `net`

### 10.6 Income vs expense — granularity options
| `granularity` value | Expected |
|---|---|
| `daily` | `200` |
| `weekly` | `200` |
| `monthly` (default/omitted) | `200` |
| `annual` (invalid) | `400` |

### 10.7 Income vs expense — zero-filled periods
- A month with no transactions must still appear in `points` with `income: 0`, `expense: 0`, `net: 0`
- This is explicitly required by the contract: *"otherwise the chart has gaps"*

### 10.8 Income vs expense — `net` calculation
- **Expect** `net == income - expense` for every point

---

## 11. Cross-cutting / security checks

| Check | What to verify |
|---|---|
| Data isolation | User A cannot see User B's transactions, budgets, or goals — gets `404` |
| No existence leaking | Accessing another user's resource ID must return `404`, never `403` |
| Money is decimal | No float drift: amounts must be exact to 2 decimal places |
| Dates are UTC ISO 8601 | All date fields match `^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$` |
| IDs are Guids | All `id` fields are valid Guid strings |
| Error shape | All `4xx` responses have at least a `message` field; `errors` only present when field-level problems exist |
| No 500s | No unhandled exceptions — the API should never return `500` for bad input |

---

## 12. Manual test checklist (Day 13)

Run against the deployed build. One pass covers all four screens.

### Dashboard screen
- [ ] Shows correct month's income, expenses, balance
- [ ] Recent transactions list shows (max 5)
- [ ] Budgets at risk section shows categories >= 80% used
- [ ] Changing month updates all figures

### Transactions screen
- [ ] List loads and paginates
- [ ] Filter by date range works
- [ ] Filter by category works
- [ ] Add transaction — form validates and saves
- [ ] Edit transaction — pre-fills form, saves changes
- [ ] Delete transaction — removes from list

### Budgets screen
- [ ] List shows for current month with spent/remaining
- [ ] Add budget — validates and saves
- [ ] Duplicate budget shows a clear error
- [ ] Edit budget limit — updates correctly
- [ ] Delete budget

### Reports screen
- [ ] Spending by category chart renders
- [ ] Income vs expense chart renders
- [ ] Date range picker works

### Auth flows
- [ ] Register new user — redirected to dashboard
- [ ] Login with correct credentials
- [ ] Login with wrong password — correct generic error shown (not which field was wrong)
- [ ] Expired/missing token — redirected to login, not a broken screen

---

## 13. Bug reporting

Log bugs as GitHub issues with label `bug`:

- **Title:** `[BUG] <short description>`
- **Body:** Steps to reproduce / Expected / Actual / Response body or screenshot

**Severity tags:** `critical` (blocks demo), `major` (feature broken), `minor` (cosmetic or edge case).

---

## Change log

| Day | Change | By |
|---|---|---|
| 1 | Initial test plan written from the API contract | Anneta |
