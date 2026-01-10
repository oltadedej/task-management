using MediatR;
using TaskManagement.Service.Models.Dtos;

namespace TaskManagement.Service.Features.Tasks.Commands;

/// <summary>
/// Command to update an existing task.
/// </summary>
public class UpdateTaskCommand : IRequest<TaskDto>
{
    /// <summary>
    /// Gets or sets the task ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the task title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the task description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the task due date.
    /// </summary>
    public DateTime? DueDate { get; set; }
}

