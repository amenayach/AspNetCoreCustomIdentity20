using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CustomIdentity.Middlewares
{
    public class OptionsMiddleware
    {
        private readonly RequestDelegate _next;

        public OptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Console.WriteLine($"Request for {httpContext.Request.Path} received ({httpContext.Request.ContentLength ?? 0} bytes)");

            if (httpContext.Request.Headers["Origin"].Any())
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", httpContext.Request.Headers["Origin"].First());
                httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            }

            if (httpContext.Request.Method.ToLower() == "options")
            {
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                httpContext.Response.Headers.Clear();

                if (httpContext.Request.Headers["Origin"].Any())
                {
                    httpContext.Response.Headers.Add("Access-Control-Allow-Origin", httpContext.Request.Headers["Origin"].First());
                    httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                }

                httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,DELETE");
                return;
            }

            // Call the next middleware delegate in the pipeline 
            await _next.Invoke(httpContext);
        }

    }

    public static class OptionsMiddlewareExtensions
    {
        public static IApplicationBuilder UseOptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OptionsMiddleware>();
        }
    }
}
