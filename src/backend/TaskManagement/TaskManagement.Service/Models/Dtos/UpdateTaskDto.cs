namespace TaskManagement.Service.Models.Dtos;

/// <summary>
/// Data Transfer Object for updating a task.
/// </summary>
public class UpdateTaskDto
{
    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the task is due.
    /// </summary>
    public DateTime? DueDate { get; set; }
}

