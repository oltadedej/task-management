using MediatR;
using TaskManagement.Service.Models.Dtos;

namespace TaskManagement.Service.Features.Tasks.Queries;

/// <summary>
/// Query to get a task by ID.
/// </summary>
public class GetTaskByIdQuery : IRequest<TaskDto?>
{
    /// <summary>
    /// Gets or sets the task ID.
    /// </summary>
    public Guid Id { get; set; }
}