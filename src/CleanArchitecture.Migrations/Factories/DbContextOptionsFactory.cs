﻿using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Migrations.Factories
{
    public static class DbContextOptionsFactory
    {
        public static DbContextOptions<WeatherContext> Create(IConfiguration configuration)
        {
            DatabaseSettings appSettings = DatabaseSettings.Create(configuration);

            return new DbContextOptionsBuilder<WeatherContext>()
                .UseSqlServer(appSettings.ConnectionString, b => b.MigrationsAssembly("CleanArchitecture.Migrations"))
                .Options;
        }
    }
}