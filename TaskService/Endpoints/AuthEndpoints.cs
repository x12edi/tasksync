using TaskService.Core.Services;

namespace TaskService.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
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
    }
}

public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);
public record RegisterRequest(string Username, string Password, string Role);