namespace Salik_Bug_Tracker_API.Models
{
    public class ModuleUser
    {
        public int Id { get; set; }
        public ApplicationUser? user { get; set; }
        public string? ApplicationUserId { get; set; }
        public Module? module { get; set; }
        public int? ModuleId { get; set; }
    }
}
