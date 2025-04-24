using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Core.Queries
{
    public class GetAllTasksQuery : IRequest<(IEnumerable<Core.Models.Task?>?, int)>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Status { get; set; }
        public string? AssignedTo { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}

