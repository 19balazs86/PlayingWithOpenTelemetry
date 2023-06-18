namespace WebApi;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await handleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task handleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = 500;

        await httpContext.Response.WriteAsync($"StatusCode = 500, Error = '{ex.Message}'");
    }
}
