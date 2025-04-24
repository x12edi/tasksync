using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TaskService.Core.Models;

namespace TaskService.Core.Repositories
{
    public interface ITaskRepository : IRepository<Models.Task>
    {
        Task<IEnumerable<Models.Task>> GetByAssignedToAsync(string assignedTo);
        Task<(IEnumerable<Models.Task>, int)> GetAllTasksAsync(
        int page,
        int pageSize,
        string? status,
        string? assignedTo,
        string? search,
        string? sortBy,
        string? sortOrder,
        CancellationToken cancellationToken);
        Task<Models.Task?> GetTaskByIdAsync(int id, CancellationToken cancellationToken);
        System.Threading.Tasks.Task AddTaskAsync(Models.Task task, CancellationToken cancellationToken);
        System.Threading.Tasks.Task UpdateTaskAsync(Models.Task task, CancellationToken cancellationToken);
        System.Threading.Tasks.Task DeleteTaskAsync(int id, CancellationToken cancellationToken);
    }
}
