using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Core.UnitOfWork;

using TaskService.Core.UnitOfWork;

namespace TaskService.Core.Queries;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, IEnumerable<Core.Models.Task?>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTasksQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Core.Models.Task?>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Tasks.GetAllAsync();
    }
}