using Microsoft.EntityFrameworkCore;
using TaskService.Core.UnitOfWork;
using TaskService.Infrastructure.Data;
using TaskService.Infrastructure.UnitOfWork;
using TaskService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using TaskService.Core.Models;
using TaskService.Core.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

// Add EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<TaskService.Core.Services.ITaskService, TaskService.Core.Services.TaskService>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

var app = builder.Build();

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/v1/tasks", async (ITaskService service) =>
    await service.GetAllTasksAsync())
    .WithName("GetTasks")
    .WithOpenApi();

app.MapGet("/api/v1/tasks/{id}", async (int id, ITaskService service) =>
    await service.GetTaskByIdAsync(id) is TaskService.Core.Models.Task task
        ? Results.Ok(task)
        : Results.NotFound())
    .WithName("GetTaskById")
    .WithOpenApi();

app.MapPost("/api/v1/tasks", async (TaskService.Core.Models.Task task, ITaskService service) =>
{
    await service.CreateTaskAsync(task);
    return Results.Created($"/api/v1/tasks/{task.Id}", task);
})
.WithName("CreateTask")
.WithOpenApi();

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
.WithOpenApi();

app.MapDelete("/api/v1/tasks/{id}", async (int id, ITaskService service) =>
{
    await service.DeleteTaskAsync(id);
    return Results.NoContent();
})
.WithName("DeleteTask")
.WithOpenApi();

app.Run();