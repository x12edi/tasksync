using MediatR;
using UserService.Models;

namespace UserService.Commands
{
    public class RegisterCommand : IRequest<User>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}