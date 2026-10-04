

# SpendWise — Personal Finance & Expense Tracker:

A full-stack personal finance tracker that lets users manage multiple accounts, record transactions, transfer money between accounts, set budgets, track recurring payments, and view reports — all with correct money handling, atomic transfers, concurrency control, and a complete audit trail.

This is not just a CRUD app. Money-handling systems have real correctness requirements: balances must always be accurate, history must never silently change, and concurrent edits must not corrupt data.


# Backend:
asp net,
entityFramework,
database: postgreSQL


# Frontend:
react
css


# Why i used decimal and not float/double for money:
All monetary values in this project use C#'s decimal type, not float/double, float and double are binary floating-point types,
They store numbers as fractions, like for example:
double a = 0.1;
double b = 0.2;
double sum = a + b;

Console.WriteLine(sum);  // 0.30000000000000004

the result should be 0.3 but double returns 0.30000000000000004, this tiny error is invisible in a single calculation,
but over so many transactions it compounds and becomes so messy.


# How to run:
1- cd backend
4- dotnet restore
3- dotnet build
3- dotnet run

4- cd frontend
5- npm install
6- npm run dev
