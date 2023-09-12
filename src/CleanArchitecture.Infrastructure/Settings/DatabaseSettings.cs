using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Infrastructure.Settings
{
    public sealed class DatabaseSettings
    {
        public string? ConnectionString { get; set; }

        public static DatabaseSettings Create(IConfiguration configuration)
        {
            DatabaseSettings databaseSettings = new DatabaseSettings();
            configuration.GetSection("Database").Bind(databaseSettings);
            return databaseSettings;
        }
    }
}