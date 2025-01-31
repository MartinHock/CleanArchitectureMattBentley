﻿using Autofac;
using CleanArchitecture.AcceptanceTests.Pages;
using CleanArchitecture.AcceptanceTests.Settings;
using CleanArchitecture.Infrastructure.AutofacModules;
using Microsoft.Extensions.Configuration;
using SpecFlow.Autofac;
using SpecFlow.Autofac.SpecFlowPlugin;

namespace CleanArchitecture.AcceptanceTests.Hooks
{
    [Binding]
    public sealed class GlobalHooks
    {
        private static IConfiguration Configuration;

        [GlobalDependencies]
        public static void CreateGlobalContainer(ContainerBuilder container)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .Build();

            BrowserSettings browserSettings = new BrowserSettings();
            Configuration.GetSection("Browser").Bind(browserSettings);

            container.RegisterInstance(new TestHostEnvironment())
                .AsImplementedInterfaces();

            TestHarness testHarness = new TestHarness(browserSettings);

            container.RegisterInstance(testHarness).AsSelf();
            container.RegisterInstance(browserSettings);

            RegisterApplicationServices(container);
        }

        [ScenarioDependencies]
        public static void CreateContainerBuilder(ContainerBuilder container)
        {
            container.AddSpecFlowBindings<GlobalHooks>();
            RegisterApplicationServices(container);
        }

        private static void RegisterApplicationServices(ContainerBuilder container)
        {
            container.RegisterModule(new InfrastructureModule(Configuration));
        }

        [BeforeFeature]
        public static async Task BeforeFeatureAsync(TestHarness testHarness)
        {
            await testHarness.StartAsync();
            testHarness.CurrentPage = new HomePage(testHarness.Page);
        }

        [BeforeScenario]
        public static async Task BeforeScenarioAsync(FeatureContext featureContext, ScenarioContext scenarioContext,
            TestHarness testHarness)
        {
            await testHarness.StartScenarioAsync(featureContext.FeatureInfo.Title, scenarioContext.ScenarioInfo.Title);
        }

        [AfterScenario]
        public static async Task AfterScenarioAsync(ScenarioContext scenarioContext, TestHarness testHarness)
        {
            await testHarness.StopScenarioAsync(scenarioContext.ScenarioExecutionStatus.ToString());
        }

        [AfterFeature]
        public static async Task AfterFeature(TestHarness testHarness)
        {
            await testHarness.StopAsync();
        }
    }
}