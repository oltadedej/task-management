namespace TaskManagement.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid task operation is attempted.
/// </summary>
public class InvalidTaskOperationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidTaskOperationException"/> class.
    /// </summary>
    public InvalidTaskOperationException()
        : base("Invalid task operation.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidTaskOperationException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidTaskOperationException(string message)
        : base(message)
    {
    }
}

