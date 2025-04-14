namespace TaskService.Core.Services;

public interface ITaskService
{
    Task<IEnumerable<Models.Task>> GetAllTasksAsync();
    Task<Models.Task> GetTaskByIdAsync(int id);
    Task CreateTaskAsync(Models.Task task);
    Task UpdateTaskAsync(Models.Task task);
    Task DeleteTaskAsync(int id);
}