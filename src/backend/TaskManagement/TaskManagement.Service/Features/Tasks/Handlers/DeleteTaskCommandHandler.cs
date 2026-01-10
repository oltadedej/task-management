using MediatR;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Commands;

namespace TaskManagement.Service.Features.Tasks.Handlers;

/// <summary>
/// Handler for deleting a task.
/// </summary>
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTaskCommandHandler"/> class.
    /// </summary>
    /// <param name="taskRepository">The task repository.</param>
    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
    }

    /// <inheritdoc />
    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);

        if (task == null)
        {
            throw new TaskNotFoundException(request.Id);
        }

        _taskRepository.Delete(task);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}

