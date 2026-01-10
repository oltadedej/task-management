using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Task entities.
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ManagementTask>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ManagementTask>> GetByStatusAsync(TaskStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ManagementTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ManagementTask> AddAsync(ManagementTask task, CancellationToken cancellationToken = default)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        await _context.Tasks.AddAsync(task, cancellationToken);
        return task;
    }

    /// <inheritdoc />
    public void Update(ManagementTask task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        _context.Tasks.Update(task);
    }

    /// <inheritdoc />
    public void Delete(ManagementTask task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        _context.Tasks.Remove(task);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

