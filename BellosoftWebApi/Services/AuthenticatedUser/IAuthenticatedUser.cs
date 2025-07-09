using BellosoftWebApi.Services.AuthenticatedUser;

namespace BellosoftWebApi.Services
{
    public interface IAuthenticatedUser
    {
        int? GetUserId();
        Task<AuthUserData?> GetActiveUser();
    }
}
