
using System.ComponentModel.DataAnnotations;

namespace TouristSafetySystem.Models
{
    public class Geofence
    {
        public int Id { get; set; }

        [Required]
        public string ZoneName { get; set; } = "";

        public double CenterLatitude { get; set; }

        public double CenterLongitude { get; set; }

        public double RadiusInKm { get; set; }
    }
}
