using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Serilog;

namespace Heyworks.PocketShooter.Realtime.Hosting
{
    /// <summary>
    /// Class responsible for setup and initialization all application infrastructure (IoC, logging, etc.).
    /// Can be instantiated 2 times without any issues, second instance may be disposed.
    /// </summary>
    public sealed class RuntimeHostBuilder
    {
        public static IHostBuilder Create(string applicationName, string binaryPath)
        {
            return new HostBuilder()
                .UseEnvironment(EnvironmentNames.EnvironmentName)
                .UseContentRoot(binaryPath)
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    var env = hostBuilderContext.HostingEnvironment;
                    env.ApplicationName = applicationName;
                    configurationBuilder
                        .SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddXmlFile("Photon.PocketShooter.config", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    services.AddSingleton<IGameManagementService, GameManagementService>();

                    services.AddSingleton<MetaClusterClientBuilder>();
                    services.AddSingleton<IClusterClient>(s =>
                    {
                        var builder = s.GetService<MetaClusterClientBuilder>();
                        builder.ConfigureApplicationParts(parts =>
                        {
                            parts.AddApplicationPart(typeof(IGameRoomsPublisherGrain).Assembly);
                        });

                        return builder.Build();
                    });

                    services.AddSingleton<INetworkConfiguration, NetworkConfiguration>();
                    services.Configure<RealtimeConfiguration>(hostBuilderContext.Configuration.GetSection("Realtime"));
                    services.AddSingleton<GameManagementChannel>();
                    services.AddSingleton<IGameManagementChannel>(s => s.GetService<GameManagementChannel>());
                })
                .UseSerilog((hostContext, logging) =>
                {
                    logging.ReadFrom
                    .Configuration(hostContext.Configuration)
                    .Enrich.WithThreadId();
                });
        }
    }
}
