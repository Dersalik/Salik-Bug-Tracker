using System.ComponentModel.DataAnnotations;

namespace Salik_Bug_Tracker_API.DTO
{
    public class ProjectForUpdateDTO
    {
        [Required]
        public string? ProjectName { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime TargetEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }
    }
}
