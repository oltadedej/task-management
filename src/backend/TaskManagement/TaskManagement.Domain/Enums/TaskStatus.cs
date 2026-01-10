namespace TaskManagement.Domain.Enums;

/// <summary>
/// Represents the status of a task.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task has not been started.
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Task is currently in progress.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Task has been completed.
    /// </summary>
    Completed = 2
}

