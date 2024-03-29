﻿namespace Salik_Bug_Tracker_API.Models
{
    public class Bug
    {
        public int Id { get; set; }
        public ApplicationUser? UserReportedBy { get; set; }
        public string? ApplicationUserId { get; set; }
        public Module? module { get; set; }
        public int ModulesId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateClosed { get; set; }
        public string? IssueSummary { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public List<BugDeveloper>? bugDevelopers { get; set; }
    }


}
