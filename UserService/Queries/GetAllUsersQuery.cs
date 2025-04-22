using MediatR;

namespace UserService.Queries
{
    public class GetAllUsersQuery : IRequest<IEnumerable<Models.User>?>
    {
    }
}
