using MediatR;

namespace TaskManagement.Service.Features.Tasks.Commands;

/// <summary>
/// Command to delete a task.
/// </summary>
public class DeleteTaskCommand : IRequest<bool>
{
    /// <summary>
    /// Gets or sets the task ID.
    /// </summary>
    public Guid Id { get; set; }
}

