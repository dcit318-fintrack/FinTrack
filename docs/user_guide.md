# FinTrack — User Guide

**Version:** 1.0 (Week 2 draft)  
**Audience:** End users of the FinTrack web application  
**Last updated:** August 2026

---

## What is FinTrack?

FinTrack is a personal finance tracker designed for students and young workers. It helps you:

- See exactly where your money is going each month
- Stay within spending limits you set per category
- Work toward savings goals step by step
- Understand your income vs. expense trends through charts

All your data is private — no one else can see your transactions, budgets, or goals.

---

## Getting Started

### Creating an Account

1. Open the FinTrack app in your browser.
2. Click **Register** on the login screen.
3. Enter your **full name**, **email address**, and a **password** (minimum 6 characters).
4. Click **Create Account**.
5. You will be taken directly to your **Dashboard**.

### Logging In

1. Go to the FinTrack login page.
2. Enter your registered **email** and **password**.
3. Click **Log In**.

> **Forgot your password?** Password reset is not available in v1. Contact your administrator if you are locked out.

### Logging Out

Click the user icon or your name in the header and select **Log Out**. Your session ends immediately and you are returned to the login page.

---

## The Dashboard

The Dashboard is your home screen. It gives you a snapshot of your current month's finances at a glance.

### What you'll see

| Section | What it shows |
|---|---|
| **Total Income** | All money you received this month |
| **Total Expenses** | All money you spent this month |
| **Balance** | Income minus Expenses |
| **Recent Transactions** | Your 5 most recent records |
| **Budgets at Risk** | Categories where you've used 80% or more of your monthly limit |

### Changing the month

Use the **month selector** at the top of the Dashboard to view a different month's summary. The cards, recent transactions, and risk indicators all update to match the selected month.

---

## Transactions

The Transactions screen is where you record every financial event — money in (income) or money out (expense).

### Viewing your transactions

- Transactions are listed in a paginated table, most recent first.
- Each row shows the **date**, **category**, **type** (Income / Expense), **description**, and **amount**.
- The list shows 25 transactions per page by default.

### Filtering

Use the filter controls above the list to narrow down what you see:

| Filter | How to use |
|---|---|
| **Date range** | Enter a "From" date and a "To" date |
| **Category** | Pick a category from the dropdown |
| **Type** | Choose Income, Expense, or leave blank for both |

Click **Apply** to update the list. Click **Clear** to remove all filters.

### Adding a Transaction

1. Click **+ Add Transaction**.
2. Fill in the form:
   - **Amount** — must be greater than 0 (in GHS)
   - **Type** — Income or Expense
   - **Category** — choose from the list (Food, Transport, Rent, Utilities, Entertainment, Salary, Other)
   - **Description** — a short note (max 200 characters)
   - **Date** — today or any past date (future dates are not allowed)
3. Click **Save**.

The new transaction appears at the top of your list.

### Editing a Transaction

1. Find the transaction in the list.
2. Click the **Edit** (pencil) icon on that row.
3. Update the fields you want to change.
4. Click **Save**.

### Deleting a Transaction

1. Find the transaction in the list.
2. Click the **Delete** (bin) icon on that row.
3. Confirm the deletion in the prompt.

> **Note:** Deleting a transaction immediately recalculates your budget spending totals and dashboard balance.

---

## Budgets

A budget sets a monthly spending **limit** for one category. For example: "I want to spend no more than GHS 500 on Food in August."

### Viewing your budgets

Open the **Budget & Savings** screen. The budgets section shows a card per category with:

- **Category name** and the month
- **Limit** — the cap you set
- **Spent** — how much you've spent in that category this month (calculated from your transactions)
- **Remaining** — how much is left
- A **progress bar** that turns amber near 80% and red at 100%

### Adding a Budget

1. Click **+ Add Budget**.
2. Select the **Category** and the **Month** (format: YYYY-MM, e.g. `2026-08`).
3. Enter a **Limit** in GHS (must be greater than 0).
4. Click **Save**.

> You can only have **one budget per category per month**. If you try to create a duplicate, you'll see a conflict error.

### Editing a Budget Limit

1. Click the **Edit** icon on the budget card.
2. Enter the new **Limit**.
3. Click **Save**.

### Deleting a Budget

Click the **Delete** icon on the budget card and confirm. This removes the limit but does not delete any transactions.

---

## Savings Goals

A savings goal tracks money you are setting aside for a specific purpose, like buying a laptop or going on a trip.

### Viewing your goals

Scroll to the **Savings Goals** section of the Budget & Savings screen. Each goal card shows:

- Goal **name**
- **Target amount** (GHS) and **current amount saved**
- **Progress bar** and **percentage** towards the target
- **Target date** — when you aim to hit the goal
- A ✅ badge if the goal has been **achieved**

### Creating a Savings Goal

1. Click **+ New Goal**.
2. Fill in:
   - **Name** — e.g. "New Laptop" (max 100 characters)
   - **Target Amount** — in GHS (must be greater than 0)
   - **Target Date** — must be a future date (tomorrow or later)
3. Click **Save**.

### Making a Contribution

1. On the goal card, click **Contribute**.
2. Enter the **amount** you are adding (must be greater than 0).
3. Click **Add**.

The current amount and progress bar update immediately. When the current amount reaches the target, the goal is automatically marked as **Achieved**.

### Editing a Goal

Click the **Edit** icon on the goal card to update the name, target amount, or target date.

### Deleting a Goal

Click the **Delete** icon on the goal card and confirm. This only removes the goal record — it has no effect on your transactions.

---

## Reports

The Reports screen gives you a visual breakdown of your finances over any date range you choose.

### Spending by Category (Doughnut Chart)

Shows what percentage of your total spending went to each category in the selected period.

1. Set the **From** and **To** dates using the date pickers.
2. The doughnut chart and table update to show each category's total amount and its share of total spending.
3. Categories are listed from highest to lowest spend.

### Income vs. Expense (Bar / Line Chart)

Compares your income, expenses, and net savings over time.

1. Set the **From** and **To** dates.
2. Choose a **Granularity**:
   - **Daily** — one data point per day
   - **Weekly** — one data point per week
   - **Monthly** — one data point per month (default)
3. The chart plots income (green), expense (red), and net (blue) for each period. Periods with no activity show as zero — this is intentional and keeps the chart continuous.

---

## Frequently Asked Questions

**Q: What currency does FinTrack use?**  
A: All amounts are in **Ghanaian Cedis (GHS)** for v1. There is no option to switch currency.

**Q: Can I add a transaction for a future date?**  
A: No. Future-dated transactions are blocked to keep your records accurate.

**Q: What happens if I delete a transaction that was inside a budget period?**  
A: The budget's "Spent" amount recalculates automatically — your remaining budget goes back up.

**Q: My contribution pushed a savings goal to 100% — why isn't it marked achieved?**  
A: Achieved status is set when `currentAmount >= targetAmount`. If it still shows as not achieved, try refreshing the page.

**Q: Can another user see my data?**  
A: No. All data is strictly isolated per account. Accessing a resource that doesn't belong to you returns "Not Found".

---

## Getting Help

Report bugs or issues by creating a GitHub issue with the label `bug` and the following format:

- **Title:** `[BUG] Short description`
- **Body:** Steps to reproduce / Expected behaviour / Actual behaviour / Screenshot (if applicable)
