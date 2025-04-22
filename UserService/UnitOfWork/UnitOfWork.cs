using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using UserService.Repositories;

namespace UserService.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IUserRepository Users { get; }

        public UnitOfWork(AppDbContext context, IUserRepository userRepository)
        {
            _context = context;
            Users = userRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}