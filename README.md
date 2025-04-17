# TaskSync
A task management system built with ASP.NET Core, showcasing advanced concepts like Repository/Unit of Work, minimal APIs, authentication, and clean architecture.

## Features
- Task CRUD via minimal APIs (`/api/v1/tasks`).
- Repository and Unit of Work pattern for data access.
- JWT authentication with role-based authorization (User/Manager).
- Refresh tokens for session management.
- Rate limiting to protect APIs.
- Global error handling with JSON responses.
- EF Core with local SQL Server.
- Swagger with JWT support.
- Modular API endpoints in separate files.

## Setup
1. Clone the repo: `git clone <url>`
2. Ensure local SQL Server is running and update `appsettings.json` with your connection string.
3. Run migrations: `cd TaskService && dotnet ef database update`
4. Start: `cd TaskService && dotnet run`
5. Open `https://localhost:5001/swagger` to test.
6. Register a user via `/api/v1/auth/register`, then login to get a token.
7. Use the token in Swagger to access task endpoints.

## Tech Stack
- ASP.NET Core 8
- EF Core
- SQL Server (local)
- JWT Authentication
- Swagger

## Notes
- Rate limiting: 10 requests/sec general, 5 login attempts/min.
- Manager role required for deleting tasks.
- Next: Microservices, CQRS, and React dashboard.
