using System.Net;
using System.Text.Json;

namespace TaskService.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";

        switch (exception)
        {
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            case InvalidOperationException:
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
        }

        var result = JsonSerializer.Serialize(new { error = message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}