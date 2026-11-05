
using Microsoft.EntityFrameworkCore;
using TouristSafetySystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services
builder.Services.AddControllersWithViews();

// Connect SQLite database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=TouristSafety.db"));

var app = builder.Build();

// Create database and missing tables
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    db.Database.EnsureCreated();

    // Create Emergencies table
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS Emergencies (
            Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            TouristId TEXT NOT NULL,
            EmergencyType TEXT NOT NULL,
            Description TEXT NOT NULL,
            ReportedAt TEXT NOT NULL,
            Status TEXT NOT NULL
        );
    ");

    // Create TouristLocations table
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS TouristLocations (
            Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            TouristId TEXT NOT NULL,
            Latitude REAL NOT NULL,
            Longitude REAL NOT NULL,
            RecordedAt TEXT NOT NULL
        );
    ");

    // Create Geofences table
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS Geofences (
            Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            ZoneName TEXT NOT NULL,
            CenterLatitude REAL NOT NULL,
            CenterLongitude REAL NOT NULL,
            RadiusInKm REAL NOT NULL
        );
    ");

    // Create GeofenceAlerts table
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS GeofenceAlerts (
            Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            TouristId TEXT NOT NULL,
            ZoneName TEXT NOT NULL,
            DistanceInKm REAL NOT NULL,
            AlertMessage TEXT NOT NULL,
            CreatedAt TEXT NOT NULL,
            Status TEXT NOT NULL
        );
    ");

    // Create DigitalIdentities table
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS DigitalIdentities (
            Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            TouristId TEXT NOT NULL,
            IdentityHash TEXT NOT NULL,
            PreviousHash TEXT NOT NULL,
            CreatedAt TEXT NOT NULL,
            Status TEXT NOT NULL
        );
    ");
}

// Configure request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
