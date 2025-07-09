using BellosoftWebApi.Models;
using BellosoftWebApi.Services.AuthenticatedUser;
using Microsoft.EntityFrameworkCore;
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

        public async Task<AuthUserData?> GetActiveUser()
        {
            int? id = GetUserId();
            if (id is null)
                return null;

            AuthUserData? authUser = await context.Users
                .Where(user => user.Id == id)
                .Select(user => new AuthUserData(user.Id, user.SelectedDeckId, user.UpdatedAt, user.DeletedAt))
                .FirstOrDefaultAsync();

            if (authUser is null || authUser.IsSoftDeleted)
                return null;

            return authUser;
        }
    }
}
