using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagement.Service.Models.Dtos;
using Xunit;

namespace TaskManagement.Tests.Integration;

/// <summary>
/// Integration tests for edge cases and boundary conditions.
/// </summary>
public class EdgeCasesTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public EdgeCasesTests(CustomWebApplicationFactory factory)
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

    public void Dispose()
    {
        _client.Dispose();
    }
}

