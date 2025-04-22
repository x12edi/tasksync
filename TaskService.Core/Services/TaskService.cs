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
        private readonly IMediator _mediator;

        public TaskService(IMediator mediator)
        {
            _mediator = mediator;
        }

        //public async Task<IEnumerable<Models.Task>> GetAllTasksAsync()
        //{
        //    //Console.WriteLine($"Invariant Mode: {CultureInfo.InvariantCulture.Name}");
        //    //Console.WriteLine($"Current Culture: {CultureInfo.CurrentCulture.Name}");
        //    return await _unitOfWork.Tasks.GetAllAsync();
        //}

        public async Task<IEnumerable<Models.Task?>> GetAllTasksAsync()
        {
            // Implement GetAllTasksQuery similarly
            return await _mediator.Send(new GetAllTasksQuery());
        }

        //public async Task<Models.Task> GetTaskByIdAsync(int id)
        //{
        //    return await _unitOfWork.Tasks.GetByIdAsync(id);
        //}

        public async Task<Models.Task?> GetTaskByIdAsync(int id)
        {
            return await _mediator.Send(new GetTaskByIdQuery { Id = id });
        }

        //public async System.Threading.Tasks.Task CreateTaskAsync(Models.Task task)
        //{
        //    await _unitOfWork.Tasks.AddAsync(task);
        //    await _unitOfWork.CompleteAsync();
        //}

        public async System.Threading.Tasks.Task CreateTaskAsync(TaskCreateDto dto)
        {
            var command = new CreateTaskCommand
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted,
                AssignedTo = dto.AssignedTo,
                DueDate = dto.DueDate
            };
            await _mediator.Send(command);
        }

        //public async System.Threading.Tasks.Task UpdateTaskAsync(Models.Task task)
        //{
        //    _unitOfWork.Tasks.Update(task);
        //    await _unitOfWork.CompleteAsync();
        //}

        public async System.Threading.Tasks.Task UpdateTaskAsync(Models.Task task)
        {
            // Implement UpdateTaskCommand
            throw new NotImplementedException();
        }
        //public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
        //{
        //    var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        //    _unitOfWork.Tasks.Delete(task);
        //    await _unitOfWork.CompleteAsync();
        //}

        public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
        {
            // Implement DeleteTaskCommand
            throw new NotImplementedException();
        }
    }
}
