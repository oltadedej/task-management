using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using ManagementTask = TaskManagement.Domain.Entities.ManagementTask;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for TaskRepository using in-memory database.
/// </summary>
public class TaskRepositoryTests : IDisposable
{
    private readonly TaskManagementDbContext _context;
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TaskManagementDbContext(options);
        _repository = new TaskRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTasks()
    {
        // Arrange
        var tasks = new List<ManagementTask>
        {
            new ManagementTask { Id = Guid.NewGuid(), Title = "Task 1", Status = TaskStatus.NotStarted, CreatedAt = DateTime.UtcNow.AddDays(-2), UpdatedAt = DateTime.UtcNow },
            new ManagementTask { Id = Guid.NewGuid(), Title = "Task 2", Status = TaskStatus.Completed, CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow }
        };

        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Title == "Task 1");
        result.Should().Contain(t => t.Title == "Task 2");
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new ManagementTask
        {
            Id = taskId,
            Title = "Test Task",
            Status = TaskStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.Title.Should().Be("Test Task");
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ShouldReturnNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(taskId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnFilteredTasks()
    {
        // Arrange
        var tasks = new List<ManagementTask>
        {
            new ManagementTask { Id = Guid.NewGuid(), Title = "Not Started Task", Status = TaskStatus.NotStarted, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ManagementTask { Id = Guid.NewGuid(), Title = "Completed Task", Status = TaskStatus.Completed, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ManagementTask { Id = Guid.NewGuid(), Title = "Another Completed Task", Status = TaskStatus.Completed, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatusAsync(TaskStatus.Completed);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.Status == TaskStatus.Completed);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTask()
    {
        // Arrange
        var task = new ManagementTask
        {
            Id = Guid.NewGuid(),
            Title = "New Task",
            Status = TaskStatus.NotStarted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.AddAsync(task);
        await _repository.SaveChangesAsync();

        // Assert
        var result = await _context.Tasks.FindAsync(task.Id);
        result.Should().NotBeNull();
        result!.Title.Should().Be("New Task");
    }

    [Fact]
    public async Task Update_ShouldUpdateTask()
    {
        // Arrange
        var task = new ManagementTask
        {
            Id = Guid.NewGuid(),
            Title = "Original Title",
            Status = TaskStatus.NotStarted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        task.Title = "Updated Title";
        _repository.Update(task);
        await _repository.SaveChangesAsync();

        // Assert
        var result = await _context.Tasks.FindAsync(task.Id);
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task Delete_ShouldDeleteTask()
    {
        // Arrange
        var task = new ManagementTask
        {
            Id = Guid.NewGuid(),
            Title = "Task to Delete",
            Status = TaskStatus.NotStarted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        _repository.Delete(task);
        await _repository.SaveChangesAsync();

        // Assert
        var result = await _context.Tasks.FindAsync(task.Id);
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

