using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Handlers;
using TaskManagement.Service.Features.Tasks.Queries;
using TaskManagement.Service.Mappings;
using Xunit;
using ManagementTask = TaskManagement.Domain.Entities.ManagementTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Unit.Queries;

/// <summary>
/// Unit tests for GetTasksByStatusQueryHandler.
/// </summary>
public class GetTasksByStatusQueryHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ITaskRepository> _mockRepository;

    public GetTasksByStatusQueryHandlerTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TaskMappingProfile>());
        _mapper = configuration.CreateMapper();

        _mockRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task Handle_ValidStatus_ShouldReturnFilteredTasks()
    {
        // Arrange
        var tasks = new List<ManagementTask>
        {
            new ManagementTask
            {
                Id = Guid.NewGuid(),
                Title = "Completed Task 1",
                Status = TaskStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new ManagementTask
            {
                Id = Guid.NewGuid(),
                Title = "Completed Task 2",
                Status = TaskStatus.Completed,
                CreatedAt = DateTime.UtcNow
            }
        };

        var query = new GetTasksByStatusQuery { Status = "Completed" };

        _mockRepository
            .Setup(x => x.GetByStatusAsync(TaskStatus.Completed, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var handler = new GetTasksByStatusQueryHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.Status == "Completed");

        _mockRepository.Verify(x => x.GetByStatusAsync(TaskStatus.Completed, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidStatusCaseInsensitive_ShouldReturnFilteredTasks()
    {
        // Arrange
        var tasks = new List<ManagementTask>
        {
            new ManagementTask
            {
                Id = Guid.NewGuid(),
                Title = "Task",
                Status = TaskStatus.InProgress,
                CreatedAt = DateTime.UtcNow
            }
        };

        var query = new GetTasksByStatusQuery { Status = "inprogress" }; // lowercase

        _mockRepository
            .Setup(x => x.GetByStatusAsync(TaskStatus.InProgress, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var handler = new GetTasksByStatusQueryHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Status.Should().Be("InProgress");
    }

    [Fact]
    public async Task Handle_InvalidStatus_ShouldThrowInvalidTaskOperationException()
    {
        // Arrange
        var query = new GetTasksByStatusQuery { Status = (TaskStatus)200 };

        var handler = new GetTasksByStatusQueryHandler(_mockRepository.Object, _mapper);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidTaskOperationException>(() => handler.Handle(query, CancellationToken.None));

        _mockRepository.Verify(x => x.GetByStatusAsync(It.IsAny<TaskStatus>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("NotStarted")]
    [InlineData("InProgress")]
    [InlineData("Completed")]
    [InlineData("notstarted")]
    [InlineData("inprogress")]
    [InlineData("completed")]
    [InlineData("NOTSTARTED")]
    [InlineData("INPROGRESS")]
    [InlineData("COMPLETED")]
    public async Task Handle_ValidStatusVariations_ShouldWork(string statusString)
    {
        // Arrange
        var tasks = new List<ManagementTask>
        {
            new ManagementTask
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Status = TaskStatus.NotStarted,
                CreatedAt = DateTime.UtcNow
            }
        };

        var query = new GetTasksByStatusQuery { Status = statusString };

        _mockRepository
            .Setup(x => x.GetByStatusAsync(It.IsAny<TaskStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var handler = new GetTasksByStatusQueryHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }
}

