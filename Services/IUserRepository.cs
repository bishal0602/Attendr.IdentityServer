using Attendr.IdentityServer.Entities;
using System.Security.Claims;
using Attendr.IdentityServer.Helpers;

namespace Attendr.IdentityServer.Services
{
    public interface IUserRepository
    {
        Task<bool> IsEmailAlreadyInUseAsync(string email);
        Task<bool> IsAccountActive(string email);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUsernameAsync(string username);
        Task CreateUserAsync(User user);
        Task RemoveUserByEmailAsync(string email);
        Task<bool> ExistsUsernameAsync(string username);
        Task<VerificationHelper.VerificationStatusCodes> VerifyUserAsync(string username, string verificationCode);

        Task<User> FindUserByUserNameAsync(string userName);
        Task<User> FindUserByUserIdAsync(string userId);
        Task<IEnumerable<UserClaim>> GetClaimsByUsernameAsync(string value);
        Task AddClaimAsync(string username, string type, string value);



        Task<bool> SaveAsync();
    }
}