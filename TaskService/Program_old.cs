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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;


var builder = WebApplication.CreateBuilder(args);

//CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
//CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

// Add EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<TaskService.Core.Services.ITaskService, TaskService.Core.Services.TaskService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
})
.AddOpenIdConnect("AzureAD", options =>
{
    options.Authority = builder.Configuration["AzureAD:Authority"];
    options.ClientId = builder.Configuration["AzureAD:ClientId"];
    options.ClientSecret = builder.Configuration["AzureAD:ClientSecret"];
    options.ResponseType = "code";
    options.CallbackPath = "/signin-oidc";
    options.SaveTokens = true;
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TaskSync API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/v1/tasks", async (ITaskService service) =>
    await service.GetAllTasksAsync())
    .WithName("GetTasks")
    .WithOpenApi()
    .RequireAuthorization(); 

app.MapGet("/api/v1/tasks/{id}", async (int id, ITaskService service) =>
    await service.GetTaskByIdAsync(id) is TaskService.Core.Models.Task task
        ? Results.Ok(task)
        : Results.NotFound())
    .WithName("GetTaskById")
    .WithOpenApi()
    .RequireAuthorization();

app.MapPost("/api/v1/tasks", async (TaskService.Core.Models.Task task, ITaskService service) =>
{
    await service.CreateTaskAsync(task);
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

app.MapPost("/api/v1/auth/login", async (LoginRequest request, IUserService service) =>
{
    try
    {
        var token = await service.LoginAsync(request.Username, request.Password);
        return Results.Ok(new { Token = token });
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Unauthorized();
    }
})
.WithName("Login")
.WithOpenApi();

app.MapPost("/api/v1/auth/refresh", async (RefreshRequest request, IUserService service) =>
{
    try
    {
        var newToken = await service.RefreshTokenAsync(request.RefreshToken);
        return Results.Ok(new { Token = newToken });
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Unauthorized();
    }
})
.WithName("RefreshToken")
.WithOpenApi();

app.MapPost("/api/v1/auth/register", async (RegisterRequest request, IUserService service) =>
{
    try
    {
        await service.RegisterAsync(request.Username, request.Password, request.Role);
        return Results.Created();
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("Register")
.WithOpenApi();

app.Run();

// DTOs
public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);
public record RegisterRequest(string Username, string Password, string Role);