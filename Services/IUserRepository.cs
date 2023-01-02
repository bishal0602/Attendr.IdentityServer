using Attendr.IdentityServer.Entities;
using System.Security.Claims;

namespace Attendr.IdentityServer.Services
{
    public interface IUserRepository
    {
        Task<User> FindUserByUserNameAsync(string userName);
        Task<User> FindUserByUserIdAsync(string userId);
        Claim[] GetClaimsForUser(User user);
    }
}