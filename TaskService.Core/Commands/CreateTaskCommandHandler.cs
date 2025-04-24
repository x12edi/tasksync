using MediatR;
using TaskService.Core.Models;
using TaskService.Core.Services;
using TaskService.Core.UnitOfWork;

namespace TaskService.Core.Commands;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Core.Models.Task>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Core.Models.Task> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new Core.Models.Task
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            AssignedTo = request.AssignedTo,
            DueDate = request.DueDate
        };

        await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return task;
    }
}