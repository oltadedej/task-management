using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Commands;
using TaskManagement.Service.Features.Tasks.Handlers;
using TaskManagement.Service.Mappings;
using Xunit;
using ManagementTask = TaskManagement.Domain.Entities.ManagementTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Unit.Commands;

/// <summary>
/// Unit tests for MarkTaskCompleteCommandHandler.
/// </summary>
public class MarkTaskCompleteCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ITaskRepository> _mockRepository;

    public MarkTaskCompleteCommandHandlerTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TaskMappingProfile>());
        _mapper = configuration.CreateMapper();

        _mockRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldMarkTaskAsCompleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new ManagementTask
        {
            Id = taskId,
            Title = "Task",
            Status = TaskStatus.InProgress,
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new MarkTaskCompleteCommand { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new MarkTaskCompleteCommandHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(TaskStatus.Completed);
        task.Status.Should().Be(TaskStatus.Completed);

        _mockRepository.Verify(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.Update(task), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TaskNotFound_ShouldThrowTaskNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new MarkTaskCompleteCommand { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementTask?)null);

        var handler = new MarkTaskCompleteCommandHandler(_mockRepository.Object, _mapper);

        // Act & Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

