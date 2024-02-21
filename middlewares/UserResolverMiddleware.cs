using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using WADProject1.Services;

namespace WADProject1.Middleware
{
    public class UserResolverMiddleware
    {
        private readonly RequestDelegate _next;

        public UserResolverMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, TenderContext dbContext)
        {
            var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid);
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;
                var user = await dbContext.Users
                    .Include(u => u.UserProfile)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                // Assuming you have a service or similar to hold the current user
                // You might need to register this service in Startup.cs
                var userService = context.RequestServices.GetService<IUserService>();
                if (userService != null)
                {
                    userService.SetCurrentUser(user);
                }
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
