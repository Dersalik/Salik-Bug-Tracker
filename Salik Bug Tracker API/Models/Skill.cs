namespace Salik_Bug_Tracker_API.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Level { get; set; }
        public ApplicationUser? user { get; set; }
        public int ApplicationUserId { get; set; }
    }
}
