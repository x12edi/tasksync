using Microsoft.EntityFrameworkCore;
using TaskService.Core.Models;
using TaskService.Core.Repositories;
using TaskService.Infrastructure.Data;

namespace TaskService.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }
}