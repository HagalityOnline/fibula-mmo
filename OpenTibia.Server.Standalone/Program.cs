// <copyright file="Program.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Standalone
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Security;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Mapping;
    using OpenTibia.Server.Monsters;
    using Serilog;

    /// <summary>
    /// Startup class for the OpenTibia.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point to the server program.
        /// </summary>
        /// <param name="args">The arguments of the program.</param>
        public static async Task Main(string[] args)
        {
            //var host = new HostBuilder()
            //    .ConfigureHostConfiguration(configHost =>
            //    {
            //        configHost.SetBasePath(Directory.GetCurrentDirectory());
            //        configHost.AddJsonFile("hostsettings.json", optional: true, reloadOnChange: true);
            //        configHost.AddEnvironmentVariables(prefix: "OTS_");
            //        configHost.AddCommandLine(args);
            //    })
            //    .ConfigureAppConfiguration((hostContext, configApp) =>
            //    {
            //        configApp.SetBasePath(Directory.GetCurrentDirectory());
            //        configApp.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            //        configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            //        configApp.AddEnvironmentVariables(prefix: "OTS_");
            //        configApp.AddCommandLine(args);
            //    })
            //    .Build();

            //await host.RunAsync();

            var env = new HostingEnvironment
            {
                EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
                ContentRootFileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory),
            };

            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            var services = ConfigureServices(new ServiceCollection(), configuration);
            var serviceProvider = services.BuildServiceProvider();

            var masterCancellationToken = serviceProvider.GetService<CancellationTokenSource>().Token;

            var tasksToRun = new List<Task>
            {
                serviceProvider.GetService<IGame>().RunAsync(masterCancellationToken),
            };

            foreach (var listener in serviceProvider.GetServices<IOpenTibiaListener>())
            {
                tasksToRun.Add(listener.RunAsync(masterCancellationToken));
            }

            await Task.WhenAll(tasksToRun.ToArray());
        }

        private static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.ThrowIfNull(nameof(services));
            configuration.ThrowIfNull(nameof(configuration));

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            // Add known instances of configuration and logger.
            services.AddSingleton(configuration);
            services.AddSingleton(Log.Logger);

            // Add the master cancellation token source of the entire service.
            services.AddSingleton<CancellationTokenSource>();

            // Add core services
            services.AddSingleton<IGame, Game>();
            services.AddSingleton<IMap, Map>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<IProtocolFactory, ProtocolFactory>();
            services.AddSingleton<IDoSDefender, SimpleDoSDefender>();

            // Add packet handlers and selectors for those handlers.
            services.AddGameHandlers();
            services.AddManagementHandlers();
            services.AddHandlerSelectors();

            // Loaders to take care of loading input files such as the map, items and scripts.
            AddLoaders(services);

            // Listeners for request processing.
            AddListeners(services);

            return services;
        }

        private static void AddLoaders(IServiceCollection services)
        {
            services.AddSingleton<IItemEventLoader, MoveUseItemEventLoader>();
            services.AddSingleton<IItemLoader, ObjectsFileItemLoader>();
            services.AddSingleton<IMonsterLoader, MonFilesMonsterLoader>();
        }

        private static void AddListeners(IServiceCollection services)
        {
            // TODO: add singleton for ManagementListener
            services.AddSingleton<IOpenTibiaListener, LoginListener>();
            services.AddSingleton<IOpenTibiaListener, GameListener>();
        }
    }
}
