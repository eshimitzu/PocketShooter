using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Concurrency.Fibers;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Server;
using Heyworks.PocketShooter.Realtime.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Hosting
{
    public class MetaServerFrontService : IHostedService
    {
        private IWebHost host;
        private ILogger<MetaServerFrontService> logger;

        public MetaServerFrontService(ILogger<MetaServerFrontService> logger)
        {
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var webBuilder = new MetaServerFront().Create(Environment.GetCommandLineArgs(), Directory.GetCurrentDirectory());
            host = webBuilder;
            await webBuilder.StartAsync();
            var address = webBuilder.ServerFeatures.Single(x => x.Value is IServerAddressesFeature).Value as IServerAddressesFeature;
            logger.LogInformation("Meta.Server have started at {Address}", address.Addresses.First());
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await host.StopAsync();
    }
}
