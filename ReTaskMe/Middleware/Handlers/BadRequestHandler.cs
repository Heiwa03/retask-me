// using Microsoft.AspNetCore.Diagnostics;
// using ReTaskMe.Middleware.Exceptions;
// using ReTaskMe.Middleware.Interfaces;

// namespace ReTaskMe.Middleware.Handlers;

// public class BadRequestHandler : IExceptionHandling{
    
//     public bool CanHandle(Exception e) => e is BadRequestException;

//     public async Task HandleAsync(HttpContext context, Exception e){
//         context.Response.StatusCode = StatusCodes.Status400BadRequest;
//         context.Response.ContentType = "application/json";
//         await context.Response.WriteAsJsonAsync(new { message = e.Message });
//     }
// }