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
        public List<Skill>? skills { get; set; }
        public List<ModuleUser>? moduleUsers { get; set; }
        public List<BugDeveloper>? bugDevelopers { get; set; }

    }
}
