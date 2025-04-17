using TaskService.Core.Models;

namespace TaskService.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}