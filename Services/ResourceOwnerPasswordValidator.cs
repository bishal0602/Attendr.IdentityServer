using Attendr.IdentityServer.Entities;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Serilog;
using System.Security.Claims;

namespace Attendr.IdentityServer.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserRepository _userRepository;

        public ResourceOwnerPasswordValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                //get your user model from db
                var user = await _userRepository.FindUserByUserNameAsync(context.UserName);
                if (user != null)
                {
                    // TODO: Hash Password
                    if (user.Password == context.Password)
                    {
                        //set the result
                        context.Result = new GrantValidationResult(
                            subject: user.Username.ToString(),
                            authenticationMethod: "custom",
                            claims: _userRepository.GetClaimsForUser(user));

                        return;
                    }

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");
                return;
            }
            catch (Exception ex)
            {
                Log.Error($"Validation error {context.UserName}:\n{ex.Message}");
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
        }

        //build claims array from user data
        //public static Claim[] GetUserClaims(User user)
        //{
        //    var userClaims = new List<Claim>();
        //    foreach (var claim in user.Claims)
        //    {
        //        userClaims.Add(new Claim(claim.Type, claim.Value));
        //    }
        //    return userClaims.ToArray();
        //}
    }
}