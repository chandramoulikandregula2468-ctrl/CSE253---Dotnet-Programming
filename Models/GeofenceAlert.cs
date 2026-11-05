
using System;
using System.ComponentModel.DataAnnotations;

namespace TouristSafetySystem.Models
{
    public class GeofenceAlert
    {
        public int Id { get; set; }

        [Required]
        public string TouristId { get; set; } = "";

        [Required]
        public string ZoneName { get; set; } = "";

        public double DistanceInKm { get; set; }

        public string AlertMessage { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";
    }
}
