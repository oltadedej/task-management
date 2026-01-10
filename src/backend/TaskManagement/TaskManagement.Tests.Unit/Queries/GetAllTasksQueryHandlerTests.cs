using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Handlers;
using TaskManagement.Service.Features.Tasks.Queries;
using TaskManagement.Service.Mappings;
using ManagementTask = TaskManagement.Domain.Entities.ManagementTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Unit.Queries;

/// <summary>
/// Unit tests for GetAllTasksQueryHandler.
/// </summary>
public class GetAllTasksQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ITaskRepository> _mockRepository;

    public GetAllTasksQueryHandlerTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TaskMappingProfile>());
        _mapper = configuration.CreateMapper();

        _mockRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllTasks()
    {
        // Arrange
        var tasks = new List<ManagementTask>
        {
            new ManagementTask
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Status = TaskStatus.NotStarted,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new ManagementTask
            {
                Id = Guid.NewGuid(),
                Title = "Task 2",
                Status = TaskStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        var query = new GetAllTasksQuery();

        _mockRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var handler = new GetAllTasksQueryHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Task 1");

        _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoTasks_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllTasksQuery();

        _mockRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ManagementTask>());

        var handler = new GetAllTasksQueryHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

