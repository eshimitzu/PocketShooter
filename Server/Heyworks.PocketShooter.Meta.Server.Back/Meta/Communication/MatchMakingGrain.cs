using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Services;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class MatchMakingGrain : Grain, IMatchMakingGrain, IMatchMakingObservableGrain
    {
        private readonly IDictionary<IPEndPoint, IMatchMakingObserver> observers = new Dictionary<IPEndPoint, IMatchMakingObserver>();

        private readonly IConfigurationsProvider configurationsProvider;
        private readonly ILogger<MatchMakingGrain> logger;
        private List<IMatchMakedObserver> fronts = new List<IMatchMakedObserver>();

        private MatchMakingQueue dominationModeQueue;

        private DateTime previousMatchingTime = DateTime.MinValue;

        public MatchMakingGrain(IConfigurationsProvider configurationsProvider, ILogger<MatchMakingGrain> logger)
        {
            this.configurationsProvider = configurationsProvider;
            this.logger = logger;
            this.dominationModeQueue = new MatchMakingQueue();
        }

        public override async Task OnActivateAsync()
        {
            // TODO: get these via stream - i.e. no calls when no needed
            var (matchingConfig, _) = await configurationsProvider.GetMatchMakingConfiguration();
            logger.LogInformation("Starting match making timer with {timer}ms", matchingConfig.ForcedStartMs);
            RegisterTimer(OnMatchTimer, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(matchingConfig.ForcedStartMs));
        }

        private async Task OnMatchTimer(object arg)
        {
            if (dominationModeQueue.Any())
            {
                var (matchingConfig, mapSelectors) = await configurationsProvider.GetMatchMakingConfiguration();
                DelayDeactivation(TimeSpan.FromMilliseconds(matchingConfig.ForcedStartMs));
                var dominationModeInfoGrain = GrainFactory.GetGrain<IDominationModeInfoGrain>(Guid.Empty);
                var maxPlayers = await dominationModeInfoGrain.GetModeLimits();
                while (dominationModeQueue.Any())
                {
                    var (level, group) = dominationModeQueue.Pop(maxPlayers, matchingConfig);
                    if (group.Any())
                    {
                        logger.LogInformation("Found new group of {PlayerCount} to play", group.Count);
                        var gameRoomsProvider = GrainFactory.GetGrain<IGameRoomsProviderGrain>(Guid.Empty);

                        MapNames map = GetMap(mapSelectors, level);

                        var roomData = await gameRoomsProvider.RentRoom(MatchType.Domination, map);
                        var realtimeServer = roomData.ServerAddress;
                        var observer = observers[realtimeServer];
                        var modeInfo = (await dominationModeInfoGrain.GetDominationModeInfo(map)).Value;
                        var gameStartInfo = new GameStartRequest(roomData.RoomId, modeInfo, MatchType.Domination, group.Count, modeInfo.MaxPlayers - group.Count, map);
                        observer.StartGame(gameStartInfo);

                        byte botCount = (byte)(modeInfo.MaxPlayers - group.Count); // byte cast gives additional safety - our server will never create more than 256 bots, but 2B will make it down
                        var averageLevel = (int)group.Average(x => x.Level);
                        await JoinBots(roomData, averageLevel, observer, botCount);

                        // non optimal, but works with current api
                        foreach (var player in group)
                        {
                            var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(player.Player);
                            var playerInfo = await playerGrain.GetPlayerInfo();
                            observer.JoinServer(roomData.RoomId, playerInfo.Value.info);
                            await playerGrain.StartMatch();
                        }

                        logger.LogInformation("Will notify players about their room with {numberOfPlayers}", group.Count);
                        foreach (var front in fronts)
                        {
                            front.OnMatchMaked(new MatchMakingResultData(realtimeServer, map), group.Select(_ => _.Player).ToArray().AsImmutable());
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private static MapNames GetMap(IList<Configuration.Data.MapsSelectorConfig> mapSelectors, int level)
        {
            var maps = mapSelectors.Any()
                                                ? mapSelectors.Where(x => x.StartLevel <= level && level <= x.EndLevel).Select(_ => _.Name)
                                                : EnumsNET.Enums.GetMembers<MapNames>().Select(_ => _.Value);
            // NOTE: we do not validate here availability of map for level - that MUST be done when we inject config into a system
            var map = maps.RandomSubset(1).Single();
            return map;
        }

        public async Task<IPEndPoint[]> GetRealtimeServers() => observers.Select(item => item.Key).ToArray();

        public async Task FindRealtimeServer(PlayerId playerId, MatchRequest requestedMatch)
        {
            // TODO: throw exception to client if over capacity
            // TODO: protect from DDOS by sending find server to many times - should protection web in front or back (i.e. on IP level or player grain level?)
            // e.g. if we need to prevent match making for players who left to often then it is grain level

            // NOTE: could log PlayerId and MatchType into Analytics log with Matches Category
            logger.LogInformation("Player {PlayerId} requested {MatchType} match making on {Map}", playerId, requestedMatch.Match, requestedMatch.MapName);
            switch (requestedMatch.Match)
            {
                case MatchType.Domination:
                    await DispatchDomination(playerId);
                    break;
                case MatchType.DominationFast:
                    await DoGame(playerId, requestedMatch.Match, requestedMatch.MapName.HasValue ? requestedMatch.MapName.Value : default(MapNames));
                    break;
                case MatchType.DominationBots:
                    await DoGame(playerId, requestedMatch.Match, requestedMatch.MapName.HasValue ? requestedMatch.MapName.Value : default(MapNames));
                    break;
            }
        }

        private async Task DispatchDomination(PlayerId playerId)
        {
            var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(playerId);
            var matchMakingData = await playerGrain.GetMatchMakingData();
            var (config, selectors) = await configurationsProvider.GetMatchMakingConfiguration();
            if (matchMakingData.LearningMeter < config.LearningMeterBeforePvP)
            {
                logger.LogInformation("Put {player} into game with bots as of {learning}", playerId, matchMakingData.LearningMeter);
                MapNames map = GetMap(selectors, matchMakingData.Level);
                await DoGame(playerId, MatchType.DominationBots, map);
            }
            else
            {
                logger.LogInformation("Put {player} into queue", playerId);
                dominationModeQueue.Add(new QueuedPlayer(playerId, DateTime.UtcNow, matchMakingData.Level));
            }
        }

        private async Task DoGame(PlayerId playerId, MatchType matchType, MapNames map)
        {
            var gameRoomsProvider = GrainFactory.GetGrain<IGameRoomsProviderGrain>(Guid.Empty);
            var roomData = await gameRoomsProvider.RentRoom(matchType, map);

            var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(playerId);
            var player = await playerGrain.GetPlayerInfo();
            try
            {
                await RoutePlayerIntoRoom(playerId, matchType, map, roomData, playerGrain, player);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get {playerId} player into {roomData.RoomId}. See innter exception for detail.", ex);
            }

        }

        private async Task RoutePlayerIntoRoom(PlayerId playerId, MatchType matchType, MapNames map, GameRoomData roomData, IPlayerGrain playerGrain, Immutable<(int accountLevel, Realtime.Data.PlayerInfo info)> player)
        {
            var (level, info) = player.Value;

            var realtimeServer = roomData.ServerAddress;
            var observer = observers[realtimeServer];

            if (!roomData.IsStarted)
            {
                var dominationModeInfoGrain = GrainFactory.GetGrain<IDominationModeInfoGrain>(Guid.Empty);
                var modeInfo = (await dominationModeInfoGrain.GetDominationModeInfo(map)).Value;
                logger.LogDebug("Asking realtime to start game in {room}", roomData.RoomId);
                observer.StartGame(new GameStartRequest(roomData.RoomId, modeInfo, matchType, 1, 0, map));
                var botCount = modeInfo.MaxPlayers - 1;
                await JoinBots(roomData, level, observer, botCount);
            }

            observer.JoinServer(roomData.RoomId, info);
            await playerGrain.StartMatch();

            foreach (var front in fronts)
            {
                logger.LogInformation("Notifying {player} about match", playerId);
                front.OnMatchMaked(new MatchMakingResultData(realtimeServer, map), new[] { playerId }.AsImmutable());
            }
        }

        private async Task JoinBots(GameRoomData roomData, int level, IMatchMakingObserver observer, int botCount)
        {
            var botsGrain = GrainFactory.GetGrain<IBotsGrain>(Guid.Empty);
            var bots = await botsGrain.GetBotPrototypes(botCount, level, true);
            bots.Value.ForEach(_ => observer.AddBot(roomData.RoomId, _));
        }

        Task IMatchMakingObservableGrain.Subscribe(IMatchMakingObserver observer, IPEndPoint observerAddress)
        {
            if (!observers.ContainsKey(observerAddress))
            {
                observers.Add(observerAddress, observer);

                logger.LogInformation("Realtime observer registered with address {observerAddress}", observerAddress);
            }

            return Task.CompletedTask;
        }

        public Task SubscribeToMatches(IMatchMakedObserver observer)
        {
            fronts.Add(observer);
            return Task.CompletedTask;
        }
    }
}