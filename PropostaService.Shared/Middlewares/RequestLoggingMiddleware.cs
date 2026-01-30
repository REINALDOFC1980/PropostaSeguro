// Middlewares/RequestLoggingMiddleware.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace PropostaService.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            Log.Information("Incoming Request: {Method} {Path}", request.Method, request.Path);

            await _next(context);

            stopwatch.Stop();
            Log.Information("Response: {StatusCode} processed in {ElapsedMilliseconds} ms",
                context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
