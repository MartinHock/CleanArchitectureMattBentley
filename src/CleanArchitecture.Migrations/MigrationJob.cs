using CleanArchitecture.Core.Locations.Entities;
using CleanArchitecture.Core.Locations.ValueObjects;
using CleanArchitecture.Hosting;
using CleanArchitecture.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Migrations
{
    public sealed class MigrationJob : Job
    {
        private readonly WeatherContext _context;

        public MigrationJob(ILogger<MigrationJob> logger,
            WeatherContext context,
            IHostApplicationLifetime hostApplicationLifetime) : base(logger, hostApplicationLifetime)
        {
            _context = context;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await MigrateDatabaseAsync();
        }

        private async Task MigrateDatabaseAsync()
        {
            Logger.LogInformation("Starting database migration");
            await _context.Database.MigrateAsync();
            Logger.LogInformation("Finished database migration");
            await MigrateLocationsAsync();
        }

        private async Task MigrateLocationsAsync()
        {
            List<Location> locations = new List<Location>
            {
                CreateLocation("United Kingdom", "London", 51.51m, -0.13m),
                CreateLocation("India", "Mumbai", 17.38m, -78.46m),
                CreateLocation("USA", "New York", 40.71m, -74.01m),
                CreateLocation("Japan", "Tokyo", 35.69m, 139.69m),
                CreateLocation("Australia", "Sydney", -33.87m, 151.21m)
            };
            List<Location> existingLocations = _context.Locations.ToList();
            foreach (Location location in locations)
            {
                if (existingLocations.Any(e => e.City == location.City))
                {
                    continue;
                }

                Logger.LogInformation("Adding location: {city}", location.City);
                _context.Locations.Add(location);
                await _context.SaveChangesAsync();
            }
        }

        private Location CreateLocation(string country, string city, decimal latitude, decimal longitude)
        {
            return Location.Create(country, city, Coordinates.Create(latitude, longitude));
        }
    }
}