using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Service.Models.Dtos;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Integration;

/// <summary>
/// Integration tests for Tasks API endpoints.
/// </summary>
public class TasksApiTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public TasksApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetAllTasks_ShouldReturnOkWithEmptyList_WhenNoTasksExist()
    {
        // Arrange
        await CleanupDatabase();

        // Act
        var response = await _client.GetAsync("/api/v1/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);
        tasks.Should().NotBeNull();
        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateTask_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = "Integration Test Task",
            Description = "This is a test task",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdTask = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        createdTask.Should().NotBeNull();
        createdTask!.Title.Should().Be(createTaskDto.Title);
        createdTask.Description.Should().Be(createTaskDto.Description);
        createdTask.Status.Should().Be(TaskStatus.NotStarted);
        createdTask.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = string.Empty,
            Description = "Description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = "Test Task",
            Description = "Description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Act
        var response = await _client.GetAsync($"/api/v1/tasks/{createdTask!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var task = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        task.Should().NotBeNull();
        task!.Id.Should().Be(createdTask.Id);
        task.Title.Should().Be(createdTask.Title);
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        await CleanupDatabase();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/tasks/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = "Original Title",
            Description = "Original Description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        var updateTaskDto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            DueDate = DateTime.UtcNow.AddDays(14)
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/tasks/{createdTask!.Id}", updateTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTask = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        updatedTask.Should().NotBeNull();
        updatedTask!.Title.Should().Be(updateTaskDto.Title);
        updatedTask.Description.Should().Be(updateTaskDto.Description);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        await CleanupDatabase();
        var nonExistentId = Guid.NewGuid();
        var updateTaskDto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/tasks/{nonExistentId}", updateTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNoContent_WhenTaskExists()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = "Task to Delete",
            Description = "Description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/tasks/{createdTask!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify task is deleted
        var getResponse = await _client.GetAsync($"/api/v1/tasks/{createdTask.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        await CleanupDatabase();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/tasks/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MarkTaskComplete_ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = "Task to Complete",
            Description = "Description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Act
        var response = await _client.PatchAsync($"/api/v1/tasks/{createdTask!.Id}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var completedTask = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        completedTask.Should().NotBeNull();
        completedTask!.Status.Should().Be(TaskStatus.Completed);
    }

    [Fact]
    public async Task GetTasksByStatus_ShouldReturnFilteredTasks()
    {
        // Arrange
        await CleanupDatabase();

        // Create tasks with different statuses
        await _client.PostAsJsonAsync("/api/v1/tasks", new CreateTaskDto { Title = "Not Started Task" });
        var inProgressTask = await _client.PostAsJsonAsync("/api/v1/tasks", new CreateTaskDto { Title = "In Progress Task" });
        var inProgressTaskDto = await inProgressTask.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        await _client.PatchAsync($"/api/v1/tasks/{inProgressTaskDto!.Id}/complete", null);

        // Act
        var response = await _client.GetAsync("/api/v1/tasks/status/0"); // NotStarted = 0

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>(_jsonOptions);
        tasks.Should().NotBeNull();
        tasks!.Should().OnlyContain(t => t.Status == TaskStatus.NotStarted);
    }

    private async Task CleanupDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
        if (context.Tasks.Any())
        {
            context.Tasks.RemoveRange(context.Tasks);
            await context.SaveChangesAsync();
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}

