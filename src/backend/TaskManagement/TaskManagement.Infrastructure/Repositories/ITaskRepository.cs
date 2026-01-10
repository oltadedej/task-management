using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Task entities.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Gets all tasks asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of tasks.</returns>
    Task<IEnumerable<ManagementTask>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tasks filtered by status asynchronously.
    /// </summary>
    /// <param name="status">The status to filter by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of tasks matching the status.</returns>
    Task<IEnumerable<ManagementTask>> GetByStatusAsync(TaskStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a task by ID asynchronously.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task if found, otherwise null.</returns>
    Task<ManagementTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new task asynchronously.
    /// </summary>
    /// <param name="task">The task to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added task.</returns>
    Task<ManagementTask> AddAsync(ManagementTask task, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="task">The task to update.</param>
    void Update(ManagementTask task);

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="task">The task to delete.</param>
    void Delete(ManagementTask task);

    /// <summary>
    /// Saves all changes made in the context asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

