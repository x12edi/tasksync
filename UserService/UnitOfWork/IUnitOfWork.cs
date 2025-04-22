using UserService.Repositories;

namespace UserService.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

