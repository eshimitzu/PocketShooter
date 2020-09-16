using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Server
{
    internal class GameBackgroundService : BackgroundService, IMatchMakedObserver
    {
        public void OnMatchMaked(MatchMakingResultData result, Immutable<PlayerId[]> players)
        {
            logger.LogInformation("Match is ready for {players} players", players.Value.Aggregate("", (acc, e) => acc + ',' + e));
            var match = players.Value.Select(x => x.ToString()).ToList();
            hubContext.Clients.Users(match).SendAsync(nameof(IGameHubObserver.MatchMaked), result);
        }

        private readonly IHubContext<GameHub> hubContext;
        private readonly ILogger<GameBackgroundService> logger;
        private readonly IClusterClient clusterClient;

        private Timer timer;

        public GameBackgroundService(IHubContext<GameHub> hubContext, ILogger<GameBackgroundService> logger, IClusterClient clusterClient)
        {
            this.hubContext = hubContext;
            this.logger = logger;
            this.clusterClient = clusterClient;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(async _ => await NotifyMatches(_), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        private async Task NotifyMatches(object state)
        {
            if (clusterClient.IsInitialized)
            {
                logger.LogInformation("Started to listen for matches are ready");
                var grain = clusterClient.GetGrain<IMatchMakingGrain>(Guid.Empty);
                var subscriber = await clusterClient.CreateObjectReference<IMatchMakedObserver>(this);
                await grain.SubscribeToMatches(subscriber);
                logger.LogInformation("Listen for matches");
                timer.Dispose();
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           return Task.CompletedTask;
        }
    }
}
