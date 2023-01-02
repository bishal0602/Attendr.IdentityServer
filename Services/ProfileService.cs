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
            try
            {
                //depending on the scope accessing the user data.
                if (!string.IsNullOrEmpty(context.Subject.Identity.Name))
                {
                    //get user from db (in my case this is by email)
                    var user = await _userRepository.FindUserByUserNameAsync(context.Subject.Identity.Name);

                    if (user != null)
                    {
                        var claims = _userRepository.GetClaimsForUser(user);

                        //set issued claims to return
                        context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                    }
                }
                else
                {
                    //get subject from context (this was set ResourceOwnerPasswordValidator.ValidateAsync),
                    //where and subject was set to my user id.
                    var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");

                    if (!string.IsNullOrEmpty(userId?.Value) && long.Parse(userId.Value) > 0)
                    {
                        //get user from db (find user by username)
                        var user = await _userRepository.FindUserByUserNameAsync(userId.Value);

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
                //get subject from context (set in ResourceOwnerPasswordValidator.ValidateAsync),
                var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "user_id");

                if (!string.IsNullOrEmpty(userId?.Value))
                {
                    var user = await _userRepository.FindUserByUserIdAsync(userId.Value);

                    if (user != null)
                    {
                        context.IsActive = user.Active;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error checking Active status of {context.Subject.Identity.Name}:\n{ex}");
                //handle error logging
            }
        }
    }
}
