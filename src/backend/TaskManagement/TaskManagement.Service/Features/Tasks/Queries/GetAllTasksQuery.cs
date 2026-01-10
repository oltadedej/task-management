using MediatR;
using TaskManagement.Service.Models.Dtos;

namespace TaskManagement.Service.Features.Tasks.Queries;

/// <summary>
/// Query to get all tasks.
/// </summary>
public class GetAllTasksQuery : IRequest<IEnumerable<TaskDto>>
{
}