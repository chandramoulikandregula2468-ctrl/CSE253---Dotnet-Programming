
using System;
using System.ComponentModel.DataAnnotations;

namespace TouristSafetySystem.Models
{
    public class Tourist
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        [Required]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Phone { get; set; } = "";

        public string Destination { get; set; } = "";

        public DateTime TravelDate { get; set; }
        
        public string TouristId { get; set; } = "";
    }
}
