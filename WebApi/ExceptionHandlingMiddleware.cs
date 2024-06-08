namespace WebApi;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandlingMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "I did catch an exception.");

            await handleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task handleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = 500;

        await httpContext.Response.WriteAsync($"StatusCode = 500, Error = '{ex.Message}'");
    }
}
