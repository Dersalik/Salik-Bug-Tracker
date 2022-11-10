namespace Salik_Bug_Tracker_API.Models
{
    public class BugDeveloper
    {
        public int Id { get; set; }
        public Bug? bug { get; set; }
        public int BugId { get; set; }
        public ApplicationUser? user { get; set; }
        public string? ApplicationUserId { get; set; }
        public DateTime AssignedDate { get; set; }  
        public DateTime DateSolved { get; set;}
        public string? SolutionDetail { get; set; }

    }
}
