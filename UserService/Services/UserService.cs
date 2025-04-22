using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UserService.Commands;
using UserService.Models;
using UserService.Queries;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IMediator _mediator;

        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<string> LoginAsync(string username, string password, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new LoginCommand { Username = username, Password = password }, cancellationToken);
        }

        public async Task<User> RegisterAsync(string username, string password, string role, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new RegisterCommand { Username = username, Password = password, Role = role }, cancellationToken);
        }

        public async Task<IEnumerable<User>?> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
        }
    }
}