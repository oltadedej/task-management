using MediatR;
using TaskManagement.Service.Models.Dtos;

namespace TaskManagement.Service.Features.Tasks.Commands;

/// <summary>
/// Command to mark a task as complete.
/// </summary>
public class MarkTaskCompleteCommand : IRequest<TaskDto>
{
    /// <summary>
    /// Gets or sets the task ID.
    /// </summary>
    public Guid Id { get; set; }
}