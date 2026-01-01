using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext httpcontext)
        {
            try
            {
                await _next(httpcontext);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Something Went Wrong: {message}", exception.Message);
                
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                };

                httpcontext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpcontext.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    


    }
}
