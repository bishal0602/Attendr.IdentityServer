using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Serilog;

namespace Attendr.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public ProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var identityClaims = ((System.Security.Claims.ClaimsIdentity)context.Subject.Identity).Claims;
            try
            {
                //depending on the scope accessing the user data.
                var userId = identityClaims.FirstOrDefault(c => c.Type == "user_id");
                if (!string.IsNullOrEmpty(userId?.Value))
                {
                    var user = await _userRepository.FindUserByUserIdAsync(userId?.Value);

                    if (user != null)
                    {
                        var claims = _userRepository.GetClaimsForUser(user);

                        //set issued claims to return
                        context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                    }
                }
                else
                {
                    //get subject from context and subject was set to username
                    var username = identityClaims.FirstOrDefault(c => c.Type == "sub");

                    if (!string.IsNullOrEmpty(username.Value))
                    {
                        //get user from db (find user by username)
                        var user = await _userRepository.FindUserByUserNameAsync(username.Value);

                        // issue the claims for the user
                        if (user != null)
                        {
                            var claims = _userRepository.GetClaimsForUser(user);

                            context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error getiing profile data of {context.Subject.Identity.Name}:\n{ex}");

            }

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                var userName = ((System.Security.Claims.ClaimsIdentity)context.Subject.Identity).Claims.FirstOrDefault(c => c.Type == "sub");

                if (!string.IsNullOrEmpty(userName?.Value))
                {
                    var user = await _userRepository.GetUserByUsernameAsync(userName.Value);

                    if (user != null)
                    {
                        context.IsActive = user.Active;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error checking Active status of {context.Subject.Identity.Name}:\n{ex}");
            }
        }
    }
}
