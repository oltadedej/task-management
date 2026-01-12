using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Commands;
using TaskManagement.Service.Features.Tasks.Handlers;
using TaskManagement.Service.Mappings;
using ManagementTask = TaskManagement.Domain.Entities.ManagementTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Unit.Commands;

/// <summary>
/// Unit tests for MarkTaskInProgressCommandHandler.
/// </summary>
public class MarkTaskInProgressCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ITaskRepository> _mockRepository;

    public MarkTaskInProgressCommandHandlerTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TaskMappingProfile>());
        _mapper = configuration.CreateMapper();

        _mockRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldMarkTaskAsInProgress()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new ManagementTask
        {
            Id = taskId,
            Title = "Task",
            Status = TaskStatus.NotStarted,
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new MarkTaskInProgressCommand { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new MarkTaskInProgressCommandHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(TaskStatus.InProgress);
        task.Status.Should().Be(TaskStatus.InProgress);

        _mockRepository.Verify(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.Update(task), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateUpdatedAtTimestamp()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var originalUpdatedAt = DateTime.UtcNow.AddDays(-1);
        var task = new ManagementTask
        {
            Id = taskId,
            Title = "Task",
            Status = TaskStatus.NotStarted,
            UpdatedAt = originalUpdatedAt
        };

        var command = new MarkTaskInProgressCommand { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new MarkTaskInProgressCommandHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        task.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public async Task Handle_TaskNotFound_ShouldThrowTaskNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new MarkTaskInProgressCommand { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementTask?)null);

        var handler = new MarkTaskInProgressCommandHandler(_mockRepository.Object, _mapper);

        // Act & Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldMarkCompletedTaskAsInProgress()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new ManagementTask
        {
            Id = taskId,
            Title = "Task",
            Status = TaskStatus.Completed,
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new MarkTaskInProgressCommand { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new MarkTaskInProgressCommandHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(TaskStatus.InProgress);
        task.Status.Should().Be(TaskStatus.InProgress);
    }
}

