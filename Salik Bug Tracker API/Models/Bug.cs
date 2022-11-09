namespace Salik_Bug_Tracker_API.Models
{
    public class Bug
    {
        public int Id { get; set; }
        public ApplicationUser? UserReportedBy { get; set; }
        public int ApplicationUserId { get; set; }
        public Module? module { get; set; }
        public int ModulesId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateClosed { get; set; }
        public string? IssueSummary { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
    }

    public class Priorities
    {
        public const string High="High";
        public const string Medium = "Medium";
        public const string Low = "Low";

    }

    public class Statuses
    {
        public const string ToDo = "To Do";
        public const string InProgress = "In Progress";
        public const string Done = "Done";
    }
}
