﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading.Tasks;

namespace Ardalis.ListStartupServices
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ShowAllServicesMiddleware
    {
        private readonly ServiceConfig _config;
        private readonly RequestDelegate _next;

        public ShowAllServicesMiddleware(RequestDelegate next, // required for DI to work
            IOptions<ServiceConfig> config)
        {
            _config = config.Value;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == _config.Path)
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _config.Services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");

                await httpContext.Response.WriteAsync(sb.ToString());
            }
            else
            {
                await _next(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ShowAllServicesMiddlewareExtensions
    {
        public static IApplicationBuilder UseShowAllServicesMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ShowAllServicesMiddleware>();
        }
    }
}
