
using System;
using System.ComponentModel.DataAnnotations;

namespace TouristSafetySystem.Models
{
    public class DigitalIdentity
    {
        public int Id { get; set; }

        [Required]
        public string TouristId { get; set; } = "";

        [Required]
        public string IdentityHash { get; set; } = "";

        public string PreviousHash { get; set; } = "GENESIS";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Verified";
    }
}
