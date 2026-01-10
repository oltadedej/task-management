using AutoMapper;
using MediatR;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Queries;
using TaskManagement.Service.Models.Dtos;

namespace TaskManagement.Service.Features.Tasks.Handlers;

/// <summary>
/// Handler for getting tasks by status.
/// </summary>
public class GetTasksByStatusQueryHandler : IRequestHandler<GetTasksByStatusQuery, IEnumerable<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTasksByStatusQueryHandler"/> class.
    /// </summary>
    /// <param name="taskRepository">The task repository.</param>
    /// <param name="mapper">The mapper.</param>
    public GetTasksByStatusQueryHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TaskDto>> Handle(GetTasksByStatusQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetByStatusAsync(request.Status, cancellationToken);
        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
    }
}

