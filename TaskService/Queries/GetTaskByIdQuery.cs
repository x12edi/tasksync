using MediatR;
using TaskService.Core.Models;

namespace TaskService.Queries;

public class GetTaskByIdQuery : IRequest<Core.Models.Task?>
{
    public int Id { get; set; }
}