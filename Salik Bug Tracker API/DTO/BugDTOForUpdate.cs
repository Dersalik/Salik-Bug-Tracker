﻿namespace Salik_Bug_Tracker_API.DTO
{
    public class BugDTOForUpdate
    {
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateClosed { get; set; }
        public string? IssueSummary { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
    }
}
