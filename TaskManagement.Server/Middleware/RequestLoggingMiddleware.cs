namespace TaskManagement.Server.Middleware
{
    /// <summary>
    /// Middleware for logging HTTP requests
    /// </summary>
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        /// <summary>
        /// Logs incoming HTTP requests
        /// </summary>
        /// <param name="context">HTTP context</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            logger.LogInformation("Processing request: {Method} {Path} from {RemoteIpAddress}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            await next(context);

            stopwatch.Stop();

            logger.LogInformation("Completed request: {Method} {Path} with status {StatusCode} in {ElapsedMilliseconds}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}