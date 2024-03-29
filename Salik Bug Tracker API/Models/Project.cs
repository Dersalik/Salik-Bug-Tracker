﻿using System.ComponentModel.DataAnnotations;

namespace Salik_Bug_Tracker_API.Models
{
    public class Project
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime TargetEndDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public List<Module>? modules { get; set;}
    }
}
