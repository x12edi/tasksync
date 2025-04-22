namespace TaskService.Core.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }

    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
}
