using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Heyworks.PocketShooter.Realtime.Runtime
{
    public class GameManagementRuntime : IHostedService, IDisposable
    {
        private readonly IGameManagementService implemetation;

        public GameManagementRuntime(IGameManagementService implemetation)
        {
            this.implemetation = implemetation;
        }

        public Task StartAsync(CancellationToken cancellationToken) => implemetation.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => implemetation.StopAsync(cancellationToken);

        public void Dispose() => implemetation.Dispose();
    }
}
