using Microsoft.AspNetCore.Authorization;
using TaskService.Core.Services;
using TaskService.Core.Models;
using TaskService.Core.Commands;
using TaskService.Core.Queries;
using MediatR;

namespace TaskService.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        //app.MapGet("/api/v1/tasks", async (ITaskService service) =>
        //    await service.GetAllTasksAsync())
        //    .WithName("GetTasks")
        //    .WithOpenApi()
        //    .RequireAuthorization();

        app.MapGet("/api/v1/tasks", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            try
            {
                var tasks = await mediator.Send(new GetAllTasksQuery(), cancellationToken);
                return Results.Ok(tasks ?? Enumerable.Empty<Core.Models.Task>());
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: "An error occurred while retrieving tasks:" + ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        })
        .WithName("GetTasks")
        .WithOpenApi()
        .RequireAuthorization();

        app.MapGet("/api/v1/tasks/{id}", async (int id, IMediator mediator) =>
            await mediator.Send(new GetTaskByIdQuery { Id = id }) is Core.Models.Task task
                ? Results.Ok(task)
                : Results.NotFound())
            .WithName("GetTaskById")
            .WithOpenApi()
            .RequireAuthorization();

        app.MapPost("/api/v1/tasks", async (TaskCreateDto dto, IMediator mediator) =>
        {
            var task = await mediator.Send(new CreateTaskCommand
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted,
                AssignedTo = dto.AssignedTo,
                DueDate = dto.DueDate
            });
            return Results.Created($"/api/v1/tasks/{task.Id}", task);
        })
        .WithName("CreateTask")
        .WithOpenApi()
        .RequireAuthorization();

        app.MapPut("/api/v1/tasks/{id}", async (int id, TaskService.Core.Models.Task inputTask, ITaskService service) =>
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