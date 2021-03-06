﻿// -----------------------------------------------------------------
// <copyright file="Program.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Standalone
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using OpenTibia.Common;
    using OpenTibia.Common.Contracts.Abstractions;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Data.Contracts.Abstractions;
    using OpenTibia.Data.InMemoryDatabase;
    using OpenTibia.Providers.Azure;
    using OpenTibia.Security;
    using OpenTibia.Security.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Events.MoveUseFile;
    using OpenTibia.Server.Factories;
    using OpenTibia.Server.Items.ObjectsFile;
    using OpenTibia.Server.Map;
    using OpenTibia.Server.Map.SectorFiles;
    using OpenTibia.Server.Monsters.MonFiles;
    using OpenTibia.Server.PathFinding.AStar;
    using OpenTibia.Server.Spawns.MonstersDbFile;
    using Serilog;

    /// <summary>
    /// Class that represents a standalone OpenTibia server.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The cancellation token source for the entire application.
        /// </summary>
        private static readonly CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">The arguments for this program.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("logsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.ForContext<Program>().Information("Building host...");

            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true, reloadOnChange: true);
                    configHost.AddEnvironmentVariables(prefix: "OTS_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    configApp.AddEnvironmentVariables(prefix: "OTS_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices(Program.ConfigureServices)
                .UseSerilog()
                .Build();

            await host.RunAsync(Program.MasterCancellationTokenSource.Token);
        }

        /// <summary>
        /// Configuration root, where services are configured and added into the service collection, often depending on the configuration set.
        /// </summary>
        /// <param name="hostingContext">The hosting context.</param>
        /// <param name="services">The services collection.</param>
        private static void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
        {
            hostingContext.ThrowIfNull(nameof(hostingContext));
            services.ThrowIfNull(nameof(services));

            // configure options
            services.Configure<GameConfigurationOptions>(hostingContext.Configuration.GetSection(nameof(GameConfigurationOptions)));
            services.Configure<ProtocolConfigurationOptions>(hostingContext.Configuration.GetSection(nameof(ProtocolConfigurationOptions)));

            // Add the master cancellation token source of the entire service.
            services.AddSingleton(Program.MasterCancellationTokenSource);

            // Add known instances of configuration and logger.
            services.AddSingleton(hostingContext.Configuration);
            services.AddSingleton(Log.Logger);
            services.AddSingleton<TelemetryClient>();

            // Add service implementations here.
            services.AddSingleton<IProtocolFactory, ProtocolFactory>();
            services.AddSingleton<IConnectionManager, ConnectionManager>();

            services.AddAzureProviders(hostingContext.Configuration);

            // services.AddCosmosDBDatabaseContext(hostingContext.Configuration);
            services.AddInMemoryDatabaseContext(hostingContext.Configuration);

            services.AddSectorFilesMapLoader(hostingContext.Configuration);

            services.AddObjectsFileItemTypeLoader(hostingContext.Configuration);

            services.AddMonFilesMonsterTypeLoader(hostingContext.Configuration);

            services.AddMonsterDbFileMonsterSpawnLoader(hostingContext.Configuration);

            services.AddMoveUseItemEventLoader(hostingContext.Configuration);

            services.AddAStarPathFinder(hostingContext.Configuration);

            // IOpenTibiaDbContext itself is added by the Add<DatabaseProvider>() call above.
            // We add Func<IOpenTibiaDbContext> to let callers retrieve a transient instance of this from the Application context,
            // rather than save an actual copy of the DB context in the app context.
            services.AddSingleton<Func<IOpenTibiaDbContext>>(s => s.GetService<IOpenTibiaDbContext>);
            services.AddSingleton<IApplicationContext, ApplicationContext>();

            services.AddSingleton<IMap, Map>();
            services.AddSingleton<ITileAccessor>(s => s.GetService<IMap>());

            services.AddSingleton<IScriptApi, StandardScriptApi>();

            services.AddSingleton<IItemFactory, ItemFactory>();

            services.AddSingleton<ICreatureFactory, CreatureFactory>();
            services.AddSingleton<ICreatureManager, CreatureManager>();
            services.AddSingleton<ICreatureFinder>(s => s.GetService<ICreatureManager>());

            services.AddGameHandlers();
            services.AddLoginHandlers();
            services.AddManagementHandlers();

            // Those executing should derive from IHostedService and be added using AddHostedService.
            services.AddSingleton<SimpleDoSDefender>();
            services.AddHostedService(s => s.GetService<SimpleDoSDefender>());
            services.AddSingleton<IDoSDefender>(s => s.GetService<SimpleDoSDefender>());

            services.AddSingleton<LoginListener>();
            services.AddHostedService(s => s.GetService<LoginListener>());
            services.AddSingleton<IOpenTibiaListener>(s => s.GetService<LoginListener>());

            services.AddSingleton<GameListener>();
            services.AddHostedService(s => s.GetService<GameListener>());
            services.AddSingleton<IOpenTibiaListener>(s => s.GetService<GameListener>());

            services.AddSingleton<Game>();
            services.AddHostedService(s => s.GetService<Game>());
            services.AddSingleton<IGame>(s => s.GetService<Game>());
        }
    }
}
