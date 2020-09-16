using System;
using System.Linq;
using System.Net;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Serialization;
using Heyworks.PocketShooter.Meta.Services;
using Heyworks.PocketShooter.Meta.Services.Configuration;
using Heyworks.PocketShooter.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.MongoDB.StorageProviders;
using Serilog;

namespace Heyworks.PocketShooter.Meta.Server
{
    public class MetaServerBack
    {
        public IHost Create(string[] args, string basePath)
        {
            var host = new HostBuilder()
                .UseContentRoot(basePath)
                .UseEnvironment(EnvironmentNames.EnvironmentName)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    configBuilder
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    configBuilder.AddEnvironmentVariables();

                    if (args != null)
                    {
                        configBuilder.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, s) =>
                {
                    s.Configure<PurchaseOptions>(hostContext.Configuration.GetSection("Purchase"));

                    var dataStorageConnection = hostContext.Configuration.GetValue<string>("DataStorage:ConnectionString");
                    var connectionUrl = MongoUrl.Create(dataStorageConnection);
                    s.AddSingleton<IMongoClient>(new MongoClient(connectionUrl));
                    s.AddTransient(sp => sp.GetService<IMongoClient>().GetDatabase(connectionUrl.DatabaseName));
                    s.AddTransient(sp => sp
                    .GetService<IMongoDatabase>()
                    .GetCollection<PaymentTransaction>(sp.GetService<IOptions<PurchaseOptions>>().Value.TransactionsCollection));

                    s.AddSingleton<IDateTimeProvider>(new SystemTimeProvider());

                    s.AddTransient<IPlayerFactory, PlayerFactory>();
                    s.AddTransient<IArmyFactory, ArmyFactory>();
                    s.AddTransient<IGameFactory, GameFactory>();
                })
                .UseSerilog((hostContext, logging) =>
                 {
                     logging.ReadFrom
                     .Configuration(hostContext.Configuration)
                     .Enrich.WithThreadId();
                 })
                .UseOrleans((hostContext, siloBuilder) =>
                {
                    siloBuilder.Configure<ClusterOptions>(hostContext.Configuration.GetSection("Meta:Cluster"));

                    if (hostContext.HostingEnvironment.IsLocalOrDevelopment())
                    {
                        siloBuilder = siloBuilder.UseLocalhostClustering();
                        siloBuilder.Configure<EndpointOptions>(hostContext.Configuration.GetSection("Meta:Back:Endpoint"));
                    }
                    else
                    {
                        var ipConfig = hostContext.Configuration.GetValue<string>("Meta:Back:Endpoint:AdvertisedIPAddress");
                        var ip = string.IsNullOrEmpty(ipConfig) ? IPUtils.GetLocalIPAddresses().First() : IPAddress.Parse(ipConfig);
                        var siloPort = hostContext.Configuration.GetValue<int>("Meta:Back:Endpoint:SiloPort");
                        var gatewayPort = hostContext.Configuration.GetValue<int>("Meta:Back:Endpoint:GatewayPort");
                        var primarySiloEndpoint = new IPEndPoint(ip, siloPort);
                        siloBuilder = siloBuilder.UseDevelopmentClustering(primarySiloEndpoint);
                        // for some reason next line does not work
                        // siloBuilder.Configure<EndpointOptions>(hostContext.Configuration.GetSection("Meta:Back:Endpoint"));                        
                        siloBuilder.Configure<EndpointOptions>(options =>
                            {
                                options.AdvertisedIPAddress = ip;
                                options.SiloPort = siloPort;
                                options.GatewayPort = gatewayPort;
                            });
                    }

                    siloBuilder.ConfigureServices(s =>
                    {
                        s.AddSingleton<IGrainStateSerializer, GrainStateSerializer>();
                        s.AddSingleton<IConfigurationServiceClient, ConfigurationServiceClient>();
                        s.AddSingleton<IConfigurationsProvider, ConfigurationServiceClient>();
                        s.AddSingleton<GameConfigProvider>();
                    })
                    .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PlayerGrain).Assembly).WithReferences())
                    .AddMongoDBGrainStorage(Constants.GrainStorageProviderName, options =>
                    {
                        var connectionString = hostContext.Configuration.GetValue<string>("Meta:Back:GrainStorage:ConnectionString");
                        //https://github.com/OrleansContrib/Orleans.Providers.MongoDB/pull/64
                        var connectionUrl = MongoUrl.Create(connectionString);
                        options.ConnectionString = connectionString;
                        options.DatabaseName = connectionUrl.DatabaseName;
                    })
                    .ConfigureServices(x =>
                    {
                        x.AddTransient<IConfigurationValidator, EmptyValidator>();
                    })
                    .AddGrainService<ConfigurationGrainService>()
                    .AddStartupTask<BootstrapTask>();
                }).Build();
            return host;
        }

        public class EmptyValidator : IConfigurationValidator
        {
            public void ValidateConfiguration()
            {
                //https://github.com/OrleansContrib/Orleans.Providers.MongoDB/issues/67
            }
        }
    }
}