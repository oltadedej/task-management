using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

/// <summary>
/// Represents a task in the task management system.
/// </summary>
public class ManagementTask
{
    /// <summary>
    /// Gets or sets the unique identifier of the task.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the status of the task (Completed, InProgress, NotStarted).
    /// </summary>
    public Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task is due (optional).
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Marks the task as completed.
    /// </summary>
    public void MarkAsCompleted()
    {
        Status = TaskStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the task as in progress.
    /// </summary>
    public void MarkAsInProgress()
    {
        Status = TaskStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the task as not started.
    /// </summary>
    public void MarkAsNotStarted()
    {
        Status = TaskStatus.NotStarted;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the task details.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <param name="description">The new description.</param>
    /// <param name="dueDate">The new due date.</param>
    public void UpdateDetails(string title, string? description, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }
}

