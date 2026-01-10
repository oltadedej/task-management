using AutoMapper;
using MediatR;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Queries;
using TaskManagement.Service.Models.Dtos;

namespace TaskManagement.Service.Features.Tasks.Handlers;

/// <summary>
/// Handler for getting a task by ID.
/// </summary>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto?>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="taskRepository">The task repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetTaskByIdQueryHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public async Task<TaskDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
        return task == null ? null : _mapper.Map<TaskDto>(task);
    }
}

