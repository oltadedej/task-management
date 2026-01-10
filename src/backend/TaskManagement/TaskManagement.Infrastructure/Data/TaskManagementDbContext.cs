using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Infrastructure.Data;

/// <summary>
/// Database context for the Task Management system.
/// </summary>
public class TaskManagementDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskManagementDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Tasks DbSet.
    /// </summary>
    public DbSet<Domain.Entities.ManagementTask> Tasks { get; set; } = null!;

    /// <summary>
    /// Configures the model and applies entity configurations.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagementDbContext).Assembly);
    }
}

