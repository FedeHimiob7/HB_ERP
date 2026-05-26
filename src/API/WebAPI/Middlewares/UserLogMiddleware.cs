using HB_ERP.SharedKernel.Domain.Primitives;
using Serilog.Context;
using System.Security.Claims;

namespace WebAPI.Middlewares
{
    public class UserLogMiddleware
    {
        private readonly RequestDelegate _next;

        public UserLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentUserProvider currentUserProvider)
        {
            var userIdString = currentUserProvider.UserId;

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                using (LogContext.PushProperty("UserId", userId))
                {
                    await _next(context);
                }
            }
            else
            {
                using (LogContext.PushProperty("UserId", "Anonymous"))
                {
                    await _next(context);
                }
            }
        }
    }
}
