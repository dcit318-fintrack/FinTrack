// This file is intentionally left empty.
// Test coverage has been organized into feature-specific files:
//
//   Auth/AuthTests.cs          — register, login, refresh, token protection
//   Transactions/TransactionTests.cs — CRUD, validation, filtering, pagination
//   Budgets/BudgetTests.cs     — CRUD, 409 duplicate, spent/remaining
//   SavingsGoals/SavingsGoalTests.cs — CRUD, contribute, isAchieved
//   Dashboard/DashboardTests.cs — balance, at-risk budgets, recent transactions
//   Reports/ReportsTests.cs    — spending-by-category, income-vs-expense
//
// All tests are marked [Skip] until their backend endpoints are implemented.
// To activate a test: remove the Skip argument from [Fact(Skip = "...")] or [Theory(Skip = "...")].

namespace FinTrack.Tests;
