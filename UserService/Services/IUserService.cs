using System.Threading;
using System.Threading.Tasks;
using UserService.Models;
using UserService.Queries;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<string> LoginAsync(string username, string password, CancellationToken cancellationToken);
        Task<User> RegisterAsync(string username, string password, string role, CancellationToken cancellationToken);
        Task<IEnumerable<User>?> GetAllAsync(CancellationToken cancellationToken);
        Task<(List<UserDto> Users, int Total)> GetUsersAsync(int page, int pageSize, string? sortBy, string? sortOrder);
    }
}