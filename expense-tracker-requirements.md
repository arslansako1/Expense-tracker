# Project Requirements: Personal Finance & Expense Tracker

**Stack:** ASP.NET Core Web API (.NET), Entity Framework Core, React (frontend), SQL Server LocalDB or SQLite
**Deployment:** Local only — no cloud hosting required. Everything should run via `dotnet run` / `npm start` on localhost.

---

## 1. Overview

You are building a personal finance tracker that lets a user manage multiple accounts (checking, savings, credit card, cash, etc.), record transactions and transfers between accounts, set budgets, view reports, and import bank statements.

This is not just a CRUD app. Money-handling systems have real correctness requirements — balances must always be accurate, history must never silently change, and concurrent edits must not corrupt data. Treat this project as if a real person's financial records depended on it, because in production-grade systems, they would.

Read through the whole document before writing code. Several requirements interact with each other (e.g., transfers + audit trail + concurrency).

---

## 2. Core Domain Requirements

### 2.1 Accounts
- A user can create multiple accounts (e.g., "Checking", "Savings", "Visa Credit Card").
- Each account has: name, type (Checking / Savings / Credit / Cash / Other), currency, current balance, created date.
- Account balance must **always** be derivable from its transaction history — do not treat "balance" as a value you freely overwrite. (Think about *how* you'll keep a stored balance field, if you use one, consistent with the ledger.)

### 2.2 Transactions
- A transaction has: amount, date, category, description, account, and type (Income / Expense).
- **All monetary values must use `decimal`, never `float` or `double`.** Explain in your README why this matters.
- Transactions must never be hard-deleted. "Deleting" a transaction should create a reversing entry or a soft-delete flag, and the original must remain visible in history/audit views.

### 2.3 Transfers Between Accounts
- A user can transfer money from one account to another (e.g., Checking → Savings).
- A transfer is **not** a single transaction row. It must be modeled as two linked entries (a debit and a credit) that always net to zero, and both must be created atomically — if one fails, neither should persist.
- Think about what happens if the two accounts have different currencies.

### 2.4 Categories
- Support user-defined categories (e.g., Groceries, Rent, Dining, Utilities) for expenses and income.
- A transaction belongs to exactly one category.

### 2.5 Recurring Transactions
- A user can define a recurring rule (e.g., "Rent, $1200, Checking account, every 1st of the month").
- The system must be able to show projected upcoming transactions (e.g., "next 3 months") **without** pre-inserting rows for every future occurrence into the database.
- When a recurring transaction's scheduled date arrives, it should generate a real transaction automatically (a background process, not something the user has to trigger manually).

### 2.6 Budgets & Alerts
- A user can set a monthly budget per category (e.g., "$500/month on Dining").
- The system must track actual spend against budget for the current month.
- When a budget is exceeded, generate an in-app notification/alert record. This check should run as a **background service**, not be recalculated on every single page load.

### 2.7 CSV Import
- Support importing a CSV file of bank transactions (you can construct a sample CSV format yourself — document it).
- The import must:
  - Detect and skip duplicate transactions (same date + amount + description already existing).
  - Handle malformed or inconsistent rows without crashing the whole import — report which rows failed and why.
  - Let the user preview parsed transactions before committing them to the database.

### 2.8 Reporting
- Monthly spend by category (aggregation).
- Balance-over-time / net worth trend across all accounts.
- Income vs. expense summary for a selected date range.
- Consider whether these reports should be computed live from raw transactions or from a maintained summary table — document your reasoning either way.

---

## 3. Cross-Cutting Requirements

### 3.1 Authentication & Authorization
- Reuse/extend the user management approach from your weather app.
- All financial data must be scoped to the logged-in user — no user should ever be able to query or see another user's accounts/transactions via the API, even by guessing IDs.

### 3.2 Concurrency
- Consider what happens if the same account is modified from two browser tabs/devices at nearly the same time (e.g., two transfers out of an account with insufficient combined funds).
- Implement optimistic concurrency control on accounts (e.g., a `RowVersion`/concurrency token) and handle the conflict gracefully — the API should return a clear error, and the frontend should surface it, not silently overwrite one change with another.

### 3.3 Audit Trail
- Every change to a transaction (create, edit, soft-delete) should be recorded somewhere with a timestamp and what changed. You decide the shape (separate audit table vs. event log), but justify the choice.

### 3.4 Data Integrity
- An account's balance, computed from its transaction/transfer history, must always be internally consistent. Write at least one automated test that proves this (e.g., create a set of transactions and transfers, then assert the computed balance matches expectations).

---

## 4. Non-Functional Requirements

- **No hosting required** — the app must run entirely on localhost (API + React dev server + local DB, e.g. SQLite or SQL Server LocalDB).
- **Currency values:** `decimal` type end-to-end (DB column, C# model, API contract). Frontend display formatting should not lose precision in stored data.
- **Background jobs:** use `IHostedService` / `BackgroundService` for recurring transaction generation and budget-alert checks.
- **Error handling:** the API should return meaningful HTTP status codes and error payloads, not generic 500s, for validation failures (e.g., insufficient funds, invalid date, duplicate import row).
- **Tests:** at minimum, unit tests around balance calculation, transfer atomicity, and duplicate-detection logic in CSV import.

---

## 5. Suggested Entity Model (starting point — adjust as needed)

- `User`
- `Account` (Id, UserId, Name, Type, Currency, RowVersion)
- `Transaction` (Id, AccountId, Amount, Date, CategoryId, Description, Type, IsDeleted/ReversalOfId, CreatedAt)
- `Transfer` (Id, FromAccountId, ToAccountId, Amount, Date, links to two Transaction rows)
- `Category` (Id, UserId, Name)
- `RecurringRule` (Id, AccountId, Amount, CategoryId, Frequency, NextRunDate, IsActive)
- `Budget` (Id, UserId, CategoryId, MonthlyLimit)
- `Alert`/`Notification` (Id, UserId, Message, CreatedAt, IsRead)
- `AuditLog` (Id, EntityType, EntityId, ChangeDescription, Timestamp)

You don't have to use this exact shape — but think through the relationships (especially Transfer ↔ Transaction) before you start writing migrations.

---

## 6. Suggested Build Order

1. Accounts + basic transactions (Income/Expense), scoped per user, with auth.
2. Transfers between accounts (atomic, double-entry).
3. Soft-delete + audit trail for transactions.
4. Optimistic concurrency on account balance updates.
5. Categories + budgets + background alert-checking service.
6. Recurring transactions + projected balance view.
7. CSV import with duplicate detection and row-level error reporting.
8. Reporting/aggregation views.

---

## 7. Stretch Goals (optional, if time allows)

- Multi-currency support with historical exchange rates stored per transaction (not just live-converted).
- Export reports to PDF or CSV.
- A "what-if" projection: given recurring rules, show a projected balance N months into the future.
- Rate-limit or debounce budget-alert checks so they don't run excessively.

---

## 8. What I'll Be Evaluating

- Correctness of money handling (`decimal` usage, no silent rounding errors).
- Whether transfers are truly atomic and consistent.
- How you handle concurrent updates — don't just avoid thinking about it.
- Whether deleted/edited transactions preserve history rather than destroying it.
- Whether background jobs are used appropriately (not synchronous, not triggered manually).
- Code organization (separation of API, business logic, and data access layers).
- Test coverage on the trickiest logic (balances, transfers, duplicate detection).

Good luck — this one will make you think like the systems actually have money riding on them, because that's the mindset finance software demands.
