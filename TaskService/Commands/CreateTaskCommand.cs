using MediatR;
using TaskService.Core.Models;

namespace TaskService.Commands;

public class CreateTaskCommand : IRequest<Core.Models.Task>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}