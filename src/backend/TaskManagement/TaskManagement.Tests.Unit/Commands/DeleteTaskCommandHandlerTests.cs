using FluentAssertions;
using Moq;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Commands;
using TaskManagement.Service.Features.Tasks.Handlers;
using Xunit;
using ManagementTask = TaskManagement.Domain.Entities.ManagementTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Unit.Commands;

/// <summary>
/// Unit tests for DeleteTaskCommandHandler.
/// </summary>
public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockRepository;

    public DeleteTaskCommandHandlerTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldDeleteTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var taskToDelete = new ManagementTask
        {
            Id = taskId,
            Title = "Task to Delete",
            Status = TaskStatus.Completed
        };

        var command = new DeleteTaskCommand { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskToDelete);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DeleteTaskCommandHandler(_mockRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        _mockRepository.Verify(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.Delete(taskToDelete), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TaskNotFound_ShouldThrowTaskNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand { Id = taskId };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementTask?)null);

        var handler = new DeleteTaskCommandHandler(_mockRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() => handler.Handle(command, CancellationToken.None));

        _mockRepository.Verify(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.Delete(It.IsAny<ManagementTask>()), Times.Never);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

