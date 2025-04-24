using Microsoft.EntityFrameworkCore;
using TaskService.Core.Repositories;
using TaskService.Infrastructure.Data;

namespace TaskService.Infrastructure.Repositories;

public class TaskRepository : Repository<Core.Models.Task>, ITaskRepository
{
    private static readonly HashSet<string> _validSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "id",
        "title",
        "description",
        "isCompleted",
        "dueDate",
        "assignedTo"
    };

    public TaskRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Core.Models.Task>> GetByAssignedToAsync(string assignedTo)
    {
        return await _context.Tasks
            .Where(t => t.AssignedTo == assignedTo)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Core.Models.Task>, int)> GetAllTasksAsync(
        int page,
        int pageSize,
        string? status,
        string? assignedTo,
        string? search,
        string? sortBy,
        string? sortOrder,
        CancellationToken cancellationToken)
    {
        var query = _context.Tasks.AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(status))
        {
            bool isCompleted = status.ToLower() == "completed";
            query = query.Where(t => t.IsCompleted == isCompleted);
        }
        if (!string.IsNullOrEmpty(assignedTo))
        {
            query = query.Where(t => t.AssignedTo == assignedTo);
        }
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t => t.Title.Contains(search) || t.Description.Contains(search));
        }

        // Sorting
        bool isAscending = sortOrder?.ToLower() == "asc";
        bool isValidSort = !string.IsNullOrEmpty(sortBy) && _validSortFields.Contains(sortBy.ToLower());

        Console.WriteLine($"Sorting by: {sortBy}, Order: {sortOrder}, Valid: {isValidSort}");

        if (isValidSort)
        {
            switch (sortBy.ToLower())
            {
                case "id":
                    query = isAscending ? query.OrderBy(t => t.Id) : query.OrderByDescending(t => t.Id);
                    break;
                case "title":
                    query = isAscending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title);
                    break;
                case "description":
                    query = isAscending ? query.OrderBy(t => t.Description) : query.OrderByDescending(t => t.Description);
                    break;
                case "iscompleted":
                    query = isAscending ? query.OrderBy(t => t.IsCompleted) : query.OrderByDescending(t => t.IsCompleted);
                    break;
                case "duedate":
                    query = isAscending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate);
                    break;
                case "assignedto":
                    query = isAscending ? query.OrderBy(t => t.AssignedTo) : query.OrderByDescending(t => t.AssignedTo);
                    break;
                default:
                    query = query.OrderBy(t => t.Id);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(t => t.Id);
        }

        // Total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Paging
        var tasks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (tasks, totalCount);
    }

    public async Task<Core.Models.Task?> GetTaskByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddTaskAsync(Core.Models.Task task, CancellationToken cancellationToken)
    {
        await _context.Tasks.AddAsync(task, cancellationToken);
    }

    public async Task UpdateTaskAsync(Core.Models.Task task, CancellationToken cancellationToken)
    {
        _context.Tasks.Update(task);
        await Task.CompletedTask;
    }

    public async Task DeleteTaskAsync(int id, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (task != null)
        {
            _context.Tasks.Remove(task);
        }
    }
}