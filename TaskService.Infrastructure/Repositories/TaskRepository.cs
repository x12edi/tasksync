using Microsoft.EntityFrameworkCore;
using TaskService.Core.Repositories;
using TaskService.Infrastructure.Data;

namespace TaskService.Infrastructure.Repositories;

public class TaskRepository : Repository<Core.Models.Task>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Core.Models.Task>> GetByAssignedToAsync(string assignedTo)
    {
        return await _context.Tasks
            .Where(t => t.AssignedTo == assignedTo)
            .ToListAsync();
    }
}