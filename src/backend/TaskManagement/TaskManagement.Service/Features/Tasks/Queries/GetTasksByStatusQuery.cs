using MediatR;
using TaskManagement.Service.Models.Dtos;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Service.Features.Tasks.Queries;

/// <summary>
/// Query to get tasks filtered by status.
/// </summary>
public class GetTasksByStatusQuery : IRequest<IEnumerable<TaskDto>>
{
    /// <summary>
    /// Gets or sets the status to filter by.
    /// </summary>
    public TaskStatus Status { get; set; }
}