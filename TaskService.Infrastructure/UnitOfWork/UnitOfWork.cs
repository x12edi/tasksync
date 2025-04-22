using TaskService.Core.Repositories;
using TaskService.Core.UnitOfWork;
using TaskService.Infrastructure.Data;
using TaskService.Infrastructure.Repositories;

namespace TaskService.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ITaskRepository? _tasks;
    //private IUserRepository? _users;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);
    //public IUserRepository Users => _users ??= new UserRepository(_context);

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}