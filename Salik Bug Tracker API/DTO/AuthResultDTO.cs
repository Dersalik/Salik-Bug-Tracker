﻿namespace Salik_Bug_Tracker_API.DTO
{
    public class AuthResultDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
