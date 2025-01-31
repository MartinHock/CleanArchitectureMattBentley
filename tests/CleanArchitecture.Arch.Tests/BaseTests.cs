﻿using System.Reflection;
using CleanArchitecture.Api.Controllers;
using CleanArchitecture.Application.AutofacModules;
using CleanArchitecture.Core.Abstractions.Entities;
using CleanArchitecture.Infrastructure.AutofacModules;

namespace CleanArchitecture.Arch.Tests
{
    public abstract class BaseTests
    {
        protected static Assembly ApiAssembly = typeof(WeatherForecastsController).Assembly;
        protected static Assembly ApplicationAssembly = typeof(ApplicationModule).Assembly;
        protected static Assembly InfrastuctureAssembly = typeof(InfrastructureModule).Assembly;
        protected static Assembly CoreAssembly = typeof(EntityBase).Assembly;

        protected static Types AllTypes = Types.InAssemblies(new List<Assembly>
            { ApiAssembly, ApplicationAssembly, InfrastuctureAssembly, CoreAssembly });
    }
}