using System.ComponentModel.DataAnnotations;

namespace Salik_Bug_Tracker_API.Models
{
    public class Module
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime TargetEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public Project? project { get; set; }
        public int ProjectId { get; set; }
        public List<ModuleUser>? moduleUsers { get; set; }

    }
}
