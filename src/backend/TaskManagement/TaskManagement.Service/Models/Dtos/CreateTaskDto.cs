namespace TaskManagement.Service.Models.Dtos;

/// <summary>
/// Data Transfer Object for creating a task.
/// </summary>
public class CreateTaskDto
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

