using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for the ManagementTask entity.
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.ManagementTask>
{
    /// <summary>
    /// Configures the ManagementTask entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Domain.Entities.ManagementTask> builder)
    {
        // Table name
        builder.ToTable("Tasks");

        // Primary key
        builder.HasKey(t => t.Id);

        // Title configuration
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("Title");

        // Description configuration
        builder.Property(t => t.Description)
            .HasMaxLength(1000)
            .HasColumnName("Description");

        // Status configuration
        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("Status");

        // CreatedAt configuration
        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnName("CreatedAt");

        // UpdatedAt configuration
        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnName("UpdatedAt");

        // DueDate configuration
        builder.Property(t => t.DueDate)
            .HasColumnName("DueDate");

        // Indexes
        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tasks_Status");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tasks_CreatedAt");

        // Seed data (optional - for initial data)
        SeedData(builder);
    }

    /// <summary>
    /// Seeds initial data for the Tasks table.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    private static void SeedData(EntityTypeBuilder<Domain.Entities.ManagementTask> builder)
    {
        // Optionally add seed data here
        // Example:
        // builder.HasData(
        //     new Entities.ManagementTask
        //     {
        //         Id = Guid.NewGuid(),
        //         Title = "Sample Task",
        //         Description = "This is a sample task",
        //         Status = TaskStatus.NotStarted,
        //         CreatedAt = DateTime.UtcNow,
        //         UpdatedAt = DateTime.UtcNow
        //     }
        // );
    }
}

