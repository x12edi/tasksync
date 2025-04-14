using System.Globalization;
using TaskService.Core.Repositories;
using TaskService.Core.UnitOfWork;

namespace TaskService.Core.Services{

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Models.Task>> GetAllTasksAsync()
    {
            //Console.WriteLine($"Invariant Mode: {CultureInfo.InvariantCulture.Name}");
            //Console.WriteLine($"Current Culture: {CultureInfo.CurrentCulture.Name}");
            return await _unitOfWork.Tasks.GetAllAsync();
    }

    public async Task<Models.Task> GetTaskByIdAsync(int id)
    {
        return await _unitOfWork.Tasks.GetByIdAsync(id);
    }

    public async System.Threading.Tasks.Task CreateTaskAsync(Models.Task task)
    {
        await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.CompleteAsync();
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(Models.Task task)
    {
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.CompleteAsync();
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        _unitOfWork.Tasks.Delete(task);
        await _unitOfWork.CompleteAsync();
    }
}
}
