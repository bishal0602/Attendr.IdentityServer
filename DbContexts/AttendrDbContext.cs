using Attendr.IdentityServer.Entities;
using Attendr.IdentityServer.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Attendr.IdentityServer.DbContexts
{
    public class AttendrDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserClaim> Claims { get; set; }

        public AttendrDbContext(DbContextOptions<AttendrDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //DataSeed.Seed(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
