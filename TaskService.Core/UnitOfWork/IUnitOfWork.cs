using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Core.Repositories;

namespace TaskService.Core.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository Tasks { get; }
        Task<int> CompleteAsync(CancellationToken cancellationToken);
    }
}
