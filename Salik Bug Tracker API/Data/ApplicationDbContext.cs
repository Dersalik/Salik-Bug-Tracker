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
        public DbSet<Bug> bugs { get; set; }
        public DbSet<Module> modules { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<BugDeveloper> bugDevelopers { get; set; }
        public DbSet<ModuleUser> ModuleUsers { get; set; }
        public DbSet<Skill> skills { get; set; }


    }
}
