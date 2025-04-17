using Microsoft.AspNetCore.Authorization;
using TaskService.Core.Services;
using TaskService.Core.Models;

namespace TaskService.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/tasks", async (ITaskService service) =>
            await service.GetAllTasksAsync())
            .WithName("GetTasks")
            .WithOpenApi()
            .RequireAuthorization();

        app.MapGet("/api/v1/tasks/{id}", async (int id, ITaskService service) =>
            await service.GetTaskByIdAsync(id) is Core.Models.Task task
                ? Results.Ok(task)
                : Results.NotFound())
            .WithName("GetTaskById")
            .WithOpenApi()
            .RequireAuthorization();

        app.MapPost("/api/v1/tasks", async (Core.Models.Task task, ITaskService service) =>
        {
            await service.CreateTaskAsync(task);
            return Results.Created($"/api/v1/tasks/{task.Id}", task);
        })
        .WithName("CreateTask")
        .WithOpenApi()
        .RequireAuthorization();

        app.MapPut("/api/v1/tasks/{id}", async (int id, Core.Models.Task inputTask, ITaskService service) =>
        {
            var task = await service.GetTaskByIdAsync(id);
            task.Title = inputTask.Title;
            task.Description = inputTask.Description;
            task.IsCompleted = inputTask.IsCompleted;
            task.AssignedTo = inputTask.AssignedTo;
            task.DueDate = inputTask.DueDate;
            await service.UpdateTaskAsync(task);
            return Results.NoContent();
        })
        .WithName("UpdateTask")
        .WithOpenApi()
        .RequireAuthorization();

        app.MapDelete("/api/v1/tasks/{id}", async (int id, ITaskService service) =>
        {
            await service.DeleteTaskAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteTask")
        .WithOpenApi()
        .RequireAuthorization("ManagerOnly");
    }
}