using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public sealed class MetaClusterClientService : IHostedService
    {
        private readonly IClusterClient clusterClient;
        private readonly ILogger logger;

        public MetaClusterClientService(IClusterClient clusterClient, ILogger<MetaClusterClientService> logger)
        {
            this.clusterClient = clusterClient;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var attempt = 0;
            var maxAttempts = 5;

            return clusterClient.Connect(async error =>
            {
                if (++attempt < maxAttempts)
                {
                    logger.LogWarning(
                        error,
                        "Failed to connect to Pocket Shooter cluster on attempt {Attempt} of {MaxAttempts}.",
                        attempt,
                        maxAttempts);

                    await Task.Delay(TimeSpan.FromSeconds(attempt));

                    return true;
                }
                else
                {
                    logger.LogError(
                        error,
                        "Failed to connect to Pocket Shooter cluster on attempt {Attempt} of {MaxAttempts}.",
                        attempt,
                        maxAttempts);

                    return false;
                }
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return clusterClient.Close();
        }
    }
}
