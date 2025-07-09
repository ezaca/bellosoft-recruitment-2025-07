using BellosoftWebApi.Models;

namespace BellosoftWebApi.Services
{
    public interface IAuthenticatedUser
    {
        int? GetUserId();
        Task<User?> GetActiveUser();
    }
}
