using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options) 
        {
                
        }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
