using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Core.Queries
{
    public class GetAllTasksQuery : IRequest<IEnumerable<Core.Models.Task?>?>
    {
        
    }
}

