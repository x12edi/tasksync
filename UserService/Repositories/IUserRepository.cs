using UserService.Models;

namespace UserService.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
