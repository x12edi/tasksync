using TaskService.Core.Models;

namespace TaskService.Core.Services;

public interface IUserService
{
    Task<string> LoginAsync(string username, string password);
    Task<string> RefreshTokenAsync(string refreshToken);
    System.Threading.Tasks.Task RegisterAsync(string username, string password, string role);
}