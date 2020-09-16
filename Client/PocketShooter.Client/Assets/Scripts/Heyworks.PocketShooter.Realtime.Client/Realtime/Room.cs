using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Connection;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Serialization;
using Heyworks.PocketShooter.Realtime.Service;
using Heyworks.PocketShooter.Realtime.Simulation;
using Microsoft.Extensions.Logging;
using UniRx;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents room for starting realtime game.
    /// </summary>
    public sealed class Room
    {
        private readonly Subject<Game> gameStarted;
        private readonly Subject<Game> gameEnded;
        private readonly Subject<BotSimulationData> botSpawned;
        private readonly Subject<BotSimulationData> botRespawned;

        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        private INetworkService networkService;
        private Ticker clientTicker;
        private DominationModeInfo dominationModeInfo;
        private IInterpolatingStateProvider<SimulationState> simulationStateProvider;

        /// <summary>
        /// Gets the game started observer.
        /// </summary>
        public IObservable<Game> GameStarted => gameStarted;

        /// <summary>
        /// Gets the game ended observer.
        /// </summary>
        public IObservable<Game> GameEnded => gameEnded;

        /// <summary>
        /// Gets the bot spawned first time observer.
        /// </summary>
        public IObservable<BotSimulationData> BotSpawned => botSpawned;

        /// <summary>
        /// Gets the bot respawned (not the first time) observer.
        /// </summary>
        public IObservable<BotSimulationData> BotRespawned => botRespawned;

        /// <summary>
        /// Gets the player info.
        /// </summary>
        public PlayerInfo PlayerInfo { get; }

        /// <summary>
        /// Gets the room id.
        /// </summary>
        public RoomId Id { get; }

        /// <summary>
        /// Gets the local trooper identifier.
        /// </summary>
        public EntityId LocalTrooperId { get; }

        /// <summary>
        /// Gets the player's team.
        /// </summary>
        public TeamNo Team { get; }

        /// <summary>
        /// Gets the network connection.
        /// </summary>
        public IGameplayConnection Connection { get; }

        /// <summary>
        /// Gets the network service.
        /// </summary>
        /// <value>The network service.</value>
        public INetworkService NetworkService => networkService;

        /// <summary>
        /// Gets the game time.
        /// </summary>
        public int GameTime => IsGameStarted
            ? NetworkSimulation.TickProvider.Tick
            : 0;

        /// <summary>
        /// Gets the game.
        /// </summary>
        public Game Game { get; private set; }

        /// <summary>
        /// Gets the network simulation controller.
        /// </summary>
        public NetworkSimulation NetworkSimulation { get; private set; }

        /// <summary>
        /// Gets the client simulation.
        /// </summary>
        public ClientSimulation LocalPlayerSimulation { get; private set; }

        /// <summary>
        /// Gets a value indicating whether game was started.
        /// </summary>
        public bool IsGameStarted => Game != null;

        /// <summary>
        /// Gets the realtime configuration.
        /// </summary>
        public IRealtimeConfiguration RealtimeConfiguration { get; }

        /// <summary>
        /// Gets a value indicating whether the manual disconnect was requested.
        /// </summary>
        public bool ManualDisconnectRequested { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="id">The room id.</param>
        /// <param name="metaGame">The meta game.</param>
        /// <param name="localTrooperId">The local trooper identifier.</param>
        /// <param name="team">The team.</param>
        /// <param name="playerInfo">Player info.</param>
        /// <param name="dominationModeInfo">Domination mode info.</param>
        public Room(
            IGameplayConnection connection,
            IRealtimeConfiguration configuration,
            RoomId id,
            EntityId localTrooperId,
            TeamNo team,
            PlayerInfo playerInfo,
            DominationModeInfo dominationModeInfo)
        {
            Connection = connection.NotNull();
            RealtimeConfiguration = configuration.NotNull();
            Id = id;
            LocalTrooperId = localTrooperId;
            Team = team;
            PlayerInfo = playerInfo.NotNull();
            this.dominationModeInfo = dominationModeInfo.NotNull();

            gameStarted = new Subject<Game>();
            gameEnded = new Subject<Game>();
            botSpawned = new Subject<BotSimulationData>();
            botRespawned = new Subject<BotSimulationData>();

            networkService = new NetworkService(Connection, new Dictionary<NetworkDataCode, IDataHandler>(), configuration);
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        /// <param name="tickToStartFrom">The starting tick for the simulation.</param>
        /// <param name="timeStamp">The time stamp when server sent the start command.</param>
        public void StartGame(int tickToStartFrom, int timeStamp)
        {
            simulationStateProvider = new InterpolatingStateProvider(RealtimeConfiguration);
            clientTicker = new Ticker(
                new AdjustableStopwatch(RealtimeConfiguration.MaximumTickerSkewInMilliseconds),
                RealtimeConfiguration.TickIntervalMs);
            var worldTicker = new Ticker(
                new AdjustableStopwatch(RealtimeConfiguration.MaximumTickerSkewInMilliseconds),
                RealtimeConfiguration.TickIntervalMs);
            var timeManager = new TimeManager(clientTicker, worldTicker, Connection, simulationStateProvider, RealtimeConfiguration);

            var serializer = new SimulationStateSerializer(RealtimeConfiguration.StatesBufferSize);
            var simulationStateHandler = new SimulationStateDataHandler(simulationStateProvider, serializer, RealtimeConfiguration, timeManager);
            var botControlTakenDataHandler = new BotControlTakenDataHandler();
            var gameEndedDataHandler = new GameEndedDataHandler();
            var serverErrorDataHandler = new ServerErrorDataHandler();

            gameEndedDataHandler.GameEnded += GameEndedDataHandler_GameEnded;

            // TODO: a.dezhurko Move all about bots to bot manager.
            botControlTakenDataHandler.BotControlTaken += BotControlTakenDataHandler_BotControlTaken;

            var handlers = new Dictionary<NetworkDataCode, IDataHandler>
            {
                { NetworkDataCode.SimulationState, simulationStateHandler },
                { NetworkDataCode.ServerError, serverErrorDataHandler },
                { NetworkDataCode.BotControlTaken, botControlTakenDataHandler },
                { NetworkDataCode.GameEnded, gameEndedDataHandler },
            };

            var networkServiceImpl = new NetworkService(Connection, handlers, RealtimeConfiguration);
            networkService = networkServiceImpl;

            SimulationLog.Information("Creating simulation for {PlayerId}", LocalTrooperId);

            NetworkSimulation = new NetworkSimulation(
                networkServiceImpl,
                simulationStateProvider,
                RealtimeConfiguration,
                timeManager);

            var state = new SimulationState(tickToStartFrom, 0, GameState.Create(dominationModeInfo));
            NetworkSimulation.Start(tickToStartFrom, timeStamp, state);

            var localPlayerStateProvider = new StateProvider<PlayerState>(RealtimeConfiguration.StatesBufferSize);
            Game = new Game(
                new SimulationRef(simulationStateProvider, tickToStartFrom),
                LocalTrooperId,
                dominationModeInfo,
                PlayerInfo,
                clientTicker);

            LocalPlayerSimulation = new ClientSimulation(
                networkService,
                localPlayerStateProvider,
                Game,
                clientTicker);

            // ISSUE: we creating player with 0 id here, how knows wher it would lead, should NOT create IMPOSSIBLE states, use id!
            // TODO: v.shimkovich Its may be better and SAFER to create simulation after player creation
            LocalPlayerSimulation.Start(tickToStartFrom, default);

            NetworkSimulation.AddSimulation(LocalPlayerSimulation);

            gameStarted.OnNext(Game);
        }

        /// <summary>
        /// Simulates the game and receives and sends network data.
        /// </summary>
        public void Update() => NetworkSimulation.Update();

        public void ManualDisconnect()
        {
            ManualDisconnectRequested = true;
            Connection.Disconnect();
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Room"/> object.
        /// </summary>
        public void Dispose()
        {
            subscriptions.ForEach(_ => _.Dispose());
            subscriptions.Clear();
        }

        private void GameEndedDataHandler_GameEnded(GameEndedData gameEndedData)
        {
            gameEnded.OnNext(Game);

            networkService = new NetworkService(Connection, new Dictionary<NetworkDataCode, IDataHandler>(), RealtimeConfiguration);
            Connection.Disconnect();
        }

        private void BotControlTakenDataHandler_BotControlTaken(BotControlTakenData botControlTakenData)
        {
            var startTick = NetworkSimulation.TickProvider.WorldTick;

            var context = new Game(
                new SimulationRef(simulationStateProvider, startTick),
                botControlTakenData.TrooperId,
                dominationModeInfo,
                botControlTakenData.BotInfo,
                clientTicker);

            var botSimulation = new ClientSimulation(
                networkService,
                new StateProvider<PlayerState>(RealtimeConfiguration.StatesBufferSize),
                context,
                clientTicker);

            // ISSUE: we creating player with 0 id here, how knows wher it would lead, should NOT create IMPOSSIBLE states, use id!
            botSimulation.Start(startTick, default);

            NetworkSimulation.AddSimulation(botSimulation);

            var botSimulationData = new BotSimulationData(botControlTakenData.BotInfo, context, botSimulation, NetworkSimulation);

            context.LocalPlayerSpawned
                .Subscribe(x => botSpawned.OnNext(botSimulationData))
                .AddTo(subscriptions);

            context.LocalPlayerRespawned
                .Subscribe(x => botRespawned.OnNext(botSimulationData))
                .AddTo(subscriptions);

            // TODO: hack. Must be removed after proper bot integration.
            TrooperClass[] availableClasses = botControlTakenData.BotInfo.Troopers.Select(item => item.Class).ToArray();
            int randomClassIndex = new System.Random().Next(0, availableClasses.Length - 1);
            var trooper = availableClasses[randomClassIndex];

            botSimulation.AddCommand(new SpawnBotTrooperCommandData(botControlTakenData.BotInfo.Id, trooper));
        }
    }
}