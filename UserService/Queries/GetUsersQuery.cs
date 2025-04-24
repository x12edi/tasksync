using MediatR;
using System.Collections.Generic;

namespace UserService.Queries
{
    public class GetUsersQuery : IRequest<GetUsersQueryResponse>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
    }

    public class GetUsersQueryResponse
    {
        public List<UserDto> Users { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}