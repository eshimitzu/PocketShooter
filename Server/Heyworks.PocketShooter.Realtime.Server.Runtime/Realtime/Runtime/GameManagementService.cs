using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Gameplay;
using Heyworks.PocketShooter.Realtime.Meta;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using MoreLinq;
using Microsoft.Extensions.Options;

namespace Heyworks.PocketShooter.Realtime.Runtime
{
    internal sealed class GameManagementService : IGameManagementService
    {
        private sealed class MonitorRoomsMessage : IManagementMessage
        {
            static MonitorRoomsMessage() => Instance = new MonitorRoomsMessage();

            private MonitorRoomsMessage()
            {
            }

            public static MonitorRoomsMessage Instance { get; }
        }

        private sealed class BotControlMessage : IManagementMessage
        {
            static BotControlMessage() => Instance = new BotControlMessage();

            private BotControlMessage()
            {
            }

            public static BotControlMessage Instance { get; }
        }

        private sealed class PublishRoomsMessage : IManagementMessage
        {
            static PublishRoomsMessage() => Instance = new PublishRoomsMessage();

            private PublishRoomsMessage()
            {
            }

            public static PublishRoomsMessage Instance { get; }
        }



        private readonly IDictionary<RoomId, (Room Room, GameRoomData RoomData, IList<PlayerInfo> AvailableBots)> rooms =
            new Dictionary<RoomId, (Room, GameRoomData, IList<PlayerInfo>)>();

        private readonly IDictionary<PlayerId, (RoomId RoomId, PlayerInfo PlayerInfo)> players =
            new Dictionary<PlayerId, (RoomId, PlayerInfo)>();

        private readonly IClusterClient metaClusterClient;
        private readonly INetworkConfiguration networkConfiguration;
        private readonly IOptionsMonitor<RealtimeConfiguration> configuration;
        private readonly GameManagementChannel gameManagementChannel;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        // Hold reference to MatchMakingObserver to prevent GC from Orleans cluster client.
        private readonly MatchMakingObserver matchMakingObserver;

        private IPEndPoint serverAddress;
        private Timer publishRoomsTimer;
        private Timer botControlTimer;
        private Timer monitorRoomsTimer;
        private Timer pingRoomsTimer;

        public GameManagementService(
            IClusterClient metaClusterClient,
            INetworkConfiguration networkConfiguration,
            GameManagementChannel gameManagementChannel,
            ILoggerFactory loggerFactory,
            IOptionsMonitor<RealtimeConfiguration> configuration)
        {
            this.metaClusterClient = metaClusterClient;
            this.networkConfiguration = networkConfiguration;
            this.configuration = configuration;

            this.gameManagementChannel = gameManagementChannel;
            this.matchMakingObserver = new MatchMakingObserver(gameManagementChannel);

            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<GameManagementService>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting Game Management Service.");

            serverAddress = await networkConfiguration.GetPublicIPAddress();

            logger.LogInformation("Realtime server public address {serverAddress} obtained", serverAddress);

            var matchMakingObservable = metaClusterClient.GetGrain<IMatchMakingObservableGrain>(Guid.Empty);
            var observerRef = await metaClusterClient.CreateObjectReference<IMatchMakingObserver>(matchMakingObserver);

            StaySubscribedToMeta(matchMakingObservable, observerRef, serverAddress);

            StartManagingGames(gameManagementChannel.Reader);

            monitorRoomsTimer = new Timer(
                state => gameManagementChannel.SendMessage(MonitorRoomsMessage.Instance),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(10));

            publishRoomsTimer = new Timer(
                state => gameManagementChannel.SendMessage(PublishRoomsMessage.Instance),
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10));

            botControlTimer = new Timer(
                state => gameManagementChannel.SendMessage(BotControlMessage.Instance),
                null,
                TimeSpan.FromSeconds(5),
                period: TimeSpan.FromSeconds(1));

            logger.LogInformation("Game Management Service is started.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Game Rooms Management Service is stopping.");

            monitorRoomsTimer?.Change(Timeout.Infinite, 0);
            publishRoomsTimer?.Change(Timeout.Infinite, 0);
            botControlTimer?.Change(Timeout.Infinite, 0);
            gameManagementChannel.Complete();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            pingRoomsTimer?.Dispose();
            monitorRoomsTimer?.Dispose();
            publishRoomsTimer?.Dispose();
            botControlTimer?.Dispose();
        }

        private async Task StaySubscribedToMeta(
            IMatchMakingObservableGrain matchMakingObservable,
            IMatchMakingObserver observerRef,
            IPEndPoint observerAddress)
        {
            while (true)
            {
                try
                {
                    await matchMakingObservable.Subscribe(observerRef, observerAddress);
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Exception while trying to subscribe to meta cluster.");
                }
            }
        }

        private async Task StartManagingGames(ChannelReader<IManagementMessage> channelReader)
        {
            // TODO: YGR. Add CancellationToken
            while (await channelReader.WaitToReadAsync())
            {
                // TODO: YGR. Add CancellationToken
                while (channelReader.TryRead(out var message))
                {
                    if (message is IHasRoomIdMessage hrim &&
                        !rooms.ContainsKey(hrim.RoomId))
                    {
                        // case when room was closed and/or removed but meta doesn't know about it yet.
                        // Need to think about better design to handle it.
                        logger.LogWarning("Can't handle message {message}. Room with id {roomId} does not exists.", message.GetType().Name, hrim.RoomId);
                        continue;
                    }
                    try
                    {
                        switch (message)
                        {
                            case StartGameMessage sgm:
                                var room = rooms[sgm.RoomId];
                                if (!room.RoomData.IsStarted)
                                {
                                    room.RoomData.IsRented = true;
                                    room.Room.StartGame(sgm.Request.ModeInfo);
                                    room.RoomData.StartedAt = DateTime.UtcNow;
                                    room.RoomData.MatchType = sgm.Request.MatchType;
                                    room.RoomData.Map = sgm.Request.Map;
                                    room.RoomData.DesiredBots = sgm.Request.DesiredBots;
                                    room.RoomData.InitialRealPlayers = sgm.Request.InitialRealPlayers;
                                }

                                logger.LogInformation("Game in room with id {roomId} is started", sgm.RoomId);
                                break;
                            case JoinServerMessage jsm:
                                if (!players.ContainsKey(jsm.PlayerInfo.Id))
                                {
                                    players.Add(jsm.PlayerInfo.Id, (jsm.RoomId, jsm.PlayerInfo));
                                    logger.LogInformation("Player {playerId} added to server. Attached room id {roomId}", jsm.PlayerInfo.Id, jsm.RoomId);
                                }
                                else
                                {
                                    var player = players[jsm.PlayerInfo.Id];
                                    if (player.RoomId != jsm.RoomId)
                                    {
                                        players[player.PlayerInfo.Id] = (jsm.RoomId, jsm.PlayerInfo);
                                    }
                                }

                                break;
                            case AddBotMessage abm:
                                rooms[abm.RoomId].AvailableBots.Add(abm.BotInfo);
                                logger.LogInformation("Bot prototype {botId} added into {roomId} room", abm.BotInfo.Id, abm.RoomId);
                                break;
                            case LeaveServerMessage lsm:
                                {
                                    var playerData = players[lsm.PlayerId];
                                    players.Remove(lsm.PlayerId);
                                    var roomData = rooms[playerData.RoomId];
                                    await roomData.Room.SendMessage(new LeaveGameMessage(lsm.PlayerId));

                                    logger.LogInformation(
                                        "Player {playerId} removed from server. Attached room id {roomId}", lsm.PlayerId, roomData.RoomData.RoomId);

                                    break;
                                }
                            case JoinRoomMessage jrm:
                                {
                                    if (players.ContainsKey(jrm.PlayerId))
                                    {
                                        var playerData = players[jrm.PlayerId];
                                        var roomData = rooms[playerData.RoomId];
                                        roomData.RoomData.Players.Add(jrm.PlayerId);

                                        await roomData.Room.SendMessage(new JoinGameMessage(playerData.PlayerInfo, jrm.PlayerChannel));

                                        logger.LogInformation(
                                            "Player {playerId} added to room {roomId}", jrm.PlayerId, roomData.RoomData.RoomId);

                                        if (roomData.RoomData.MatchType == MatchType.DominationBots)
                                        {
                                            for (var i = 2; i <= 10; i++)
                                                gameManagementChannel.SendMessage(new RequestBotControlMessage(jrm.PlayerId));
                                        }
                                    }
                                    else
                                    {
                                        // TODO: send not authorized message to players. await jrm.PlayerChannel.WriteAsync();
                                        logger.LogWarning("Can't join player {playerId} to room. Player is not authorized on server.", jrm.PlayerId);
                                    }
                                    break;
                                }
                            case BotControlMessage bcm:
                                {
                                    foreach (var roomForBots in rooms)
                                    {
                                        var roomDataForBots = roomForBots.Value.RoomData;
                                        if (roomDataForBots.Players.Count > 0 && roomDataForBots.DesiredBots > 0
                                            &&
                                            (roomDataForBots.InitialRealPlayers == roomDataForBots.Players.Count // better algorithm would be add bots as people arrive, but that more complex and needs counter maitanance
                                                ||
                                                roomDataForBots.StartedAt > DateTime.UtcNow.AddMilliseconds(-7_000)) // what if some players failed to join?
                                            )
                                        {
                                            // assign bots to players randomly as these arrive, more clever would be by device
                                            while (roomDataForBots.DesiredBots > 0)
                                            {
                                                var player = roomDataForBots.Players.RandomSubset(1).Single();
                                                logger.LogWarning("Sending bot control to {player}", player);
                                                gameManagementChannel.SendMessage(new RequestBotControlMessage(player));
                                                roomDataForBots.DesiredBots--;
                                            }
                                        }
                                    }
                                }
                                break;

                            case CloseRoomMessage crm:
                                {
                                    rooms.Remove(crm.RoomId);
                                    var playersInRoom = players.Values
                                        .Where(item => item.RoomId == crm.RoomId)
                                        .Select(item => item.PlayerInfo)
                                        .ToList();

                                    foreach (var playerInfo in playersInRoom)
                                    {
                                        players.Remove(playerInfo.Id);
                                    }

                                    logger.LogInformation("Room {roomId} removed from server", crm.RoomId);

                                    break;
                                }
                            case RequestBotControlMessage rbcm:
                                {
                                    var playerData = players[rbcm.PlayerId];
                                    var roomData = rooms[playerData.RoomId];
                                    var botInfo = roomData.AvailableBots.First();

                                    await roomData.Room.SendMessage(new TakeBotControlMessage(rbcm.PlayerId, botInfo));

                                    roomData.RoomData.Bots.Add(botInfo.Id);
                                    roomData.AvailableBots.Remove(botInfo);

                                    logger.LogInformation(
                                        "Player {playerId} requested bot control in room {roomId}", rbcm.PlayerId, roomData.RoomData.RoomId);

                                    break;
                                }
                            case ApplyMatchResultsMessage amrm:
                                {
                                    var resultsData = amrm.MatchResults
                                        .Select(item =>
                                        new PlayerMatchResultsData
                                        {
                                            PlayerId = item.PlayerId,
                                            Nickname = item.Nickname,
                                            TeamNo = item.TeamNo,
                                            TrooperClass = item.TrooperClass,
                                            CurrentWeapon = item.CurrentWeapon,
                                            Kills = item.MatchStats.Kills,
                                            Deaths = item.MatchStats.Deaths,
                                            IsWinner = item.IsWinner,
                                            IsDraw = item.IsDraw,
                                            IsBot = item.IsBot,
                                        })
                                        .ToList();

                                    await ApplyMatchResults(amrm.RoomId, resultsData);
                                }
                                break;
                            case UpdateConsumablesMessage ucm:
                                await UpdateConsumables(ucm.PlayerId, ucm.UsedOffensives, ucm.UsedSupports);
                                break;
                            case MonitorRoomsMessage mrm:
                                MonitorRooms();
                                break;
                            case PublishRoomsMessage prm:
                                await PublishRooms();
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Exception during processing messages");
                    }
                }
            }
        }

        private async Task ApplyMatchResults(Guid roomId, IList<PlayerMatchResultsData> resultsData)
        {
            var grain = metaClusterClient.GetGrain<IMatchResultsPublisherGrain>(roomId);
            await grain.ApplyMatchResults(resultsData.AsImmutable());
        }

        private async Task UpdateConsumables(Guid playerId, int usedOffensives, int usedSupports)
        {
            var grain = metaClusterClient.GetGrain<IConsumablesPublisherGrain>(playerId);
            await grain.UpdateConsumables(usedOffensives, usedSupports);
        }

        private void MonitorRooms()
        {
            while (rooms.Count < configuration.CurrentValue.MaximalRoomCount)
            {
                var roomId = Guid.NewGuid();
                var roomLogger = loggerFactory.CreateLogger($"{typeof(Room).FullName}.<{roomId}>");
                var newRoom = new Room(roomId, gameManagementChannel, roomLogger);
                var rand = new Random(DateTime.Now.Millisecond).Next(int.MaxValue);
                rooms.Add(
                    roomId,
                    (Room: newRoom,
                        RoomData: new GameRoomData
                        {
                            ServerAddress = serverAddress,
                            RoomId = roomId,
                            // HOTFIX PSH-851
                            MatchType = MatchType.DominationFast,
                        },
                        AvailableBots: new List<PlayerInfo>()
                    ));

                logger.LogInformation("New room with id {roomId} created", roomId);
            }
        }

        private async Task PublishRooms()
        {
            // TODO: optimize and send only changed room, send them out of main loop in background to avoid block
            var gameRoomsGrain = metaClusterClient.GetGrain<IGameRoomsPublisherGrain>(Guid.Empty);
            await gameRoomsGrain.PublishRoomData(
                rooms
                .Select(item => item.Value.RoomData)
                .ToList()
                .AsImmutable());
        }
    }
}
