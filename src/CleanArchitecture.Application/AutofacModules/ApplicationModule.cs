﻿using System.Reflection;
using Autofac;
using AutoMapper;
using MediatR;
using Module = Autofac.Module;

namespace CleanArchitecture.Application.AutofacModules
{
    public sealed class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register the Command and Query handler classes (they implement IRequestHandler<>)
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register Automapper profiles
            MapperConfiguration config = new MapperConfiguration(cfg => { cfg.AddMaps(ThisAssembly); });
            config.AssertConfigurationIsValid();

            builder.Register(c => config)
                .AsSelf()
                .SingleInstance();

            builder.Register(c =>
                {
                    IComponentContext ctx = c.Resolve<IComponentContext>();
                    MapperConfiguration mapperConfig = c.Resolve<MapperConfiguration>();
                    return mapperConfig.CreateMapper(ctx.Resolve);
                }).As<IMapper>()
                .SingleInstance();
        }
    }
}