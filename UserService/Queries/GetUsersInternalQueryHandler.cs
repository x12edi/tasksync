using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.UnitOfWork;

namespace UserService.Queries
{
    public class GetUsersInternalQueryHandler : IRequestHandler<GetUsersInternalQuery, (List<UserDto> Users, int Total)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetUsersInternalQueryHandler> _logger;

        public GetUsersInternalQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUsersInternalQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<(List<UserDto> Users, int Total)> Handle(GetUsersInternalQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching users: page={Page}, pageSize={PageSize}, sortBy={SortBy}, sortOrder={SortOrder}",
                request.Page, request.PageSize, request.SortBy, request.SortOrder);

            var query = _unitOfWork.Users.GetAll();

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "username" => request.SortOrder == "desc" ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
                    "role" => request.SortOrder == "desc" ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
                    _ => query
                };
            }

            var total = await query.CountAsync(cancellationToken);
            var users = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched {Count} users, total={Total}", users.Count, total);

            return (users, total);
        }
    }
}