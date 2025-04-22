using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using UserService.Services;

namespace UserService.Endpoints
{
    public static class AuthEndpoints
    {
        public record LoginRequest(string Username, string Password);
        public record RegisterRequest(string Username, string Password, string Role);

        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapGet("/api/v1/auth/users", async (IUserService service, CancellationToken cancellationToken) =>
            {
                try
                {
                    var users = await service.GetAllAsync(cancellationToken);
                    return Results.Ok(users ?? Enumerable.Empty<Models.User>());
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: $"An error occurred while logging in: {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            })
            .WithName("GetUsers")
            .WithOpenApi()
            .RequireAuthorization(); 

            app.MapPost("/api/v1/auth/login", async (LoginRequest request, IUserService service, CancellationToken cancellationToken) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                        return Results.BadRequest("Username and password are required.");

                    var token = await service.LoginAsync(request.Username, request.Password, cancellationToken);
                    return Results.Ok(new { Token = token });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: $"An error occurred while logging in: {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            })
            .WithName("Login")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Logs in a user";
                operation.Description = "Authenticates a user with username and password, returning a JWT token.";
                operation.Responses["200"] = new OpenApiResponse
                {
                    Description = "Login successful",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["token"] = new OpenApiSchema { Type = "string" }
                                }
                            }
                        }
                    }
                };
                operation.Responses["400"] = new OpenApiResponse { Description = "Invalid input" };
                operation.Responses["401"] = new OpenApiResponse { Description = "Unauthorized" };
                operation.Responses["500"] = new OpenApiResponse { Description = "Server error" };
                return operation;
            });

            app.MapPost("/api/v1/auth/register", async (RegisterRequest request, IUserService service, CancellationToken cancellationToken) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Role))
                        return Results.BadRequest("Username, password, and role are required.");

                    var user = await service.RegisterAsync(request.Username, request.Password, request.Role, cancellationToken);
                    return Results.Created($"/api/v1/users/{user.Id}", new { user.Id, user.Username, user.Role });
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: $"An error occurred while registering: {ex.Message}",
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            })
            .WithName("Register")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Registers a new user";
                operation.Description = "Creates a new user account with the provided username, password, and role.";
                operation.Responses["201"] = new OpenApiResponse
                {
                    Description = "User created successfully",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["id"] = new OpenApiSchema { Type = "integer" },
                                    ["username"] = new OpenApiSchema { Type = "string" },
                                    ["role"] = new OpenApiSchema { Type = "string" }
                                }
                            }
                        }
                    }
                };
                operation.Responses["400"] = new OpenApiResponse { Description = "Invalid input" };
                operation.Responses["409"] = new OpenApiResponse { Description = "Username already exists" };
                operation.Responses["500"] = new OpenApiResponse { Description = "Server error" };
                return operation;
            });
        }
    }
}