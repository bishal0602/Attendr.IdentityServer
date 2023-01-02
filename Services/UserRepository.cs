using Attendr.IdentityServer.DbContexts;
using Attendr.IdentityServer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
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
    }
}
