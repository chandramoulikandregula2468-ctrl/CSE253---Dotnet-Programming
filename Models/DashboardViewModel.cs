using System.Collections.Generic;

namespace TouristSafetySystem.Models
{
    public class DashboardViewModel
    {
        public List<Tourist> Tourists { get; set; } = new();

        public List<Emergency> Emergencies { get; set; } = new();

        public List<TouristLocation> TouristLocations { get; set; } = new();

        public List<GeofenceAlert> GeofenceAlerts { get; set; } = new();

        public List<DigitalIdentity> DigitalIdentities { get; set; } = new();
    }
}