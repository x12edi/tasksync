using MediatR;
using System.Collections.Generic;

namespace UserService.Queries
{
    public class GetUsersInternalQuery : IRequest<(List<UserDto> Users, int Total)>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}