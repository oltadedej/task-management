using AutoMapper;
using MediatR;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Commands;
using TaskManagement.Service.Models.Dtos;

namespace TaskManagement.Service.Features.Tasks.Handlers;

/// <summary>
/// Handler for marking a task as complete.
/// </summary>
public class MarkTaskCompleteCommandHandler : IRequestHandler<MarkTaskCompleteCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkTaskCompleteCommandHandler"/> class.
    /// </summary>
    /// <param name="taskRepository">The task repository.</param>
    /// <param name="mapper">The mapper.</param>
    public MarkTaskCompleteCommandHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public async Task<TaskDto> Handle(MarkTaskCompleteCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);

        if (task == null)
        {
            throw new TaskNotFoundException(request.Id);
        }

        task.MarkAsCompleted();
        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskDto>(task);
    }
}

