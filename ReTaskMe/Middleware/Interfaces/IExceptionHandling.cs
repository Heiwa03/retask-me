namespace ReTaskMe.Middleware.Interfaces;

public interface IExceptionHandling{
    bool CanHandle(Exception e);
    Task HandleAsync(HttpContext context, Exception e);
}