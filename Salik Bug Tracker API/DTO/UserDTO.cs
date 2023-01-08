namespace Salik_Bug_Tracker_API.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public int Score { get; set; } = 0;
        public string? speciality { get; set; }
        public string? Email { get; set; }
    }
}
