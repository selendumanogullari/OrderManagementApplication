using CustomerOrders.Core.Entities.Base;
using CustomerOrders.Infrastructure.Data;
using Logging.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
    {
        if (httpContext.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(httpContext);
            return;
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        var username = httpContext.User.Identity.IsAuthenticated
            ? httpContext.User.Identity.Name
            : "Anonymous";
        var userId = httpContext.User.Identity.IsAuthenticated
            ? httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            : "Unknown";

        var authToken = httpContext.Request.Headers.ContainsKey("Authorization")
            ? httpContext.Request.Headers["Authorization"].ToString()
            : string.Empty;

        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown";

        var originalBodyStream = httpContext.Response.Body;

        using (var memoryStream = new MemoryStream())
        {
            httpContext.Response.Body = memoryStream;

            await _next(httpContext);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(memoryStream).ReadToEnd();

            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);

            stopwatch.Stop();

            var responseStatusCode = httpContext.Response.StatusCode;

            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var apiLog = new ApiCallLog
                {
                    RequestId = requestId,
                    Endpoint = httpContext.Request.Path,
                    HttpMethod = httpContext.Request.Method,
                    ClientIp = clientIp,
                    UserAgent = userAgent,
                    Username = username,
                    UserId = userId,
                    AuthToken = authToken,
                    RequestDate = DateTime.UtcNow,
                    Duration = stopwatch.Elapsed,
                    IsSuccess = true,
                    ResponseMessage = responseBody,
                    ErrorDetails = string.Empty,
                };

                context.ApiCallLog.Add(apiLog);
                await context.SaveChangesAsync();
            }
    
            var fileLogger = LoggerService.CreateLogger();
            fileLogger.Information("Request ID: {RequestId} Method: {Method} Endpoint: {Endpoint} StatusCode: {StatusCode} Duration: {Duration}ms Response: {ResponseBody}",
                requestId, httpContext.Request.Method, httpContext.Request.Path, responseStatusCode, stopwatch.Elapsed, responseBody);
        }

    }
}
