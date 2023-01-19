﻿using System.ComponentModel.DataAnnotations;

namespace Salik_Bug_Tracker_API.DTO
{
    public class SkillDTOForUpdate
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Level { get; set; }
    }
}
