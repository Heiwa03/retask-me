using BusinessLogicLayerCore.Exceptions;
using ReTaskMe.Middleware.Interfaces;

namespace ReTaskMe.Middleware.Handlers;

public class UsernameExistException : IExceptionHandling
{
    public bool CanHandle(Exception e) => e is InvalidCredentialsException;

    public async Task HandleAsync(HttpContext context, Exception e)
    {
        context.Response.StatusCode = StatusCodes.Status406NotAcceptable; // TODO: To choose correct status for Username that already exists
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = e.Message });
    }
}
