using Salik_Bug_Tracker_API.Models;
using System.ComponentModel.DataAnnotations;

namespace Salik_Bug_Tracker_API.DTO
{
    public class ProjectDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? ProjectName { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime TargetEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }

    }
}
