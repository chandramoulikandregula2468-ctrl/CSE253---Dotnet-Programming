
using System;
using System.ComponentModel.DataAnnotations;

namespace TouristSafetySystem.Models
{
    public class TouristLocation
    {
        public int Id { get; set; }

        [Required]
        public string TouristId { get; set; } = "";

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }
}
