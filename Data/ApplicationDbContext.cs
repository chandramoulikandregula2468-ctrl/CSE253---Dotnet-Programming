using Microsoft.EntityFrameworkCore;
using TouristSafetySystem.Models;

namespace TouristSafetySystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tourist details
        public DbSet<Tourist> Tourists { get; set; }

        // Emergency reports
        public DbSet<Emergency> Emergencies { get; set; }

        // Tourist GPS locations
        public DbSet<TouristLocation> TouristLocations { get; set; }

        // Geo-Fencing safety zones
        public DbSet<Geofence> Geofences { get; set; }

        // Geo-Fencing alerts
        public DbSet<GeofenceAlert> GeofenceAlerts { get; set; }

        // Blockchain-style digital identities
        public DbSet<DigitalIdentity> DigitalIdentities { get; set; }
    }
}