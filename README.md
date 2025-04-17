# TaskSync
A task management system built with ASP.NET Core, showcasing advanced concepts like Repository/Unit of Work, minimal APIs, and clean architecture.

## Features
- Task CRUD via minimal APIs (`/api/v1/tasks`).
- Repository and Unit of Work pattern for data access.
- EF Core with SQL Server.
- Swagger for API documentation.

## Setup
1. Clone the repo: `git clone https://github.com/x12edi/tasksync.git`
2. Update `appsettings.json` with your SQL Server connection string.
3. Run migrations: `dotnet ef database update`
4. Start: `dotnet run`
5. Open `https://localhost:5001/swagger` to test.

## Tech Stack
- ASP.NET Core 8
- EF Core
- SQL Server
- Swagger

*More features (auth, microservices, React) coming soon!*
