# Task Management System - Backend API

A comprehensive Task Management System built with .NET 8, implementing Clean Architecture principles, CQRS pattern with MediatR, and following best practices for enterprise-level applications.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Migrations](#database-migrations)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Development Guidelines](#development-guidelines)
- [Troubleshooting](#troubleshooting)

## Overview

This project is a RESTful API for managing tasks. It provides full CRUD operations, task status management, and filtering capabilities. The system is built with a focus on maintainability, testability, and scalability.

### Key Features

- ✅ Full CRUD operations for tasks
- ✅ Task status management (NotStarted, InProgress, Completed)
- ✅ Task filtering by status
- ✅ Comprehensive validation using FluentValidation
- ✅ CQRS pattern implementation with MediatR
- ✅ Clean Architecture with separation of concerns
- ✅ API versioning support
- ✅ Swagger/OpenAPI documentation
- ✅ Comprehensive unit and integration tests
- ✅ SQLite database with Entity Framework Core

## Architecture

The solution follows Clean Architecture principles with clear separation of concerns.

### Important Note on Entity Naming

The main domain entity is named `ManagementTask` (instead of `Task`) to avoid naming conflicts with `System.Threading.Tasks.Task` from the .NET framework. Throughout the codebase:
- **Domain Entity**: `ManagementTask` (class in `TaskManagement.Domain.Entities`)
- **Business Concept**: Still referred to as "task" in API endpoints and documentation (e.g., `/api/v1/tasks`)
- **Database Table**: `Tasks` (plural, as per conventions)

```
┌─────────────────────────────────────────────────────────────┐
│                     API Layer (Minimal APIs)                 │
│                   - Endpoints                                │
│                   - Request/Response mapping                 │
│                   - Swagger configuration                    │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                  Application Layer (Service)                 │
│                   - CQRS Commands & Queries                  │
│                   - Handlers (MediatR)                       │
│                   - DTOs                                     │
│                   - Validators (FluentValidation)            │
│                   - AutoMapper profiles                      │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                     Domain Layer                             │
│                   - Entities                                 │
│                   - Enums                                    │
│                   - Exceptions                               │
│                   - Business logic                           │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                         │
│                   - DbContext                                │
│                   - Repositories                             │
│                   - Entity Configurations                    │
│                   - Database (SQLite)                        │
└─────────────────────────────────────────────────────────────┘
```

## Technology Stack

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 8.0** - ORM
- **SQLite** - Database
- **MediatR 12.2.0** - CQRS pattern implementation
- **FluentValidation 11.9.0** - Request validation
- **AutoMapper 12.0.1** - Object mapping
- **Swashbuckle.AspNetCore 6.8.1** - Swagger/OpenAPI
- **Asp.Versioning.Http 8.1.0** - API versioning
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library

## Project Structure

```
TaskManagement/
├── TaskManagement.Api/                  # API layer (Minimal APIs)
│   ├── Program.cs                       # Application entry point
│   └── appsettings.json                 # Configuration
│
├── TaskManagement.Service/              # Application layer (CQRS)
│   ├── Features/
│   │   └── Tasks/
│   │       ├── Commands/                # Command definitions
│   │       ├── Queries/                 # Query definitions
│   │       ├── Handlers/                # Command/Query handlers
│   │       └── Validators/              # FluentValidation validators
│   ├── Models/
│   │   └── Dtos/                        # Data Transfer Objects
│   ├── Mappings/                        # AutoMapper profiles
│   └── Behaviors/                       # MediatR pipeline behaviors
│
├── TaskManagement.Domain/               # Domain layer
│   ├── Entities/                        # Domain entities
│   │   └── Task.cs                      # Contains ManagementTask class (renamed to avoid Task conflict)
│   ├── Enums/                           # Domain enums (TaskStatus)
│   └── Exceptions/                      # Domain exceptions
│
├── TaskManagement.Infrastructure/       # Infrastructure layer
│   ├── Data/                            # DbContext
│   │   ├── TaskManagementDbContext.cs   # DbContext with ManagementTask DbSet
│   │   └── Configurations/              # EF Core configurations (TaskConfiguration)
│   └── Repositories/                    # Repository implementations (ITaskRepository, TaskRepository)
│
├── TaskManagement.Migrations/           # EF Core migrations project
│   └── Migrations/                      # Generated migration files (created when running migrations add)
│
├── TaskManagement.Tests.Unit/           # Unit tests
│   ├── Commands/                        # Command handler tests
│   ├── Queries/                         # Query handler tests
│   ├── Validators/                      # Validator tests
│   └── Repositories/                    # Repository tests
│
└── TaskManagement.Tests.Integration/    # Integration tests
    ├── TasksApiTests.cs                 # API endpoint tests
    └── EdgeCasesTests.cs                # Edge case tests
```

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/) for version control

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd task-management/src/backend/TaskManagement
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

## Database Migrations

This project uses Entity Framework Core for database management. Migrations are generated and stored in the `TaskManagement.Migrations` project to keep them separate from the Infrastructure layer.

**Important Configuration:** 

1. The DbContext is configured to use `TaskManagement.Migrations` as the migrations assembly in `Program.cs`:

```csharp
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=TaskManagement.db",
        b => b.MigrationsAssembly("TaskManagement.Migrations")));
```

2. A `DesignTimeDbContextFactory` is provided in the `TaskManagement.Migrations` project to ensure EF Core tools can properly create the DbContext during migration generation.

This ensures that all migrations are generated in the `TaskManagement.Migrations` project. The `--startup-project` parameter points to the API project to read configuration (connection strings, appsettings.json, etc.).

**Note:** If you see an error about migrations assembly mismatch, ensure:
- There are no existing migrations in the `TaskManagement.Infrastructure` project
- The `MigrationsAssembly` is correctly specified in both `Program.cs` and `DesignTimeDbContextFactory.cs`

### Creating a Migration

To create a new migration, run the following command from the solution root. Migrations will be generated in the `TaskManagement.Migrations` project:

```bash
dotnet ef migrations add <MigrationName> --project TaskManagement.Migrations --startup-project TaskManagement.Api --context TaskManagementDbContext
```

**Example:**
```bash
dotnet ef migrations add InitialCreate --project TaskManagement.Migrations --startup-project TaskManagement.Api --context TaskManagementDbContext
```

**Note:** Migrations are generated in the `TaskManagement.Migrations` project's `Migrations` folder to keep them separate from the Infrastructure layer.

### Applying Migrations

#### Option 1: Apply migrations automatically on application startup (Default Behavior)

The application is **automatically configured** to apply pending migrations on startup. When the application starts, it will:
- Check for pending migrations
- Apply them automatically before accepting requests
- Log the migration status (success or error)

This is configured in `Program.cs`:

```csharp
// Apply pending migrations at runtime
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TaskManagementDbContext>();
    context.Database.Migrate();
}
```

**Behavior by Environment:**
- **Development**: Migrations are applied automatically. If an error occurs, it's logged but the application continues to start (for easier debugging).
- **Production**: Migrations are applied automatically. If an error occurs, the application fails to start to prevent running with an incorrect database state.

#### Option 2: Apply migrations manually using EF Core CLI

If you prefer to apply migrations manually (e.g., in production with review process), you can disable automatic migration in `Program.cs` and use:

```bash
dotnet ef database update --project TaskManagement.Migrations --startup-project TaskManagement.Api --context TaskManagementDbContext
```

This will create or update the SQLite database file (`TaskManagement.db`) in the API project's output directory by applying all pending migrations from the `TaskManagement.Migrations` project.

### Removing Migrations

If you need to remove the last migration (before applying it):

```bash
dotnet ef migrations remove --project TaskManagement.Migrations --startup-project TaskManagement.Api --context TaskManagementDbContext
```

This will remove the last migration from the `TaskManagement.Migrations` project.

### Generating SQL Script

To generate a SQL script from migrations without applying them:

```bash
dotnet ef migrations script --project TaskManagement.Migrations --startup-project TaskManagement.Api --context TaskManagementDbContext --output migration.sql
```

This will generate a SQL script from all migrations in the `TaskManagement.Migrations` project.

### Migration Best Practices

1. **Always review migrations** before applying them to production
2. **Test migrations** on a copy of production data when possible
3. **Use descriptive names** for migrations (e.g., `AddDueDateToTask`)
4. **Never edit existing migrations** - create new ones instead
5. **Keep migrations small and focused** - one logical change per migration

## Running the Application

### Development Mode

1. Navigate to the API project:
```bash
cd TaskManagement.Api
```

2. Run the application:
```bash
dotnet run
```

Or from the solution root:
```bash
dotnet run --project TaskManagement.Api
```

The API will start at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

3. Access Swagger UI:
```
https://localhost:5001/swagger
```

### Configuration

Database connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TaskManagement.db"
  }
}
```

You can override this in `appsettings.Development.json` for development-specific settings.

## API Documentation

### Base URL

```
https://localhost:5001/api/v1
```

### Endpoints

#### 1. Get All Tasks

```http
GET /api/v1/tasks
```

**Response:**
```json
[
  {
    "id": "guid",
    "title": "Task Title",
    "description": "Task Description",
    "status": 0,
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-01-01T00:00:00Z",
    "dueDate": "2026-01-15T00:00:00Z"
  }
]
```

#### 2. Get Task by ID

```http
GET /api/v1/tasks/{id}
```

**Parameters:**
- `id` (Guid) - Task identifier

**Response (200 OK):**
```json
{
  "id": "guid",
  "title": "Task Title",
  "description": "Task Description",
  "status": 0,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-01T00:00:00Z",
  "dueDate": "2026-01-15T00:00:00Z"
}
```

**Response (404 Not Found):**
```json
{
  "message": "Task with ID {id} was not found."
}
```

#### 3. Get Tasks by Status

```http
GET /api/v1/tasks/status/{status}
```

**Parameters:**
- `status` (int) - Task status (0 = NotStarted, 1 = InProgress, 2 = Completed)

**Response:**
```json
[
  {
    "id": "guid",
    "title": "Task Title",
    "description": "Task Description",
    "status": 0,
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-01-01T00:00:00Z",
    "dueDate": "2026-01-15T00:00:00Z"
  }
]
```

#### 4. Create Task

```http
POST /api/v1/tasks
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "New Task",
  "description": "Task Description",
  "dueDate": "2026-01-15T00:00:00Z"
}
```

**Validation Rules:**
- `title`: Required, max 200 characters
- `description`: Optional, max 1000 characters
- `dueDate`: Optional, must be in the future if provided

**Response (201 Created):**
```json
{
  "id": "guid",
  "title": "New Task",
  "description": "Task Description",
  "status": 0,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-01T00:00:00Z",
  "dueDate": "2026-01-15T00:00:00Z"
}
```

#### 5. Update Task

```http
PUT /api/v1/tasks/{id}
Content-Type: application/json
```

**Parameters:**
- `id` (Guid) - Task identifier

**Request Body:**
```json
{
  "title": "Updated Task Title",
  "description": "Updated Description",
  "dueDate": "2026-01-20T00:00:00Z"
}
```

**Response (200 OK):**
```json
{
  "id": "guid",
  "title": "Updated Task Title",
  "description": "Updated Description",
  "status": 1,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-02T00:00:00Z",
  "dueDate": "2026-01-20T00:00:00Z"
}
```

**Note:** The task status is NOT changed by this endpoint. Use status-specific endpoints to change status.

#### 6. Delete Task

```http
DELETE /api/v1/tasks/{id}
```

**Parameters:**
- `id` (Guid) - Task identifier

**Response (204 No Content):** Empty body

**Response (404 Not Found):**
```json
{
  "message": "Task with ID {id} was not found."
}
```

#### 7. Mark Task as Complete

```http
PATCH /api/v1/tasks/{id}/complete
```

**Parameters:**
- `id` (Guid) - Task identifier

**Response (200 OK):**
```json
{
  "id": "guid",
  "title": "Task Title",
  "description": "Task Description",
  "status": 2,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-02T00:00:00Z",
  "dueDate": "2026-01-15T00:00:00Z"
}
```

#### 8. Mark Task as Incomplete

```http
PATCH /api/v1/tasks/{id}/incomplete
```

**Parameters:**
- `id` (Guid) - Task identifier

**Response (200 OK):**
```json
{
  "id": "guid",
  "title": "Task Title",
  "description": "Task Description",
  "status": 0,
  "createdAt": "2026-01-01T00:00:00Z",
  "updatedAt": "2026-01-02T00:00:00Z",
  "dueDate": "2026-01-15T00:00:00Z"
}
```

#### 9. Health Check

```http
GET /health
```

**Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2026-01-01T00:00:00Z"
}
```

### Task Status Values

- `0` - NotStarted
- `1` - InProgress
- `2` - Completed

## Testing

### Running Unit Tests

```bash
dotnet test TaskManagement.Tests.Unit
```

### Running Integration Tests

```bash
dotnet test TaskManagement.Tests.Integration
```

### Running All Tests

```bash
dotnet test
```

### Test Coverage

The project includes comprehensive test coverage for:

- **Command Handlers** - All CRUD operations
- **Query Handlers** - All read operations
- **Validators** - All FluentValidation rules
- **Repositories** - All data access operations
- **API Endpoints** - All REST endpoints
- **Edge Cases** - Boundary conditions and error scenarios

## Development Guidelines

### Code Style

- Follow C# coding conventions
- Use meaningful names for classes, methods, and variables
- Add XML documentation comments to public APIs
- Keep methods focused and single-purpose

### Adding New Features

1. **Domain Layer**: Add entities, enums, or exceptions
   - The main entity is `ManagementTask` (located in `TaskManagement.Domain/Entities/ManagementTask.cs`)
   - Note: The class is named `ManagementTask` to avoid conflicts with `System.Threading.Tasks.Task`
2. **Infrastructure Layer**: Add repository interfaces and implementations
   - Update `TaskManagementDbContext` if adding new entities
   - Create entity configurations in `Configurations/` folder
3. **Service Layer**: 
   - Create DTOs for the feature
   - Create Commands/Queries
   - Create Handlers
   - Create Validators
   - Add AutoMapper mappings (update `TaskMappingProfile`)
4. **API Layer**: Add Minimal API endpoints
5. **Tests**: Add unit and integration tests

### CQRS Pattern

- **Commands**: Use for write operations (Create, Update, Delete)
- **Queries**: Use for read operations (Get, List)
- **Handlers**: Implement business logic for Commands/Queries
- **Validators**: Validate Commands/Queries using FluentValidation

### Example: Adding a New Feature

```csharp
// 1. Domain Layer - Update ManagementTask entity if needed
// File: TaskManagement.Domain/Entities/Task.cs
// Class: ManagementTask (to avoid conflict with System.Threading.Tasks.Task)

// 2. Create Command
public class ArchiveTaskCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

// 3. Create Handler
public class ArchiveTaskCommandHandler : IRequestHandler<ArchiveTaskCommand, bool>
{
    private readonly ITaskRepository _repository;
    
    public ArchiveTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<bool> Handle(ArchiveTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (task == null) throw new TaskNotFoundException(request.Id);
        
        // Archive logic here
        _repository.Update(task);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// 4. Create Validator
public class ArchiveTaskCommandValidator : AbstractValidator<ArchiveTaskCommand>
{
    public ArchiveTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

// 5. Add API Endpoint
apiV1.MapPost("/tasks/{id:guid}/archive", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
{
    var command = new ArchiveTaskCommand { Id = id };
    var result = await mediator.Send(command, cancellationToken);
    return Results.Ok(result);
});
```

**Note:** The domain entity is named `ManagementTask` to avoid naming conflicts with `System.Threading.Tasks.Task`. When working with the entity, use `ManagementTask` or `TaskManagement.Domain.Entities.ManagementTask`.

## Troubleshooting

### Common Issues

#### 1. Database file not found

**Issue:** `Microsoft.Data.Sqlite.SqliteException: SQLite Error 1: 'no such table: Tasks'`

**Solution:** Apply migrations:
```bash
dotnet ef database update --project TaskManagement.Migrations --startup-project TaskManagement.Api --context TaskManagementDbContext
```

#### 2. Migrations assembly mismatch error

**Issue:** `Your target project 'TaskManagement.Migrations' doesn't match your migrations assembly 'TaskManagement.Infrastructure'`

**Solution:** 
- Ensure `MigrationsAssembly("TaskManagement.Migrations")` is specified in both `Program.cs` and `DesignTimeDbContextFactory.cs`
- Clean and rebuild the solution:
  ```bash
  dotnet clean
  dotnet build
  ```
- Delete the `bin` and `obj` folders in all projects if the issue persists
- Verify that the `DesignTimeDbContextFactory` exists in the `TaskManagement.Migrations` project

#### 2. Port already in use

**Issue:** `Address already in use`

**Solution:** Change the port in `launchSettings.json` or stop the application using the port.

#### 3. NuGet package restore fails

**Issue:** Package restore errors

**Solution:** 
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Restore packages: `dotnet restore`
- Check for custom NuGet.config files that might be causing issues

#### 4. XML documentation warnings

**Issue:** XML documentation warnings during build

**Solution:** XML documentation is optional. Warnings are suppressed in project files, but you can add XML comments to public APIs to improve documentation.

### Getting Help

- Check the [Swagger UI](https://localhost:5001/swagger) for API documentation
- Review test files for usage examples
- Check logs for detailed error messages

## License

[Your License Here]

## Contributing

[Contributing Guidelines Here]

---

**Last Updated:** 2026-01-14

