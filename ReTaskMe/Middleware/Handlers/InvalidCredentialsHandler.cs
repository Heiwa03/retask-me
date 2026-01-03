using BusinessLogicLayerCore.Exceptions;
using ReTaskMe.Middleware.Interfaces;

namespace ReTaskMe.Middleware.Handlers;

public class InvalidCredentialsHandler : IExceptionHandling
{
    public bool CanHandle(Exception e) => e is InvalidCredentialsException;

    public async Task HandleAsync(HttpContext context, Exception e)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = e.Message });
    }
}
