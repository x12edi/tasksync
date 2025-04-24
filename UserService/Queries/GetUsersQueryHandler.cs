using MediatR;
using System.Threading;
using System.Threading.Tasks;
using UserService.Services;

namespace UserService.Queries
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersQueryResponse>
    {
        private readonly IUserService _userService;

        public GetUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetUsersQueryResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var (users, total) = await _userService.GetUsersAsync(
                request.Page,
                request.PageSize,
                request.SortBy,
                request.SortOrder
            );

            return new GetUsersQueryResponse
            {
                Users = users,
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}