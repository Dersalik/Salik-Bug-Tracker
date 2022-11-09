using Microsoft.AspNetCore.Identity;
using Salik_Bug_Tracker_API.Models.Helpers;

namespace Salik_Bug_Tracker_API.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedRolesToDb(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await roleManager.RoleExistsAsync(UserRoles.Developer))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Developer));
            }
        }
    }
}
