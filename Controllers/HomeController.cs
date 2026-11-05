
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristSafetySystem.Models;
using TouristSafetySystem.Data;

namespace TouristSafetySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // HOMEPAGE
        // ==========================================

        public IActionResult Index()
        {
            return View();
        }

        // ==========================================
        // SAFETY DASHBOARD
        // ==========================================

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Dashboard()
        {
            var dashboard = new DashboardViewModel
            {
                Tourists = _context.Tourists.ToList(),

                Emergencies = _context.Emergencies
                    .OrderByDescending(e => e.ReportedAt)
                    .ToList(),

                TouristLocations = _context.TouristLocations
                    .OrderByDescending(l => l.RecordedAt)
                    .ToList(),

                GeofenceAlerts = _context.GeofenceAlerts
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList(),

                DigitalIdentities = _context.DigitalIdentities
                    .OrderByDescending(d => d.Id)
                    .ToList()
            };

            return View(dashboard);
        }

        // ==========================================
        // AUTOMATIC DASHBOARD UPDATE DATA
        // ==========================================

        [HttpGet]
        public IActionResult DashboardUpdates()
        {
            var tourists = _context.Tourists.ToList();

            var emergencies = _context.Emergencies
                .OrderByDescending(e => e.ReportedAt)
                .ToList();

            var locations = _context.TouristLocations
                .OrderByDescending(l => l.RecordedAt)
                .Take(20)
                .ToList();

            var alerts = _context.GeofenceAlerts
                .OrderByDescending(a => a.CreatedAt)
                .Take(20)
                .ToList();

            var identities = _context.DigitalIdentities.ToList();

            var result = new
            {
                totalTourists = tourists.Count,

                emergencyReports = emergencies.Count,

                pendingEmergencies = emergencies.Count(
                    e => e.Status == "Pending"),

                geofenceAlerts = alerts.Count == 0
                    ? _context.GeofenceAlerts.Count()
                    : _context.GeofenceAlerts.Count(),

                digitalIdentities = identities.Count,

                latestLocations = locations.Select(l => new
                {
                    touristId = l.TouristId,
                    latitude = l.Latitude,
                    longitude = l.Longitude,
                    recordedAt = l.RecordedAt.ToString(
                        "dd-MM-yyyy HH:mm:ss")
                }).ToList(),

                latestAlerts = alerts.Select(a => new
                {
                    id = a.Id,
                    touristId = a.TouristId,
                    zoneName = a.ZoneName,
                    distanceInKm = a.DistanceInKm,
                    alertMessage = a.AlertMessage,
                    createdAt = a.CreatedAt.ToString(
                        "dd-MM-yyyy HH:mm:ss"),
                    status = a.Status
                }).ToList(),

                latestEmergencies = emergencies.Take(20).Select(e => new
                {
                    id = e.Id,
                    touristId = e.TouristId,
                    emergencyType = e.EmergencyType,
                    description = e.Description,
                    reportedAt = e.ReportedAt.ToString(
                        "dd-MM-yyyy HH:mm:ss"),
                    status = e.Status
                }).ToList()
            };

            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            return Json(result);
        }

        // ==========================================
        // TOURIST REGISTRATION
        // ==========================================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Tourist tourist)
        {
            if (ModelState.IsValid)
            {
                using var transaction =
                    _context.Database.BeginTransaction();

                try
                {
                    tourist.TouristId = "T" +
                        Guid.NewGuid().ToString("N")
                        .Substring(0, 8).ToUpper();

                    _context.Tourists.Add(tourist);
                    _context.SaveChanges();

                    var previousIdentity = _context.DigitalIdentities
                        .OrderByDescending(d => d.Id)
                        .FirstOrDefault();

                    string previousHash = previousIdentity == null
                        ? "GENESIS"
                        : previousIdentity.IdentityHash;

                    DateTime createdAt = DateTime.Now;

                    string identityData =
                        tourist.TouristId + "|" +
                        tourist.Name + "|" +
                        tourist.Email + "|" +
                        tourist.Destination + "|" +
                        tourist.TravelDate.ToString("O") + "|" +
                        previousHash + "|" +
                        createdAt.ToString("O");

                    using SHA256 sha256 = SHA256.Create();

                    byte[] bytes =
                        Encoding.UTF8.GetBytes(identityData);

                    byte[] hashBytes =
                        sha256.ComputeHash(bytes);

                    string identityHash =
                        Convert.ToHexString(hashBytes);

                    var digitalIdentity = new DigitalIdentity
                    {
                        TouristId = tourist.TouristId,
                        IdentityHash = identityHash,
                        PreviousHash = previousHash,
                        CreatedAt = createdAt,
                        Status = "Verified"
                    };

                    _context.DigitalIdentities.Add(digitalIdentity);
                    _context.SaveChanges();

                    transaction.Commit();

                    return View("RegistrationSuccess", tourist);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return View(tourist);
        }

        // ==========================================
        // GENERATE MISSING DIGITAL IDENTITIES
        // ==========================================

        [HttpPost]
        public IActionResult GenerateMissingIdentities()
        {
            using var transaction =
                _context.Database.BeginTransaction();

            try
            {
                var tourists = _context.Tourists.ToList();

                int createdCount = 0;

                foreach (var tourist in tourists)
                {
                    bool exists = _context.DigitalIdentities
                        .Any(d => d.TouristId == tourist.TouristId);

                    if (exists)
                    {
                        continue;
                    }

                    var previousIdentity = _context.DigitalIdentities
                        .OrderByDescending(d => d.Id)
                        .FirstOrDefault();

                    string previousHash = previousIdentity == null
                        ? "GENESIS"
                        : previousIdentity.IdentityHash;

                    DateTime createdAt = DateTime.Now;

                    string identityData =
                        tourist.TouristId + "|" +
                        tourist.Name + "|" +
                        tourist.Email + "|" +
                        tourist.Destination + "|" +
                        tourist.TravelDate.ToString("O") + "|" +
                        previousHash + "|" +
                        createdAt.ToString("O");

                    using SHA256 sha256 = SHA256.Create();

                    byte[] bytes =
                        Encoding.UTF8.GetBytes(identityData);

                    byte[] hashBytes =
                        sha256.ComputeHash(bytes);

                    string identityHash =
                        Convert.ToHexString(hashBytes);

                    var identity = new DigitalIdentity
                    {
                        TouristId = tourist.TouristId,
                        IdentityHash = identityHash,
                        PreviousHash = previousHash,
                        CreatedAt = createdAt,
                        Status = "Verified"
                    };

                    _context.DigitalIdentities.Add(identity);
                    _context.SaveChanges();

                    createdCount++;
                }

                transaction.Commit();

                TempData["IdentityMessage"] =
                    createdCount +
                    " missing digital identity record(s) generated.";

                return RedirectToAction("Dashboard");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ==========================================
        // DIGITAL IDENTITY VERIFICATION
        // ==========================================

        [HttpGet]
        public IActionResult VerifyIdentity()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyIdentity(string touristId)
        {
            var tourist = _context.Tourists
                .FirstOrDefault(t => t.TouristId == touristId);

            if (tourist == null)
            {
                ViewBag.Message = "Tourist ID not found!";
                return View();
            }

            var identity = _context.DigitalIdentities
                .FirstOrDefault(d => d.TouristId == touristId);

            if (identity == null)
            {
                ViewBag.Message = "Digital Identity not found!";
                return View();
            }

            DateTime createdAt = DateTime.SpecifyKind(
                identity.CreatedAt,
                DateTimeKind.Local
            );

            string identityData =
                tourist.TouristId + "|" +
                tourist.Name + "|" +
                tourist.Email + "|" +
                tourist.Destination + "|" +
                tourist.TravelDate.ToString("O") + "|" +
                identity.PreviousHash + "|" +
                createdAt.ToString("O");

            using SHA256 sha256 = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(identityData);

            byte[] hashBytes = sha256.ComputeHash(bytes);

            string calculatedHash =
                Convert.ToHexString(hashBytes);

            bool hashMatches = string.Equals(
                calculatedHash,
                identity.IdentityHash,
                StringComparison.OrdinalIgnoreCase
            );

            var allIdentities = _context.DigitalIdentities
                .OrderBy(d => d.Id)
                .ToList();

            string previousHash = "GENESIS";

            bool chainValid = true;

            foreach (var item in allIdentities)
            {
                if (!string.Equals(
                    item.PreviousHash,
                    previousHash,
                    StringComparison.OrdinalIgnoreCase))
                {
                    chainValid = false;
                    break;
                }

                previousHash = item.IdentityHash;
            }

            if (hashMatches && chainValid)
            {
                ViewBag.Message =
                    "Identity and blockchain verification successful!";

                ViewBag.VerificationStatus = "Verified";

                ViewBag.ChainStatus =
                    "Blockchain Chain Verified";
            }
            else
            {
                ViewBag.Message =
                    "Verification failed! Identity or blockchain chain mismatch.";

                ViewBag.VerificationStatus = hashMatches
                    ? "Verified"
                    : "Verification Failed";

                ViewBag.ChainStatus = chainValid
                    ? "Blockchain Chain Verified"
                    : "Blockchain Chain Failed";
            }

            ViewBag.Tourist = tourist;
            ViewBag.Identity = identity;
            ViewBag.CalculatedHash = calculatedHash;

            return View();
        }

        // ==========================================
        // BLOCKCHAIN TAMPERING DETECTION
        // ==========================================

        [HttpGet]
        public IActionResult TamperingTest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TamperingTest(
            string touristId,
            string modifiedName)
        {
            var tourist = _context.Tourists
                .FirstOrDefault(t => t.TouristId == touristId);

            if (tourist == null)
            {
                ViewBag.Message = "Tourist ID not found!";
                return View();
            }

            var identity = _context.DigitalIdentities
                .FirstOrDefault(d => d.TouristId == touristId);

            if (identity == null)
            {
                ViewBag.Message = "Digital Identity not found!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(modifiedName))
            {
                ViewBag.Message = "Please enter a modified name!";
                return View();
            }

            DateTime createdAt = DateTime.SpecifyKind(
                identity.CreatedAt,
                DateTimeKind.Local
            );

            string testData =
                tourist.TouristId + "|" +
                modifiedName + "|" +
                tourist.Email + "|" +
                tourist.Destination + "|" +
                tourist.TravelDate.ToString("O") + "|" +
                identity.PreviousHash + "|" +
                createdAt.ToString("O");

            using SHA256 sha256 = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(testData);

            byte[] hashBytes = sha256.ComputeHash(bytes);

            string simulatedHash = Convert.ToHexString(hashBytes);

            bool hashMatches = string.Equals(
                simulatedHash,
                identity.IdentityHash,
                StringComparison.OrdinalIgnoreCase
            );

            ViewBag.Tourist = tourist;
            ViewBag.Identity = identity;
            ViewBag.ModifiedName = modifiedName;
            ViewBag.SimulatedHash = simulatedHash;

            if (hashMatches)
            {
                ViewBag.Message =
                    "No tampering detected. Hash matches.";

                ViewBag.TamperingStatus =
                    "No Tampering Detected";
            }
            else
            {
                ViewBag.Message =
                    "WARNING! Identity data has been changed. Hash mismatch detected!";

                ViewBag.TamperingStatus =
                    "Tampering Detected";
            }

            return View();
        }

        // ==========================================
        // EMERGENCY SOS
        // ==========================================

        [HttpGet]
        public IActionResult SOS()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SOS(Emergency emergency)
        {
            if (ModelState.IsValid)
            {
                bool touristExists = _context.Tourists
                    .Any(t => t.TouristId == emergency.TouristId);

                if (!touristExists)
                {
                    ModelState.AddModelError(
                        "TouristId",
                        "Invalid Tourist ID. Please enter a registered Tourist ID."
                    );

                    return View(emergency);
                }

                emergency.ReportedAt = DateTime.Now;
                emergency.Status = "Pending";

                _context.Emergencies.Add(emergency);
                _context.SaveChanges();

                return Content(
                    "Emergency report submitted successfully!"
                );
            }

            return View(emergency);
        }

        // Update Emergency Status
        [HttpPost]
        public IActionResult UpdateEmergencyStatus(int id)
        {
            var emergency = _context.Emergencies
                .FirstOrDefault(e => e.Id == id);

            if (emergency != null)
            {
                emergency.Status = "Resolved";
                _context.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }

        // Update Geo-Fencing Alert Status
        [HttpPost]
        public IActionResult UpdateGeofenceAlertStatus(int id)
        {
            var alert = _context.GeofenceAlerts
                .FirstOrDefault(a => a.Id == id);

            if (alert != null)
            {
                alert.Status = "Resolved";
                _context.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }

        // ==========================================
        // TOURIST LOCATION TRACKING
        // ==========================================

        [HttpGet]
        public IActionResult TrackLocation()
        {
            return View();
        }

        // Save location and automatically check safety zones
        [HttpPost]
        public IActionResult TrackLocation(TouristLocation location)
        {
            if (ModelState.IsValid)
            {
                bool touristExists = _context.Tourists
                    .Any(t => t.TouristId == location.TouristId);

                if (!touristExists)
                {
                    ModelState.AddModelError(
                        "TouristId",
                        "Invalid Tourist ID. Please enter a registered Tourist ID."
                    );

                    return View(location);
                }

                location.RecordedAt = DateTime.Now;

                _context.TouristLocations.Add(location);
                _context.SaveChanges();

                var zones = _context.Geofences.ToList();

                foreach (var zone in zones)
                {
                    double distance = CalculateDistance(
                        zone.CenterLatitude,
                        zone.CenterLongitude,
                        location.Latitude,
                        location.Longitude
                    );

                    bool isOutside = distance > zone.RadiusInKm;

                    var previousAlert = _context.GeofenceAlerts
                        .Where(a =>
                            a.TouristId == location.TouristId &&
                            a.ZoneName == zone.ZoneName)
                        .OrderByDescending(a => a.CreatedAt)
                        .FirstOrDefault();

                    bool wasOutside = previousAlert != null &&
                        (previousAlert.AlertMessage.Contains(
                            "outside",
                            StringComparison.OrdinalIgnoreCase
                        ) ||
                         previousAlert.AlertMessage.Contains(
                            "left",
                            StringComparison.OrdinalIgnoreCase
                        ));

                    if (previousAlert == null && isOutside)
                    {
                        _context.GeofenceAlerts.Add(new GeofenceAlert
                        {
                            TouristId = location.TouristId,
                            ZoneName = zone.ZoneName,
                            DistanceInKm = distance,
                            AlertMessage =
                                "Tourist is outside the safety zone!",
                            CreatedAt = DateTime.Now,
                            Status = "Pending"
                        });
                    }
                    else if (previousAlert != null &&
                             wasOutside != isOutside)
                    {
                        string message = isOutside
                            ? "Tourist has LEFT the safety zone!"
                            : "Tourist has ENTERED the safety zone!";

                        _context.GeofenceAlerts.Add(new GeofenceAlert
                        {
                            TouristId = location.TouristId,
                            ZoneName = zone.ZoneName,
                            DistanceInKm = distance,
                            AlertMessage = message,
                            CreatedAt = DateTime.Now,
                            Status = "Pending"
                        });
                    }
                }

                _context.SaveChanges();

                return Content(
                    "Location saved and Geo-Fencing checked successfully!"
                );
            }

            return View(location);
        }

        // ==========================================
        // GEO-FENCING
        // ==========================================

        [HttpGet]
        public IActionResult GeoFence()
        {
            LoadGeofences();
            return View();
        }

        // Save Geo-Fencing safety zone
        [HttpPost]
        public IActionResult GeoFence(Geofence geofence)
        {
            if (ModelState.IsValid)
            {
                if (geofence.CenterLatitude < -90 ||
                    geofence.CenterLatitude > 90 ||
                    geofence.CenterLongitude < -180 ||
                    geofence.CenterLongitude > 180 ||
                    geofence.RadiusInKm <= 0)
                {
                    ModelState.AddModelError(
                        "",
                        "Please enter valid coordinates and radius."
                    );

                    LoadGeofences();
                    return View(geofence);
                }

                _context.Geofences.Add(geofence);
                _context.SaveChanges();

                ViewBag.Message =
                    "Geo-Fencing safety zone saved successfully!";
            }

            LoadGeofences();
            return View(geofence);
        }

        // Check tourist location and generate alert
        [HttpPost]
        public IActionResult CheckGeoFence(
            string touristId,
            int geofenceId)
        {
            var tourist = _context.Tourists
                .FirstOrDefault(t => t.TouristId == touristId);

            var zone = _context.Geofences
                .FirstOrDefault(g => g.Id == geofenceId);

            if (tourist == null || zone == null)
            {
                ViewBag.CheckResult =
                    "Invalid Tourist ID or safety zone.";

                LoadGeofences();
                return View("GeoFence");
            }

            var location = _context.TouristLocations
                .Where(l => l.TouristId == touristId)
                .OrderByDescending(l => l.RecordedAt)
                .FirstOrDefault();

            if (location == null)
            {
                ViewBag.CheckResult =
                    "No saved GPS location found for this tourist.";

                LoadGeofences();
                return View("GeoFence");
            }

            double distance = CalculateDistance(
                zone.CenterLatitude,
                zone.CenterLongitude,
                location.Latitude,
                location.Longitude
            );

            if (distance <= zone.RadiusInKm)
            {
                ViewBag.CheckResult =
                    "Tourist is INSIDE the safety zone. Distance: "
                    + distance.ToString("0.00") + " km";
            }
            else
            {
                var alert = new GeofenceAlert
                {
                    TouristId = touristId,
                    ZoneName = zone.ZoneName,
                    DistanceInKm = distance,
                    AlertMessage =
                        "Tourist is outside the safety zone!",
                    CreatedAt = DateTime.Now,
                    Status = "Pending"
                };

                _context.GeofenceAlerts.Add(alert);
                _context.SaveChanges();

                ViewBag.CheckResult =
                    "WARNING: Tourist is OUTSIDE the safety zone! "
                    + "Distance: " + distance.ToString("0.00") + " km. "
                    + "Alert saved successfully!";
            }

            LoadGeofences();
            return View("GeoFence");
        }

        // Load saved safety zones
        private void LoadGeofences()
        {
            ViewBag.Geofences = _context.Geofences.ToList();
        }

        // Calculate distance using Haversine formula
        private double CalculateDistance(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            double earthRadius = 6371;

            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) *
                Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(
                Math.Sqrt(a),
                Math.Sqrt(1 - a)
            );

            return earthRadius * c;
        }

        // ==========================================
        // ERROR PAGE
        // ==========================================

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
