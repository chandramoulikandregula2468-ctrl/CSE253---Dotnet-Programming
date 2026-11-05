
using System;
using System.ComponentModel.DataAnnotations;

namespace TouristSafetySystem.Models
{
    public class Emergency
    {
        public int Id { get; set; }

        [Required]
        public string TouristId { get; set; } = "";

        [Required]
        public string EmergencyType { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        public DateTime ReportedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";
    }
}
