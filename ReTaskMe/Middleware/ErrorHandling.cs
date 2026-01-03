using ReTaskMe.Middleware.Interfaces;

namespace ReTaskMe.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IEnumerable<IExceptionHandling> _strategies;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        IEnumerable<IExceptionHandling> strategies,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _strategies = strategies;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context){
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(ex));

            if (strategy != null)
            {
                await strategy.HandleAsync(context, ex);
                return;
            }

            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Internal server error"
            });
        }
    }
}

