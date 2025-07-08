using BellosoftWebApi.Models;

namespace BellosoftWebApi.Facades
{
    public interface IAuthenticatedUser
    {
        int? GetUserId();
        Task<User?> GetActiveUser();
    }
}
