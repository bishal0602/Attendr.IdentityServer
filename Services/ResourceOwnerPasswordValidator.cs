using Attendr.IdentityServer.Entities;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Security.Claims;

namespace Attendr.IdentityServer.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ResourceOwnerPasswordValidator(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                //get your user model from db
                var user = await _userRepository.FindUserByUserNameAsync(context.UserName);
                if (user != null)
                {
                    if (!user.Active)
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Account not verified.");
                        return;
                    }

                    var verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.Password, context.Password);

                    if (verifyPassword == PasswordVerificationResult.Success)
                    {
                        //set the result
                        context.Result = new GrantValidationResult(
                            subject: user.Username.ToString(),
                            authenticationMethod: "custom",
                            claims: await GetClaimsAsync(user.Username.ToString())
                            );

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

        private async Task<IEnumerable<Claim>> GetClaimsAsync(string username)
        {
            var userClaims = await _userRepository.GetClaimsByUsernameAsync(username);
            return userClaims.Select(c => new Claim(c.Type, c.Value)).ToList();
        }
    }
}