using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Salik_Bug_Tracker_API.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string? Name { get; set; }
        public int Score { get; set; } = 0;
        public string? speciality { get; set; }
        public IEnumerable<Skill>? skills { get; set; }
        public IEnumerable<ModuleUser>? moduleUsers { get; set; }
        public IEnumerable<BugDeveloper>? bugDevelopers { get; set; }

    }
}
