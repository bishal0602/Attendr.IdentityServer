using Attendr.IdentityServer.DbContexts;
using Attendr.IdentityServer.Entities;
using Attendr.IdentityServer.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Attendr.IdentityServer.Helpers.VerificationHelper;

namespace Attendr.IdentityServer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AttendrDbContext _context;

        public UserRepository(AttendrDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<User> FindUserByUserNameAsync(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
            return user;
        }
        public async Task<User> FindUserByUserIdAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            return user;
        }

        public Claim[] GetClaimsForUser(User user)
        {
            // making sure user has claims included
            var userWithClaims = _context.Users.Include(u => u.Claims).First(u => u.Id == user.Id);
            var userClaims = new List<Claim>();
            userClaims.Add(new Claim("user_id", userWithClaims.Id.ToString()));
            foreach (var claim in userWithClaims.Claims)
            {
                userClaims.Add(new Claim(claim.Type, claim.Value));
            }
            return userClaims.ToArray();
        }

        public async Task<bool> IsEmailAlreadyInUseAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Trim().ToLower() == email.Trim().ToLower());
        }
        public async Task<bool> IsAccountActive(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Trim().ToLower() == email.Trim().ToLower()
            && u.Active == true);
        }
        public async Task CreateUserAsync(User user)
        {
            user.Active = false;
            user.Email = user.Email.Trim().ToLower();
            user.Username = user.Username.Trim().ToLower();
            // TODO: generate securitycode
            var token = Guid.NewGuid().ToString();
            user.VerificationCode = token;
            user.VerificationCodeExpirationDate = DateTime.UtcNow.AddMinutes(5);

            await _context.AddAsync(user);
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == email.Trim().ToLower());
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username.Trim().ToLower() == username.Trim().ToLower());
        }
        public async Task RemoveUserAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            _context.Remove(user);
        }

        public async Task<bool> ExistsUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username.Trim().ToLower() == username.Trim().ToLower());
        }

        public async Task<VerificationStatusCodes> VerifyUserAsync(string username, string verificationCode)
        {
            if (!(await ExistsUsernameAsync(username)))
            {
                return VerificationStatusCodes.InvalidUsername;
            }
            var user = await GetUserByUsernameAsync(username);
            if (user.Active)
            {
                return VerificationStatusCodes.AccountAlreadyActivated;
            }

            var isVerificationCodeCorrect = user.VerificationCode == verificationCode;
            if (!isVerificationCodeCorrect)
            {
                return VerificationStatusCodes.InvalidVerificationCode;
            }
            var isVerificationCodeExpired = user.VerificationCodeExpirationDate <= DateTime.UtcNow;
            if (isVerificationCodeExpired)
            {
                return VerificationStatusCodes.VerificationCodeExpired;
            }

            user.Active = true;
            await SaveAsync();
            return VerificationStatusCodes.Success;

        }

        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }


    }
}
