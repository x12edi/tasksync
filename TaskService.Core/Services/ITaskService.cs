namespace TaskService.Core.Services;

public interface ITaskService
{
    Task<(IEnumerable<Core.Models.Task>, int)> GetAllTasksAsync(
        int page,
        int pageSize,
        string? status,
        string? assignedTo,
        string? search,
        string? sortBy,
        string? sortOrder,
        CancellationToken cancellationToken);
    Task<Core.Models.Task?> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Core.Models.Task> CreateTaskAsync(Core.Models.Task task, CancellationToken cancellationToken = default);
    Task UpdateTaskAsync(Core.Models.Task task, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
}