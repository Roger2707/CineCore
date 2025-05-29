using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Shared.Middlewares
{
    public class TenantAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
            var cinemaIdClaim = context.User.FindFirst("CinemaId")?.Value;

            if (string.IsNullOrEmpty(userRole))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing role claim");
                return;
            }

            var path = context.Request.Path.Value.ToLower();

            if (path.StartsWith("/api/movies") && userRole != "SuperAdmin" && userRole != "Admin")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Insufficient role");
                return;
            }

            await _next(context);
        }
    }
}
