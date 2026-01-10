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
/// Unit tests for UpdateTaskCommandHandler.
/// </summary>
public class UpdateTaskCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ITaskRepository> _mockRepository;

    public UpdateTaskCommandHandlerTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TaskMappingProfile>());
        _mapper = configuration.CreateMapper();

        _mockRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new ManagementTask
        {
            Id = taskId,
            Title = "Original Title",
            Description = "Original Description",
            Status = TaskStatus.InProgress,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Description = "Updated Description",
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateTaskCommandHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Description.Should().Be(command.Description);
        result.DueDate.Should().Be(command.DueDate);
        result.Status.Should().Be(TaskStatus.InProgress); // Status should not change

        _mockRepository.Verify(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.Update(It.IsAny<ManagementTask>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TaskNotFound_ShouldThrowTaskNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Description = "Updated Description"
        };

        _mockRepository
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementTask?)null);

        var handler = new UpdateTaskCommandHandler(_mockRepository.Object, _mapper);

        // Act & Assert
        await Assert.ThrowsAsync<TaskNotFoundException>(() => handler.Handle(command, CancellationToken.None));

        _mockRepository.Verify(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.Update(It.IsAny<ManagementTask>()), Times.Never);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

