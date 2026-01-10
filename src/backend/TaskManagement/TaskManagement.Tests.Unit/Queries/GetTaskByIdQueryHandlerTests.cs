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
/// Unit tests for GetTaskByIdQueryHandler.
/// </summary>
public class GetTaskByIdQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ITaskRepository> _mockRepository;

    public GetTaskByIdQueryHandlerTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TaskMappingProfile>());
        _mapper = configuration.CreateMapper();

        _mockRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task Handle_ValidId_ShouldReturnTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new ManagementTask
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = TaskStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var handler = new GetTaskByIdQueryHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.Title.Should().Be(task.Title);
        result.Description.Should().Be(task.Description);

        _mockRepository.Verify(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidId_ShouldReturnNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var query = new GetTaskByIdQuery { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementTask?)null);

        var handler = new GetTaskByIdQueryHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

