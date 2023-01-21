using Salik_Bug_Tracker_API.Models;

namespace Salik_Bug_Tracker_API.DTO
{
    public class BugDTO
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateClosed { get; set; }
        public string? IssueSummary { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
    }
}
