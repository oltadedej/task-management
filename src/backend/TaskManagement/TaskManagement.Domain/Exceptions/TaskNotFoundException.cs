namespace TaskManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown when a task is not found.
/// </summary>
public class TaskNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TaskNotFoundException"/> class.
    /// </summary>
    public TaskNotFoundException()
        : base("Task not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskNotFoundException"/> class with a specific task ID.
    /// </summary>
    /// <param name="taskId">The ID of the task that was not found.</param>
    public TaskNotFoundException(Guid taskId)
        : base($"Task with ID {taskId} was not found.")
    {
        TaskId = taskId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskNotFoundException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public TaskNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Gets the task ID that was not found.
    /// </summary>
    public Guid? TaskId { get; }
}

