using System;
using System.Linq;
using System.Net;
using Heyworks.PocketShooter.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Serilog;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public sealed class MetaClusterClientBuilder
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MetaClusterClientBuilder> logger;
        private readonly IClientBuilder clientBuilder;
        private readonly Microsoft.Extensions.Hosting.IHostingEnvironment hosting;

        public MetaClusterClientBuilder(
            IConfiguration configuration,
            ILogger<MetaClusterClientBuilder> logger,
            Microsoft.Extensions.Hosting.IHostingEnvironment hosting)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.hosting = hosting.NotNull();
            this.clientBuilder = CreateInitialBuilderSetup();
        }

        public void ConfigureApplicationParts(Action<IApplicationPartManager> configure) =>
            clientBuilder.ConfigureApplicationParts(configure);

        public IClusterClient Build() => clientBuilder.Build();

        private IClientBuilder CreateInitialBuilderSetup()
        {
            var gatewayPort = configuration.GetValue<int>("Meta:Front:StaticGateway:GatewayPort");

            IClientBuilder builder = new ClientBuilder()
                .Configure<ClusterOptions>(configuration.GetSection("Meta:Cluster"))
                .ConfigureApplicationParts(parts =>
                {
                    // TODO: add ping here
                    // parts.AddApplicationPart(typeof(IIAmAlive).Assembly);
                })
               .ConfigureLogging(logging =>
               {
                   logging.AddSerilog(dispose: true);
               });

            if (hosting.IsLocalOrDevelopment())
            {
                return builder.UseLocalhostClustering(gatewayPort: gatewayPort);
            }
            else
            {
                var ipConfig = configuration.GetValue<string>("Meta:Front:StaticGateway:AdvertisedIP");
                var ip = string.IsNullOrEmpty(ipConfig) ? IPUtils.GetLocalIPAddresses().First() : IPAddress.Parse(ipConfig);

                return builder.UseStaticClustering(new IPEndPoint(ip, gatewayPort));
            }
        }
    }
}
