using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Concurrency.Fibers;
using Heyworks.PocketShooter.Meta.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Hosting
{
    public class MetaServerBackService : IHostedService
    {
        private ILogger logger;
        private MetaServerBack metaServerBack;
        private IHostingEnvironment hostingEnvironment;
        private IHost host;

        public MetaServerBackService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            host = new MetaServerBack().Create(Environment.GetCommandLineArgs(), hostingEnvironment.ContentRootPath);
            await host.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await host.StopAsync();
    }
}
