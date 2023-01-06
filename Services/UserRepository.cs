using Attendr.IdentityServer.DbContexts;
using Attendr.IdentityServer.Entities;
using Attendr.IdentityServer.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Attendr.IdentityServer.Helpers.VerificationHelper;

namespace Attendr.IdentityServer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AttendrDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserRepository(AttendrDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
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

        public async Task<IEnumerable<UserClaim>> GetClaimsByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            return await _context.Claims.Where(c => c.User.Username == username).ToListAsync();


            //// making sure user has claims included
            //var userWithClaims = _context.Users.Include(u => u.Claims).First(u => u.Id == user.Id);
            //var userClaims = new List<Claim>
            //{
            //    new Claim("user_id", userWithClaims.Id.ToString()),
            //    new Claim(JwtClaimTypes.Email, userWithClaims.Email),
            //};
            //foreach (var claim in userWithClaims.Claims)
            //{
            //    userClaims.Add(new Claim(claim.Type, claim.Value));
            //}
            //return userClaims.ToArray();

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

            // hashing password
            user.Password = _passwordHasher.HashPassword(user, user.Password);

            // setting user claims
            user.Claims.Add(new UserClaim(JwtClaimTypes.Email, user.Email));
            user.Claims.Add(new UserClaim("role", "student"));
            ;

            // verification
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
        public async Task AddClaimAsync(string username, string type, string value)
        {
            var user = await GetUserByUsernameAsync(username);
            user.Claims.Add(new UserClaim(type, value));
        }
        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
