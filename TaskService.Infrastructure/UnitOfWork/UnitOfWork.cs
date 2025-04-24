using TaskService.Core.Repositories;
using TaskService.Core.UnitOfWork;
using TaskService.Infrastructure.Data;
using TaskService.Infrastructure.Repositories;

namespace TaskService.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ITaskRepository? _tasks;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ITaskRepository Tasks
    {
        get => _tasks ??= new TaskRepository(_context);
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}