using MediatR;
using TaskService.Core.Models;
using TaskService.Core.UnitOfWork;

namespace TaskService.Core.Queries;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Core.Models.Task?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Core.Models.Task?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Tasks.GetByIdAsync(request.Id);
    }
}