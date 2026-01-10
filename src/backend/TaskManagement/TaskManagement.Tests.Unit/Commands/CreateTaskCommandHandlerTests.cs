using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Service.Features.Tasks.Commands;
using TaskManagement.Service.Features.Tasks.Handlers;
using TaskManagement.Service.Mappings;
using TaskManagement.Service.Models.Dtos;
using Xunit;
using ManagementTask = TaskManagement.Domain.Entities.ManagementTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Unit.Commands;

/// <summary>
/// Unit tests for CreateTaskCommandHandler.
/// </summary>
public class CreateTaskCommandHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ITaskRepository> _mockRepository;

    public CreateTaskCommandHandlerTests()
    {
        // Setup AutoMapper
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<TaskMappingProfile>());
        _mapper = configuration.CreateMapper();

        _mockRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateTask()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Test Description",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        ManagementTask? capturedTask = null;
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ManagementTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementTask task, CancellationToken ct) =>
            {
                task.Id = Guid.NewGuid();
                capturedTask = task;
                return task;
            });

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateTaskCommandHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Description.Should().Be(command.Description);
        result.Status.Should().Be(TaskStatus.NotStarted);
        result.Id.Should().NotBeEmpty();

        _mockRepository.Verify(x => x.AddAsync(It.IsAny<ManagementTask>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        capturedTask.Should().NotBeNull();
        capturedTask!.Status.Should().Be(TaskStatus.NotStarted);
        capturedTask.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_CommandWithNullDescription_ShouldCreateTask()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = null,
            DueDate = null
        };

        ManagementTask? capturedTask = null;
        _mockRepository
            .Setup(x => x.AddAsync(It.IsAny<ManagementTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ManagementTask task, CancellationToken ct) =>
            {
                task.Id = Guid.NewGuid();
                capturedTask = task;
                return task;
            });

        _mockRepository
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateTaskCommandHandler(_mockRepository.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Description.Should().BeNull();
        result.DueDate.Should().BeNull();
    }
}

