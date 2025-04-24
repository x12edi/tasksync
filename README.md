# TaskSync
A task management system built with ASP.NET Core microservices, featuring UserService for authentication, TaskService for task management, and an Ocelot gateway for routing. Implements MediatR, CQRS, service layer, Unit of Work, JWT authentication, clean architecture and react front-end.

## Features
- User registration and login via minimal APIs (/api/v1/auth/register, /api/v1/auth/login).
- MediatR/CQRS with LoginCommand, RegisterCommand.
- Service layer (IUserService) and Unit of Work (IUnitOfWork).
- Task CRUD via minimal APIs (`/api/v1/tasks`).
- Repository and Unit of Work pattern for data access.
- JWT authentication with role-based authorization (User/Manager).
- Rate limiting to protect APIs.
- Global error handling with JSON responses.
- EF Core with local SQL Server.
- Swagger with JWT support.
- Modular API endpoints in separate files.
- API Gateway : Routes /api/v1/auth/* to UserService, /api/v1/tasks/* to TaskService.
- Frontend : Built with Vite, React, TypeScript, Ant Design. Task crud with pagination, filtering, sorting, and search.

## Setup
1. Clone the repo: `git clone <url>`
2. Ensure local SQL Server is running and update `appsettings.json` with your connection string.
3. Run migrations: `cd TaskService && dotnet ef database update`
4. Start: `cd TaskService && dotnet run`; 'cd ../UserService && dotnet run'; 'cd ../Gateway && dotnet run'
5. Open `https://localhost:5001/swagger` to test.
6. Register a user via `/api/v1/auth/register`, then login to get a token.
7. Use the token in Swagger to access task endpoints.

## Frontend Setup
- cd ReactDashboard
- npm install
- npm run dev
- Access dashboard at http://localhost:3000

## Tech Stack
- ASP.NET Core 8
- EF Core
- SQL Server (local)
- JWT Authentication
- Swagger
- MediatR (CQRS)
- Ocelot Gateway
- React, Vite, TypeScript, Ant Design

## Testing
1. Open https://localhost:5001/swagger (TaskService) or https://localhost:5002/swagger (UserService) to test APIs.
2. Register a user via POST /api/v1/auth/register (e.g., { "username": "testuser", "password": "Password123!" }).
3. Login via POST /api/v1/auth/login to get a JWT token.
4. Use token in Swagger or frontend to access task endpoints via Ocelot (https://localhost:5000).

## Test frontend:
1. Login at http://localhost:3000/login (e.g., testuser, Password123!).
2. Navigate to /tasks to manage tasks.

## Notes
- Ensure Jwt:Key, Issuer, Audience match across UserService and TaskService.
- Ocelot runs on https://localhost:5000, routing to UserService (5002) and TaskService (5001).
- Rate limiting: 10 requests/sec general, 5 login attempts/min.
- Manager role required for deleting tasks.
- Next: Kafka implementation - enables asynchronous and decoupled message exchange between microservices.
