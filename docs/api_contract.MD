# FinTrack API contract

**Status:** draft — seeded by PM, to be finalised by Backend and Frontend on Day 1.

This document is the agreement between `FinTrack.Server` and `FinTrack.Client`. Frontend builds against these shapes with mocked data before the API exists; Backend implements to match. Shared DTOs live in `FinTrack.Shared` so both sides compile against the same types.

**Rule:** if you need to change a shape here, say so in the standup before you change it. A silent change breaks the other pair.

Base URL: `/api`
All endpoints except `/auth/register` and `/auth/login` require `Authorization: Bearer <token>`.

---

## Conventions

**Money** — `decimal`, two places. Never `float`.

**Dates** — ISO 8601 UTC (`2026-08-07T14:30:00Z`).

**IDs** — `Guid`.

**Success responses** — the resource directly, no envelope.

**Error responses** — always this shape:

```json
{
  "message": "Human-readable summary",
  "errors": {
    "amount": ["Amount must be greater than zero"]
  }
}
```

`errors` is omitted when there are no field-level problems.

**Status codes** — 200 OK, 201 Created (with `Location`), 204 No Content on delete, 400 validation, 401 missing/invalid token, 403 resource belongs to another user, 404 not found.

**Scoping** — every endpoint operates only on the authenticated user's data. A user requesting another user's transaction gets 404, not 403 (don't leak existence).

---

## 1. Auth

### POST /api/auth/register

Request:
```json
{
  "email": "user@example.com",
  "password": "string",
  "fullName": "string"
}
```

Response `201`:
```json
{
  "userId": "guid",
  "email": "user@example.com",
  "fullName": "string",
  "accessToken": "jwt",
  "refreshToken": "string",
  "expiresAt": "2026-08-07T15:30:00Z"
}
```

### POST /api/auth/login

Request: `{ "email": "string", "password": "string" }`
Response `200`: same shape as register.
Failure `401`: `{ "message": "Invalid email or password" }` — never say which one was wrong.

### POST /api/auth/refresh

Request: `{ "refreshToken": "string" }`
Response `200`: new `accessToken`, `refreshToken`, `expiresAt`.

---

## 2. Categories

Seeded by the DB pair (Food, Transport, Rent, Utilities, Entertainment, Salary, Other). Read-only for v1.

### GET /api/categories

Response `200`:
```json
[
  { "id": "guid", "name": "Food", "type": "Expense" }
]
```

`type` is `"Income"` or `"Expense"`.

---

## 3. Transactions

### GET /api/transactions

Query params (all optional): `from`, `to` (dates), `categoryId`, `type`, `page` (default 1), `pageSize` (default 25).

Response `200`:
```json
{
  "items": [
    {
      "id": "guid",
      "amount": 45.50,
      "type": "Expense",
      "categoryId": "guid",
      "categoryName": "Food",
      "description": "Groceries",
      "date": "2026-08-05T00:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 25,
  "totalCount": 132
}
```

`categoryName` is denormalised deliberately — it saves the client a second call to render the list.

### GET /api/transactions/{id}
Response `200`: a single transaction object as above.

### POST /api/transactions

Request:
```json
{
  "amount": 45.50,
  "type": "Expense",
  "categoryId": "guid",
  "description": "Groceries",
  "date": "2026-08-05T00:00:00Z"
}
```

Response `201`: the created transaction.
Validation: `amount` > 0, `type` required, `categoryId` must exist, `description` max 200 chars, `date` not in the future.

### PUT /api/transactions/{id}
Request: same as POST. Response `200`: updated transaction.

### DELETE /api/transactions/{id}
Response `204`.

---

## 4. Budgets

A budget is a spending limit for one category in one month.

### GET /api/budgets

Query params: `month` (`2026-08`, defaults to current).

Response `200`:
```json
[
  {
    "id": "guid",
    "categoryId": "guid",
    "categoryName": "Food",
    "limit": 500.00,
    "spent": 312.75,
    "remaining": 187.25,
    "month": "2026-08"
  }
]
```

`spent` and `remaining` are computed server-side from transactions. The client does not calculate these.

### POST /api/budgets
Request: `{ "categoryId": "guid", "limit": 500.00, "month": "2026-08" }`
Response `201`. Validation: `limit` > 0; one budget per category per month (409 on duplicate).

### PUT /api/budgets/{id}
Request: `{ "limit": 600.00 }`
Response `200`.

### DELETE /api/budgets/{id}
Response `204`.

---

## 5. Savings goals

### GET /api/savings-goals

Response `200`:
```json
[
  {
    "id": "guid",
    "name": "Laptop",
    "targetAmount": 5000.00,
    "currentAmount": 1750.00,
    "progressPercent": 35.0,
    "targetDate": "2026-12-31T00:00:00Z",
    "isAchieved": false
  }
]
```

### POST /api/savings-goals
Request: `{ "name": "string", "targetAmount": 5000.00, "targetDate": "2026-12-31T00:00:00Z" }`
Response `201`. Validation: `name` max 100 chars, `targetAmount` > 0, `targetDate` in the future.

### PUT /api/savings-goals/{id}
Request: same as POST, plus optional `currentAmount`.
Response `200`.

### POST /api/savings-goals/{id}/contribute
Request: `{ "amount": 250.00 }`
Response `200`: the updated goal.

Separate from PUT so the client doesn't have to read-modify-write to add a contribution.

### DELETE /api/savings-goals/{id}
Response `204`.

---

## 6. Dashboard

One call for the whole Dashboard screen, so it doesn't need four round trips.

### GET /api/dashboard

Query params: `month` (defaults to current).

Response `200`:
```json
{
  "totalIncome": 3200.00,
  "totalExpenses": 1845.25,
  "balance": 1354.75,
  "month": "2026-08",
  "recentTransactions": [ ... up to 5 transaction objects ... ],
  "budgetsAtRisk": [
    { "categoryName": "Food", "limit": 500.00, "spent": 480.00, "percentUsed": 96.0 }
  ]
}
```

`budgetsAtRisk` = budgets at 80% or more of their limit.

---

## 7. Reports

Both endpoints are aggregations. Coordinate with the DB pair on query efficiency — these are the slowest calls in the app.

### GET /api/reports/spending-by-category

Query params: `from`, `to` (both required).

Response `200`:
```json
{
  "from": "2026-07-01T00:00:00Z",
  "to": "2026-07-31T00:00:00Z",
  "totalSpent": 1845.25,
  "categories": [
    {
      "categoryId": "guid",
      "categoryName": "Food",
      "amount": 620.50,
      "percentOfTotal": 33.6
    }
  ]
}
```

Sorted by `amount` descending. Feeds the pie/doughnut chart.

### GET /api/reports/income-vs-expense

Query params: `from`, `to`, `granularity` (`daily` | `weekly` | `monthly`, default `monthly`).

Response `200`:
```json
{
  "granularity": "monthly",
  "points": [
    {
      "period": "2026-07",
      "income": 3200.00,
      "expense": 1845.25,
      "net": 1354.75
    }
  ]
}
```

Periods with no transactions are still returned with zeros — otherwise the chart has gaps where the client can't tell missing from zero.

---

## Open questions for Day 1

Backend and Frontend settle these together and update this doc:

1. Should transactions support attachments or receipts? (Suggest: no, out of scope for v1.)
2. Access token lifetime — 15 min with refresh, or longer for demo simplicity?
3. Does `POST /api/savings-goals/{id}/contribute` create a corresponding transaction, or is savings tracked separately? This affects the schema.
4. Pagination on transactions — is it needed for the demo, or is a flat list fine given seed data volume?
5. Currency — is it fixed to GHS, or stored per user?

---

## Change log

| Date | Change | By |
|------|--------|-----|
| Day 0 | Initial draft seeded from the requirement spec | PM |