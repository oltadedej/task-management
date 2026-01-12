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
    public async Task MarkTaskInProgress_ShouldReturnOk_WhenTaskExists()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = "Task to Mark In Progress",
            Description = "Description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Act
        var response = await _client.PatchAsync($"/api/v1/tasks/{createdTask!.Id}/inprogress", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var inProgressTask = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        inProgressTask.Should().NotBeNull();
        inProgressTask!.Status.Should().Be(TaskStatus.InProgress);
    }

    [Fact]
    public async Task MarkTaskInProgress_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        await CleanupDatabase();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync($"/api/v1/tasks/{nonExistentId}/inprogress", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MarkTaskInProgress_ShouldChangeStatusFromNotStartedToInProgress()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = "Not Started Task",
            Description = "Description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        createdTask!.Status.Should().Be(TaskStatus.NotStarted);

        // Act
        var response = await _client.PatchAsync($"/api/v1/tasks/{createdTask.Id}/inprogress", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var inProgressTask = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        inProgressTask!.Status.Should().Be(TaskStatus.InProgress);
    }

    [Fact]
    public async Task MarkTaskInProgress_ShouldChangeStatusFromCompletedToInProgress()
    {
        // Arrange
        await CleanupDatabase();
        var createTaskDto = new CreateTaskDto
        {
            Title = "Completed Task",
            Description = "Description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Mark as completed first
        await _client.PatchAsync($"/api/v1/tasks/{createdTask!.Id}/complete", null);

        // Act
        var response = await _client.PatchAsync($"/api/v1/tasks/{createdTask.Id}/inprogress", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var inProgressTask = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        inProgressTask!.Status.Should().Be(TaskStatus.InProgress);
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

    [Fact]
    public async Task CreateTask_WithMaxLengthTitle_ShouldSucceed()
    {
        // Arrange
        var maxTitle = new string('A', 200); // Maximum allowed length
        var createTaskDto = new CreateTaskDto
        {
            Title = maxTitle,
            Description = "Description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var task = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        task!.Title.Should().Be(maxTitle);
    }

    [Fact]
    public async Task CreateTask_WithTitleExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var tooLongTitle = new string('A', 201); // Exceeds maximum
        var createTaskDto = new CreateTaskDto
        {
            Title = tooLongTitle,
            Description = "Description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_WithMaxLengthDescription_ShouldSucceed()
    {
        // Arrange
        var maxDescription = new string('A', 1000); // Maximum allowed length
        var createTaskDto = new CreateTaskDto
        {
            Title = "Valid Title",
            Description = maxDescription
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateTask_WithDescriptionExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var tooLongDescription = new string('A', 1001); // Exceeds maximum
        var createTaskDto = new CreateTaskDto
        {
            Title = "Valid Title",
            Description = tooLongDescription
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_WithPastDueDate_ShouldFail()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Title = "Valid Title",
            Description = "Description",
            DueDate = DateTime.UtcNow.AddDays(-1) // Past date
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTasksByStatus_WithInvalidStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidStatus = 999; // Invalid status value

        // Act
        var response = await _client.GetAsync($"/api/v1/tasks/status/{invalidStatus}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_MaintainsOriginalStatus()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto { Title = "Original Task" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Mark as completed
        await _client.PatchAsync($"/api/v1/tasks/{createdTask!.Id}/complete", null);

        var updateTaskDto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        // Act
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/tasks/{createdTask.Id}", updateTaskDto);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        updatedTask!.Status.Should().Be(Domain.Enums.TaskStatus.Completed); // Status should remain Completed
        updatedTask.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task MarkTaskIncomplete_ShouldChangeStatusToNotStarted()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto { Title = "Task" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Mark as completed first
        await _client.PatchAsync($"/api/v1/tasks/{createdTask!.Id}/complete", null);

        // Act
        var response = await _client.PatchAsync($"/api/v1/tasks/{createdTask.Id}/incomplete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var task = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        task!.Status.Should().Be(Domain.Enums.TaskStatus.NotStarted);
    }

    [Fact]
    public async Task MarkTaskInProgress_ShouldChangeStatusFromAnyStatusToInProgress()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto { Title = "Task" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);

        // Verify initial status is NotStarted
        createdTask!.Status.Should().Be(Domain.Enums.TaskStatus.NotStarted);

        // Act - Mark as in progress
        var inProgressResponse = await _client.PatchAsync($"/api/v1/tasks/{createdTask.Id}/inprogress", null);

        // Assert
        inProgressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var inProgressTask = await inProgressResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        inProgressTask!.Status.Should().Be(Domain.Enums.TaskStatus.InProgress);

        // Mark as completed
        await _client.PatchAsync($"/api/v1/tasks/{createdTask.Id}/complete", null);

        // Act - Mark as in progress again from Completed status
        var inProgressFromCompletedResponse = await _client.PatchAsync($"/api/v1/tasks/{createdTask.Id}/inprogress", null);

        // Assert
        inProgressFromCompletedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var taskFromCompleted = await inProgressFromCompletedResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        taskFromCompleted!.Status.Should().Be(Domain.Enums.TaskStatus.InProgress);
    }

    [Fact]
    public async Task MarkTaskInProgress_ShouldUpdateUpdatedAtTimestamp()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto { Title = "Task" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createTaskDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        createdTask.Should().NotBeNull();
        var originalUpdatedAt = createdTask.UpdatedAt;

        // Wait a bit to ensure timestamp difference
        await Task.Delay(100);

        // Act
        var response = await _client.PatchAsync($"/api/v1/tasks/{createdTask.Id}/inprogress", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTask = await response.Content.ReadFromJsonAsync<TaskDto>(_jsonOptions);
        updatedTask.Should().NotBeNull();
        var newUpdatedAt = updatedTask!.UpdatedAt;
        newUpdatedAt.Should().BeAfter(originalUpdatedAt);
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

