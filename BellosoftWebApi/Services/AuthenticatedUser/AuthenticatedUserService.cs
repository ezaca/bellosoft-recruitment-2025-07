using BellosoftWebApi.Models;
using System.Security.Claims;

namespace BellosoftWebApi.Services
{
    public class AuthenticatedUserService : IAuthenticatedUser
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext context;

        public AuthenticatedUserService(AppDbContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            httpContextAccessor = accessor;
        }

        public int? GetUserId()
        {
            Claim? claim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        public async Task<User?> GetActiveUser()
        {
            int? id = GetUserId();
            if (id is null)
                return null;

            User? user = await context.Users.FindAsync(id.Value);
            if (user is null || user.IsSoftDeleted)
                return null;

            return user;
        }
    }
}
