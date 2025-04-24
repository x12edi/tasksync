using System.Globalization;
using TaskService.Core.Repositories;
using TaskService.Core.UnitOfWork;
using TaskService.Core.Models;
using TaskService.Core.Commands;
using TaskService.Core.Queries;
using TaskService.Core.Services;
using MediatR;

namespace TaskService.Core.Services
{

    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<Models.Task>, int)> GetAllTasksAsync(
            int page,
            int pageSize,
            string? status,
            string? assignedTo,
            string? search,
            string? sortBy,
            string? sortOrder,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.Tasks.GetAllTasksAsync(
                page,
                pageSize,
                status,
                assignedTo,
                search,
                sortBy,
                sortOrder,
                cancellationToken);
        }

        public async Task<Core.Models.Task?> GetTaskByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Tasks.GetTaskByIdAsync(id, cancellationToken);
        }

        public async Task<Models.Task> CreateTaskAsync(Core.Models.Task task, CancellationToken cancellationToken)
        {
            await _unitOfWork.Tasks.AddTaskAsync(task, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return task;
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(Models.Task task, CancellationToken cancellationToken)
        {
            await _unitOfWork.Tasks.UpdateTaskAsync(task, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task DeleteTaskAsync(int id, CancellationToken cancellationToken)
        {
            await _unitOfWork.Tasks.DeleteTaskAsync(id, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
