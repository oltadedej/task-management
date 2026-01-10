using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service;
using TaskManagement.Service.Features.Tasks.Commands;
using TaskManagement.Service.Features.Tasks.Queries;
using TaskManagement.Service.Models.Dtos;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();

// Configure API Versioning for Minimal APIs
builder.Services.AddApiVersioning()
    .AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1.0",
        Description = "A comprehensive Task Management System API built with .NET 8, implementing CQRS pattern with MediatR.",
        Contact = new OpenApiContact
        {
            Name = "Task Management Team",
            Email = "support@taskmanagement.com"
        }
    });

    // Include XML comments in Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Include XML comments from Service assembly
    var serviceXmlFile = "TaskManagement.Service.xml";
    var serviceXmlPath = Path.Combine(AppContext.BaseDirectory, serviceXmlFile);
    if (File.Exists(serviceXmlPath))
    {
        c.IncludeXmlComments(serviceXmlPath);
    }
});

// Configure Database
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=TaskManagement.db",
        b => b.MigrationsAssembly("TaskManagement.Migrations")));

// Register repositories
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Register Service Layer (MediatR, FluentValidation, AutoMapper)
builder.Services.AddServiceLayer();


// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply pending migrations at runtime
// This ensures the database is up-to-date with the latest migrations before the application starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<TaskManagement.Api.Program>>();

    try
    {
        var context = services.GetRequiredService<TaskManagementDbContext>();

        // Check if there are pending migrations
        var pendingMigrations = context.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migration(s): {Migrations}",
                pendingMigrations.Count, string.Join(", ", pendingMigrations));

            // Apply pending migrations
            context.Database.Migrate();

            logger.LogInformation("Database migrations applied successfully. Database is up-to-date.");
        }
        else
        {
            logger.LogInformation("No pending migrations. Database is already up-to-date.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying database migrations.");

        // In production, fail fast to prevent running with incorrect database state
        // In development, log the error but continue to allow manual intervention
        if (app.Environment.IsProduction())
        {
            logger.LogCritical("Application startup failed due to migration error. Application will not start.");
            throw;
        }

        logger.LogWarning("Application will continue despite migration error. Please apply migrations manually.");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant()
            );
        }
    });
}

app.UseCors();
app.UseHttpsRedirection();

// Configure Minimal API endpoints - Versioning is handled via URL path
var apiV1 = app.MapGroup("/api/v1").WithTags("Tasks");

// Get all tasks
apiV1.MapGet("/tasks", async (IMediator mediator, CancellationToken cancellationToken) =>
{
    var query = new GetAllTasksQuery();
    var result = await mediator.Send(query, cancellationToken);
    return Results.Ok(result);
})
.WithName("GetAllTasks")
.WithTags("Tasks")
.Produces<IEnumerable<TaskDto>>(StatusCodes.Status200OK)
.WithSummary("Get all tasks")
.WithDescription("Retrieves all tasks from the system, ordered by creation date (newest first).");

// Get task by ID
apiV1.MapGet("/tasks/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
{
    var query = new GetTaskByIdQuery { Id = id };
    var result = await mediator.Send(query, cancellationToken);

    return result == null
        ? Results.NotFound(new { message = $"Task with ID {id} was not found." })
        : Results.Ok(result);
})
.WithName("GetTaskById")
.WithTags("Tasks")
.Produces<TaskDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithSummary("Get task by ID")
.WithDescription("Retrieves a specific task by its unique identifier.");

// Get tasks by status
apiV1.MapGet("/tasks/status/{status:int}", async (int status, IMediator mediator, CancellationToken cancellationToken) =>
    {
        if (!Enum.IsDefined(typeof(TaskStatus), status))
        {
            return Results.BadRequest(new { message = "Invalid status value. Valid values are: 0 (NotStarted), 1 (InProgress), 2 (Completed)." });
        }

        var query = new GetTasksByStatusQuery { Status = (TaskStatus)status };
        var result = await mediator.Send(query, cancellationToken);
        return Results.Ok(result);
    })
.WithName("GetTasksByStatus")
.WithTags("Tasks")
.Produces<IEnumerable<TaskDto>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithSummary("Get tasks by status")
.WithDescription("Retrieves all tasks filtered by status. Status values: 0 = NotStarted, 1 = InProgress, 2 = Completed.");

// Create task
apiV1.MapPost("/tasks", async (CreateTaskDto createTaskDto, IMediator mediator, CancellationToken cancellationToken) =>
{
    var command = new CreateTaskCommand
    {
        Title = createTaskDto.Title,
        Description = createTaskDto.Description,
        DueDate = createTaskDto.DueDate
    };

    try
    {
        var result = await mediator.Send(command, cancellationToken);
        return Results.Created($"/api/v1/tasks/{result.Id}", result);
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { errors = ex.Errors });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("CreateTask")
.WithTags("Tasks")
.Accepts<CreateTaskDto>("application/json")
.Produces<TaskDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.WithSummary("Create a new task")
.WithDescription("Creates a new task in the system. The task will be created with NotStarted status by default.");

// Update task
apiV1.MapPut("/tasks/{id:guid}", async (Guid id, UpdateTaskDto updateTaskDto, IMediator mediator, CancellationToken cancellationToken) =>
{
    var command = new UpdateTaskCommand
    {
        Id = id,
        Title = updateTaskDto.Title,
        Description = updateTaskDto.Description,
        DueDate = updateTaskDto.DueDate
    };

    try
    {
        var result = await mediator.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    catch (TaskNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
    catch (ValidationException ex)
    {
        return Results.BadRequest(new { errors = ex.Errors });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("UpdateTask")
.WithTags("Tasks")
.Accepts<UpdateTaskDto>("application/json")
.Produces<TaskDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest)
.WithSummary("Update an existing task")
.WithDescription("Updates the details of an existing task. The task status is not changed by this endpoint.");

// Delete task
apiV1.MapDelete("/tasks/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
{
    var command = new DeleteTaskCommand { Id = id };

    try
    {
        var result = await mediator.Send(command, cancellationToken);
        return Results.NoContent();
    }
    catch (TaskNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("DeleteTask")
.WithTags("Tasks")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithSummary("Delete a task")
.WithDescription("Deletes a task from the system permanently.");

// Mark task as complete
apiV1.MapPatch("/tasks/{id:guid}/complete", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
{
    var command = new MarkTaskCompleteCommand { Id = id };

    try
    {
        var result = await mediator.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    catch (TaskNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("MarkTaskComplete")
.WithTags("Tasks")
.Produces<TaskDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithSummary("Mark task as complete")
.WithDescription("Marks a task as completed, changing its status to Completed.");

// Mark task as incomplete
apiV1.MapPatch("/tasks/{id:guid}/incomplete", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
{
    var command = new MarkTaskIncompleteCommand { Id = id };

    try
    {
        var result = await mediator.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    catch (TaskNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("MarkTaskIncomplete")
.WithTags("Tasks")
.Produces<TaskDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithSummary("Mark task as incomplete")
.WithDescription("Marks a task as incomplete (not started), changing its status to NotStarted.");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health")
    .ExcludeFromDescription();

app.Run();

namespace TaskManagement.Api
{
    public partial class Program { }
}
