using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Core.Services;
using TaskService.Core.UnitOfWork;

using TaskService.Core.UnitOfWork;

namespace TaskService.Core.Queries;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, (IEnumerable<Core.Models.Task?>, int)>
{
    //private readonly IUnitOfWork _unitOfWork;

    //public GetAllTasksQueryHandler(IUnitOfWork unitOfWork)
    //{
    //    _unitOfWork = unitOfWork;
    //}

    //public async Task<IEnumerable<Core.Models.Task?>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    //{
    //    return await _unitOfWork.Tasks.GetAllAsync();
    //}

    private readonly ITaskService _taskService;

    public GetAllTasksQueryHandler(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public async Task<(IEnumerable<Core.Models.Task>, int)> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        return await _taskService.GetAllTasksAsync(
            request.Page,
            request.PageSize,
            request.Status,
            request.AssignedTo,
            request.Search,
            request.SortBy,
            request.SortOrder,
            cancellationToken);
    }
}