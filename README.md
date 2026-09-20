# Task & Expense Tracker API

A RESTful API for managing personal tasks and expenses with JWT-based authentication, built with ASP.NET Core 10 and PostgreSQL.

🌐 **Live Demo:** https://task-expense-tracker-production.up.railway.app/swagger  
💻 **GitHub:** https://github.com/aniruddha4/task-expense-tracker

---

## Overview

The Task & Expense Tracker API allows authenticated users to create, read, update, and delete their personal tasks and expenses. Each user's data is isolated — you can only see your own tasks and expenses. The API is fully documented via Swagger/OpenAPI and secured with JSON Web Tokens (JWT).

---

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────┐
│                        CLIENT                           │
│              (Browser / Postman / Mobile App)           │
└────────────────────────┬────────────────────────────────┘
                         │ HTTPS
                         ▼
┌─────────────────────────────────────────────────────────┐
│                   ASP.NET Core 10 API                   │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────────┐  │
│  │    Auth     │  │    Tasks     │  │   Expenses    │  │
│  │ Controller  │  │  Controller  │  │  Controller   │  │
│  └──────┬──────┘  └──────┬───────┘  └───────┬───────┘  │
│         │                │                  │           │
│  ┌──────▼──────────────────────────────────▼───────┐   │
│  │            ASP.NET Core Identity + JWT           │   │
│  └──────────────────────┬───────────────────────────┘   │
│                         │                               │
│  ┌──────────────────────▼───────────────────────────┐   │
│  │           Entity Framework Core (ORM)            │   │
│  └──────────────────────┬───────────────────────────┘   │
└─────────────────────────┼───────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    PostgreSQL Database                   │
│   ┌───────────┐   ┌────────────┐   ┌───────────────┐   │
│   │   Users   │   │   Tasks    │   │   Expenses    │   │
│   └───────────┘   └────────────┘   └───────────────┘   │
└─────────────────────────────────────────────────────────┘
```

---

## Request Flow Diagram

```
Client                  API                    Database
  │                      │                         │
  │──POST /auth/register─►│                         │
  │                      │──INSERT User────────────►│
  │◄──200 OK─────────────│◄────────────────────────│
  │                      │                         │
  │──POST /auth/login────►│                         │
  │                      │──SELECT User────────────►│
  │                      │◄────────────────────────│
  │◄──200 OK + JWT token─│                         │
  │                      │                         │
  │──POST /api/tasks─────►│                         │
  │  Authorization: Bearer│                         │
  │  {JWT token}         │──Validate JWT token      │
  │                      │──INSERT Task────────────►│
  │◄──201 Created────────│◄────────────────────────│
  │                      │                         │
  │──GET /api/tasks──────►│                         │
  │  Authorization: Bearer│                         │
  │  {JWT token}         │──Validate JWT token      │
  │                      │──SELECT Tasks WHERE      │
  │                      │  UserId = current user──►│
  │◄──200 OK + tasks─────│◄────────────────────────│
```

---

## Project Structure

```
TaskExpenseTracker/
├── TaskExpenseTracker.API/          # Presentation layer
│   ├── Controllers/
│   │   ├── AuthController.cs        # Register & Login endpoints
│   │   ├── TasksController.cs       # CRUD for tasks
│   │   └── ExpensesController.cs    # CRUD for expenses
│   ├── Program.cs                   # App configuration & DI setup
│   └── appsettings.json             # Configuration (JWT, DB connection)
│
├── TaskExpenseTracker.Application/  # Business logic layer
│   └── DTOs/
│       ├── AuthDtos.cs              # Register, Login, AuthResponse DTOs
│       ├── TaskDtos.cs              # Create, Update, TaskResponse DTOs
│       └── ExpenseDtos.cs           # Create, Update, ExpenseResponse DTOs
│
├── TaskExpenseTracker.Domain/       # Core entities
│   └── Entities/
│       ├── AppTask.cs               # Task entity
│       └── Expense.cs               # Expense entity
│
├── TaskExpenseTracker.Infrastructure/ # Data access layer
│   ├── Data/
│   │   └── AppDbContext.cs          # EF Core DbContext with Identity
│   └── Migrations/                  # EF Core database migrations
│
└── TaskExpenseTracker.Tests/        # Unit & integration tests
```

---

## Components

### AuthController
Handles user registration and login. On successful login, generates a signed JWT token containing the user's ID and email. The token is valid for 8 hours and must be passed as a `Bearer` token in the `Authorization` header for all protected endpoints.

### TasksController
Full CRUD for tasks. All endpoints are protected with `[Authorize]`. Tasks are scoped to the authenticated user — the `UserId` is extracted from the JWT token, not from the request body, so users can never access another user's data.

### ExpensesController
Full CRUD for expenses. Same auth pattern as tasks. Expenses include description, amount, category, and date fields.

### AppDbContext
EF Core context extending `IdentityDbContext`, giving us ASP.NET Core Identity tables (users, roles, claims) alongside the custom `Tasks` and `Expenses` tables in the same PostgreSQL database.

### JWT Authentication
On login, a `JwtSecurityToken` is created with three claims: `NameIdentifier` (user ID), `Email`, and a unique `Jti` (token ID). The token is signed with HMAC-SHA256 using a secret key stored in configuration.

---

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | ❌ | Register a new user |
| POST | `/api/auth/login` | ❌ | Login and receive JWT token |
| GET | `/api/tasks` | ✅ | Get all tasks for current user |
| POST | `/api/tasks` | ✅ | Create a new task |
| GET | `/api/tasks/{id}` | ✅ | Get a specific task |
| PUT | `/api/tasks/{id}` | ✅ | Update a task |
| DELETE | `/api/tasks/{id}` | ✅ | Delete a task |
| GET | `/api/expenses` | ✅ | Get all expenses for current user |
| POST | `/api/expenses` | ✅ | Create a new expense |
| GET | `/api/expenses/{id}` | ✅ | Get a specific expense |
| PUT | `/api/expenses/{id}` | ✅ | Update an expense |
| DELETE | `/api/expenses/{id}` | ✅ | Delete an expense |

---

## Tech Stack

| Technology | Purpose |
|------------|---------|
| ASP.NET Core 10 | Web API framework |
| Entity Framework Core | ORM for database access |
| PostgreSQL | Relational database |
| ASP.NET Core Identity | User management |
| JWT Bearer Auth | Stateless authentication |
| Swagger / OpenAPI | API documentation |
| Docker | Containerization |
| Railway | Cloud deployment |

---

## Running Locally

### Prerequisites
- .NET 10 SDK
- Docker Desktop

### Steps

```bash
# Clone the repository
git clone https://github.com/aniruddha4/task-expense-tracker.git
cd task-expense-tracker

# Start PostgreSQL with Docker
docker run --name taskdb -e POSTGRES_PASSWORD=password \
  -e POSTGRES_DB=taskexpense -p 5432:5432 -d postgres:16

# Run the API
cd TaskExpenseTracker.API
dotnet run
```

Open `http://localhost:5000/swagger` to explore the API.

### Environment Variables

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__Default` | PostgreSQL connection string |
| `Jwt__Key` | JWT signing secret (min 32 chars) |
| `Jwt__Issuer` | JWT issuer name |
| `Jwt__Audience` | JWT audience name |
