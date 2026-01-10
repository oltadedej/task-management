using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Migrations;

/// <summary>
/// Design-time DbContext factory for Entity Framework Core migrations.
/// This factory is used by EF Core tools (like dotnet ef migrations) to create a DbContext instance.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TaskManagementDbContext>
{
    /// <summary>
    /// Creates a new instance of the DbContext for design-time operations.
    /// </summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>A new instance of TaskManagementDbContext.</returns>
    public TaskManagementDbContext CreateDbContext(string[] args)
    {
        // When using --startup-project, EF Core will look for appsettings.json in the startup project directory
        // For design-time, we'll use a default connection string or try to read from the startup project
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "TaskManagement.Api");

        if (!Directory.Exists(basePath))
        {
            // Fallback: use default connection string if startup project path not found
            // This is fine when using --startup-project parameter, as EF Core will handle configuration
            basePath = Directory.GetCurrentDirectory();
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=TaskManagement.db";

        var optionsBuilder = new DbContextOptionsBuilder<TaskManagementDbContext>();

        // IMPORTANT: Specify the migrations assembly as TaskManagement.Migrations
        optionsBuilder.UseSqlite(
            connectionString,
            b => b.MigrationsAssembly("TaskManagement.Migrations"));

        return new TaskManagementDbContext(optionsBuilder.Options);
    }
}

