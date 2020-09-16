using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Gameplay
{
    /// <summary>
    /// Represents a game room.
    /// </summary>
    public sealed class Room
    {
        private sealed class TickMessage : IMessage
        {
            static TickMessage() => Instance = new TickMessage();

            private TickMessage()
            {
            }

            public static TickMessage Instance { get; }
        }

        private readonly RoomId roomId;
        private readonly IGameManagementChannel gameManagementChannel;
        private readonly ILogger logger;

        private readonly IRefList<GameState> gameStateProvider;
        private readonly Stopwatch timer;
        private readonly Ticker ticker;

        private readonly SafeCyclicSequenceBuffer<PooledList<IGameCommandData>> commandSequenceBuffer;
        private readonly CyclicIdPool idPool;
        private readonly PooledList<IServiceMessage> serviceMessages = new PooledList<IServiceMessage>();

        // TODO: Move logic to RoomPlayer class or out of room to channel layer.
        private readonly IDictionary<PlayerId, SafeCyclicSequence> playerToHandledCommands = new Dictionary<PlayerId, SafeCyclicSequence>();

        private readonly IDictionary<PlayerId, RoomPlayer> players = new Dictionary<PlayerId, RoomPlayer>();
        private readonly IDictionary<BotId, RoomBot> bots = new Dictionary<BotId, RoomBot>();

        private readonly Channel<IMessage> roomChannel;
        private readonly CancellationTokenSource stopGameCancellationSource;

        private DominationModeInfo modeInfo;
        private Game game;
        private ISimulation gameSimulation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="roomId">The room id.</param>
        /// <param name="gameManagementChannel">The game management channel.</param>
        /// <param name="logger">The logger factory.</param>
        public Room(
            RoomId roomId,
            IGameManagementChannel gameManagementChannel,
            ILogger logger)
        {
            this.roomId = roomId;
            this.gameManagementChannel = gameManagementChannel;
            this.logger = logger;

            var playerIds = Enumerable.Range(1, EntityId.MaxValue).Select(item => (EntityId)item).ToArray();
            this.idPool = new CyclicIdPool(playerIds);

            this.timer = new Stopwatch();
            this.ticker = new Ticker();

            this.stopGameCancellationSource = new CancellationTokenSource();

            this.roomChannel = Channel.CreateUnbounded<IMessage>(
               new UnboundedChannelOptions
               {
                   SingleReader = true,
                   SingleWriter = false,
               });

            this.gameStateProvider = new CyclicSequenceBuffer<GameState>(Constants.BufferSize);
            this.commandSequenceBuffer = new SafeCyclicSequenceBuffer<PooledList<IGameCommandData>>(Constants.BufferSize);
        }

        public void StartGame(DominationModeInfo modeInfo)
        {
            this.modeInfo = modeInfo;
            gameStateProvider.Insert(0, GameState.Create(modeInfo));

            this.game = new Game(new GameRef(gameStateProvider, 0), modeInfo, logger);
            this.gameSimulation = new GameSimulation(game, modeInfo, commandSequenceBuffer, ticker, logger);

            Task.Run(() => RunGameLoop(roomChannel, stopGameCancellationSource.Token));
            Task.Run(() => StartReadMessages(roomChannel, stopGameCancellationSource.Token));
        }

        public ValueTask SendMessage(IMessage message) => roomChannel.Writer.WriteAsync(message, stopGameCancellationSource.Token);

        private async Task RunGameLoop(ChannelWriter<IMessage> сhannelWriter, CancellationToken cancellationToken)
        {
            timer.Start();
            logger.LogDebug("Game timer started at (system: {systemTime}, timer: {timer}) ms", Environment.TickCount, timer.ElapsedMilliseconds);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(Constants.TickIntervalMs, cancellationToken);
                    await сhannelWriter.WriteAsync(TickMessage.Instance, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            timer.Stop();
            logger.LogDebug("Game timer stopped at (system: {systemTime}, timer: {timer}) ms", Environment.TickCount, timer.ElapsedMilliseconds);
        }

        private async Task StartReadMessages(ChannelReader<IMessage> сhannelReader, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && await сhannelReader.WaitToReadAsync(cancellationToken))
            {
                while (!cancellationToken.IsCancellationRequested && сhannelReader.TryRead(out var theMessage))
                {
                    try
                    {
                        ProcessMessage(theMessage);
                    }
                    catch (ClientException ex)
                    {
                        var msg = new ServerErrorMessage(ex);

                        // assume channel exists because we still process its messages in single thread
                        var player = players.Values.First(item => item.TrooperId == ex.TrooperId);
                        await player.SendMessage(msg);
                    }
                    catch (ServerException ex)
                    {
                        logger.LogError(ex, "Handled exception during game simulation.");
                        var msg = new ServerErrorMessage(ex);
                        foreach (var player in players.Values)
                        {
                            await player.SendMessage(msg);
                        }

                        DebugExit();
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex, "Unhandled exception during game simulation.");
                        var msg = new ServerErrorMessage(new ServerException(ClientErrorCode.SomethingWrongHappenedAndWeDoNotKnowWhat));
                        foreach (var player in players.Values)
                        {
                            await player.SendMessage(msg);
                        }

                        DebugExit();
                    }
                }
            }

            logger.LogDebug("Channel read cycle is done");
        }

        private void Tick()
        {
            var elapsedSinceLastUpdateMs = timer.ElapsedMilliseconds - ticker.Current * Constants.TickIntervalMs;

            logger.LogDebug("Elapsed ms since last update {elapsedSinceLastUpdateMs}", elapsedSinceLastUpdateMs);

            if (elapsedSinceLastUpdateMs / Constants.TickIntervalMs >= Constants.BufferSize)
                logger.LogError("Room lagged {ticks} ticks. Must not happen ever", elapsedSinceLastUpdateMs / Constants.TickIntervalMs);

            while (elapsedSinceLastUpdateMs >= Constants.TickIntervalMs)
            {
                ticker.Tick();
                logger.LogDebug("Update on tick {tick} at (system: {systemTime}, timer: {timer}) ms", ticker.Current, Environment.TickCount, timer.ElapsedMilliseconds);
                Update();
                elapsedSinceLastUpdateMs -= Constants.TickIntervalMs;
            }
        }

        [Conditional("DEBUG")]
        private void DebugExit()
        {
            Thread.Sleep(2000);
            Environment.Exit(42);
        }

        private void StopGame()
        {
            stopGameCancellationSource.Cancel();
            roomChannel.Writer.Complete();

            players.Clear();
            playerToHandledCommands.Clear();

            bots.Clear();

            gameManagementChannel.CloseRoom(roomId);
        }

        private void Update()
        {
            var gameState = game.GetState();
            gameStateProvider.Insert(ticker.Current, in gameState);
            game.ApplyState(new GameRef(gameStateProvider, ticker.Current));

            // collect changes

            // we must process service commands changing world
            // and send them in same tick
            // also order of world changing commands and simulation commands may vary
            try
            {
                foreach (var serviceMessage in serviceMessages)
                {
                    ProcessServiceMessage(serviceMessage);
                }
            }
            catch
            {
                // we could handle errors on message by message basis - and send client errors to specific clients and handle other case
                // e.g. like here https://github.com/dnikolovv/dev-adventures-realworld or https://github.com/Resultful/Resultful
                // i.e. these works well on per client error handling (error as data) and of cheaper than throw error
                throw;
            }
            finally
            {
                // we clear hear messages to avoid handling erroneous next time
                serviceMessages.Clear();
            }

            gameSimulation.Update();

            BroadcastSimulationState(ticker.Current);

            if (game.IsEnded)
            {
                BroadcastEndGame();
                UpdateConsumables();
                ApplyMatchResults();
                StopGame();
            }
        }

        private void ProcessMessage(IMessage message)
        {
            switch (message)
            {
                case TickMessage tick:
                    Tick();
                    break;
                case PingMessage pmsg:
                    {
                        if (players.TryGetValue(pmsg.PlayerId, out var player))
                        {
                            logger.LogTrace("Player {PlayerId} confirmed {LastConfirmedTick} in {ClientTick}", pmsg.PlayerId, pmsg.Ping.ConfirmedTick, pmsg.Ping.Tick);
                            player.ConfirmTick(pmsg.Ping.ConfirmedTick);
                            player.RegisterLastCommandTick(pmsg.Ping.Tick);

                            var clientCommands = playerToHandledCommands[player.Id];

                            foreach (var command in pmsg.Commands)
                                if (!clientCommands.ContainsKey(command.Tick))
                                    ProcessSimulationMessage(command.Tick, command.GameCommandData);

                            // NOTE: avoids double command application (not most efficient - but robust before demo)
                            // TODO: consider moving this out of room onto channel layer (so room gets only `correct` network state)
                            foreach (var command in pmsg.Commands)
                                clientCommands.Insert(command.Tick);
                        }
                        else
                        {
                            logger.LogInformation("Player {PlayerId} not found, but remaining pings received", pmsg.PlayerId);
                        }

                        break;
                    }
                case IServiceMessage serviceMessage:
                    serviceMessages.Add(serviceMessage);
                    break;
                default:
                    throw new NotImplementedException($"The message of type {message.GetType()} is not supported");
            }
        }

        private void ProcessServiceMessage(IServiceMessage message)
        {
            switch (message)
            {
                case JoinGameMessage jgm:
                    {

                        // case when disconnected player returning to the same room with new connection.
                        if (players.TryGetValue(jgm.PlayerInfo.Id, out var roomPlayer))
                        {
                            RemovePlayer(jgm.PlayerInfo.Id);
                        }

                        var player = AddPlayer(jgm.PlayerInfo, jgm.PlayerChannel);

                        player.SendMessage(new GameJoinedMessage(
                            modeInfo, player.TeamNo, player.Info, player.TrooperId, ticker.Current, roomChannel, roomId));

                        break;
                    }
                case TakeBotControlMessage tbcm:
                    {
                        var bot = AddBot(tbcm.BotInfo);
                        var player = players[tbcm.OwnerId];
                        player.ControlledBots.Add(bot.Id);

                        player.SendMessage(new BotControlTakenMessage(bot.Info, bot.TeamNo, bot.TrooperId));

                        break;
                    }
                case SpawnTrooperMessage stm:
                    {
                        var player = players[stm.PlayerId];
                        game.RespawnTrooper(player.Info.Nickname, player.TrooperId, player.TeamNo, player.GetTrooperInfo(stm.TrooperClass));

                        break;
                    }
                case SpawnBotTrooperMessage sbtm:
                    {
                        var bot = bots[sbtm.BotId];
                        game.RespawnTrooper(bot.Info.Nickname, bot.TrooperId, bot.TeamNo, bot.GetTrooperInfo(sbtm.TrooperClass));

                        break;
                    }
                case LeaveGameMessage lgm:
                    LeaveGame(lgm.PlayerId);
                    break;
                default:
                    throw new NotImplementedException($"The message of type {message.GetType()} is not supported");
            }
        }

        private void ProcessSimulationMessage(int commandTick, IGameCommandData commandData)
        {
            if (commandTick >= ticker.Current
                && commandTick - ticker.Current <= Constants.AcceptedClientTickOffset
                && !commandSequenceBuffer.IsStale(commandTick))
            {
                AddCommand(commandTick, commandData);
            }
            else
            {
                logger.LogDebug(
                    "Skipping client command with tick {commandTick}. Current simulation tick {simulationTick}",
                    commandTick,
                    ticker.Current);
            }
        }

        private void AddCommand(int tick, IGameCommandData command)
        {
            // commands will be hold until out of reach in buffer, could build in resource clean up or pool into API
            if (commandSequenceBuffer.TryGetValue(tick, out PooledList<IGameCommandData> commandSet))
                commandSet.Add(command);
            else
            {
                var newCommandSet = new PooledList<IGameCommandData>(modeInfo.MaxPlayers);
                commandSequenceBuffer.Insert(tick, newCommandSet);
                newCommandSet.Add(command);
            }
        }

        private void LeaveGame(PlayerId playerId)
        {
            var player = players[playerId];

            UpdateConsumables(player.Id, player.TrooperId);

            foreach (var botId in player.ControlledBots)
            {
                RemoveBot(botId);

                // TODO: when player leaves, migrate his bots to another player(s)
                // do something like this:

                // var newOwnerId = GetNewBotOwner();
                // TakeBotControl(newOwnerId, botInfo);
            }

            RemovePlayer(playerId);

            if (!game.Players.Any())
            {
                StopGame();
            }
        }

        private RoomPlayer AddPlayer(PlayerInfo playerInfo, ChannelWriter<IMessage> playerChannel)
        {
            if (players.Count + bots.Count == modeInfo.MaxPlayers)
            {
                throw new ServerException(ClientErrorCode.RoomIsFull, "Room is full");
            }
            else if (idPool.Acquire(out var newTrooperId))
            {
                var player = new RoomPlayer(playerInfo, GetNextTeam(), newTrooperId, playerChannel);
                players.Add(player.Id, player);

                playerToHandledCommands.Add(player.Id, new SafeCyclicSequence(Constants.BufferSize));

                game.MatchResult.AddPlayer(player.TrooperId);
                game.ConsumablesState.AddPlayer(player.TrooperId, playerInfo.Consumables);

                return player;
            }
            else
            {
                var ex = new InvalidOperationException("Can't acquire new id for the player's trooper.");
                logger.LogError("Failed to add player's trooper", ex);

                throw ex;
            }
        }

        private void RemovePlayer(PlayerId playerId)
        {
            var player = players[playerId];

            players.Remove(playerId);
            game.RemoveTrooper(player.TrooperId);
            idPool.Release(player.TrooperId);

            playerToHandledCommands.Remove(playerId);
        }

        private RoomBot AddBot(PlayerInfo botInfo)
        {
            if (players.Count + bots.Count == modeInfo.MaxPlayers)
            {
                throw new ServerException(ClientErrorCode.RoomIsFull, "Room is full");
            }
            else if (idPool.Acquire(out var newTrooperId))
            {
                var bot = new RoomBot(botInfo, GetNextTeam(), newTrooperId);
                bots.Add(bot.Id, bot);

                game.MatchResult.AddPlayer(bot.TrooperId);
                game.ConsumablesState.AddPlayer(bot.TrooperId, botInfo.Consumables);

                return bot;
            }
            else
            {
                var ex = new InvalidOperationException("Can't acquire new id for the bot's trooper.");
                logger.LogError("Failed to add bot's trooper.", ex);

                throw ex;
            }
        }

        private void RemoveBot(BotId botId)
        {
            var bot = bots[botId];

            bots.Remove(botId);
            game.RemoveTrooper(bot.TrooperId);
            idPool.Release(bot.TrooperId);
        }

        private void BroadcastSimulationState(int tickToSend)
        {
            foreach (var player in players.Values)
            {
                if (player.ConfirmedTick > tickToSend)
                {
                    throw new InvalidProgramException(
                        "Tick to send cannot be less than confirmed. Do sanitize game input in player input layer");
                }

                SimulationState? baseline = null;
                if (gameStateProvider.ContainsKey(player.ConfirmedTick))
                {
                    baseline = new SimulationState(player.ConfirmedTick, 0, gameStateProvider[player.ConfirmedTick]);
                }

                int commandBufferSize = player.GetLastCommandTickDelay(tickToSend);
                var updatedState = new SimulationState(tickToSend, commandBufferSize, gameStateProvider[tickToSend]);
                var simulationStateMessage = new SimulationStateMessage(baseline, updatedState);

                player.SendMessage(simulationStateMessage);
            }

            //NOTE: must note copy state into string on each run in production by default as it is slow operation leading to big data
            //NOTE: if this will be needed - create custom system which detects changes in game and streams this into replay queue event storage
            logger.LogTrace("Tick {tick}. GameState {gameState}", tickToSend, new GameRef(gameStateProvider, tickToSend).Value);
        }

        private void BroadcastEndGame()
        {
            foreach (var player in players.Values)
            {
                player.SendMessage(new GameEndedMessage(ticker.Current));
            }

            logger.LogInformation("Game ended in room {roomId} at tick {tick}.", roomId, ticker.Current);
        }

        private void ApplyMatchResults()
        {
            var matchResults = new List<PlayerMatchResults>();
            var winnerTeamNo = game.MatchResult.WinnerTeam;

            foreach (var player in players.Values)
            {
                // HOTFIX: PSH-841
                if (!game.Players.ContainsKey(player.TrooperId))
                    continue;

                var matchStats = game.MatchResult.PlayerStats[player.TrooperId];
                var trooperClass = game.Players[player.TrooperId].TrooperClass;
                var currentWeapon = game.Players[player.TrooperId].CurrentWeapon.Name;
                var consumablesState = game.ConsumablesState.PlayerStats[player.TrooperId];

                matchResults.Add(new PlayerMatchResults(
                    player.Id, player.Info.Nickname, player.TeamNo, winnerTeamNo, trooperClass, matchStats, false, currentWeapon));
            }

            foreach (var bot in bots.Values)
            {
                // HOTFIX: PSH-841
                if (!game.Players.ContainsKey(bot.TrooperId))
                    continue;

                var matchStats = game.MatchResult.PlayerStats[bot.TrooperId];
                var trooperClass = game.Players[bot.TrooperId].TrooperClass;
                var currentWeapon = game.Players[bot.TrooperId].CurrentWeapon.Name;

                matchResults.Add(new PlayerMatchResults(
                    bot.Id, bot.Info.Nickname, bot.TeamNo, winnerTeamNo, trooperClass, matchStats, true, currentWeapon));
            }

            gameManagementChannel.ApplyMatchResults(roomId, matchResults);
        }

        private void UpdateConsumables()
        {
            foreach (var player in players.Values)
            {
                UpdateConsumables(player.Id, player.TrooperId);
            }
        }

        private void UpdateConsumables(PlayerId playerId, EntityId trooperId)
        {
            var consumablesState = game.ConsumablesState.PlayerStats[trooperId];

            gameManagementChannel.UpdateConsumables(playerId, consumablesState.UsedOffensives, consumablesState.UsedSupports);
        }

        private TeamNo GetNextTeam()
        {
            var team1Count =
                players.Count(item => item.Value.TeamNo == TeamNo.First) +
                bots.Count(item => item.Value.TeamNo == TeamNo.First);

            var team2Count =
                players.Count(item => item.Value.TeamNo == TeamNo.Second) +
                bots.Count(item => item.Value.TeamNo == TeamNo.Second);

            return team1Count > team2Count ? TeamNo.Second : TeamNo.First;
        }
    }
}
