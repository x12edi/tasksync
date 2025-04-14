using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TaskService.Core.Models;
using Task = TaskService.Core.Models.Task;

namespace TaskService.Core.Repositories
{
    public interface ITaskRepository : IRepository<Task>
    {
        Task<IEnumerable<Task>> GetByAssignedToAsync(string assignedTo);
    }
}
