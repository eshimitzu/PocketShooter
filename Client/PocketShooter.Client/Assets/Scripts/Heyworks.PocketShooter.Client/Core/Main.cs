using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Communication;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.LocalSettings;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.SocialConnections.Core;
using Heyworks.PocketShooter.UI;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.UI.Notifications;
using Heyworks.PocketShooter.UI.Popups;
using Microsoft.Extensions.Logging;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;
using Notification = Heyworks.PocketShooter.UI.Notifications.Notification;

namespace Heyworks.PocketShooter.Core
{
    public class Main : MonoBehaviour, IGameHubObserver
    {
        private const int LoadingScreenMinTimeMs = 2000;

        [Inject]
        private RealtimeRunBehavior runBehavior;

        [Inject]
        private IAppConfiguration appConfiguration;

        [Inject]
        private IConfigurationStorage configurationStorage;

        [Inject]
        private IConfigurationsProvider configurationsProvider;

        [Inject]
        private ScreensController screensController;

        [Inject]
        private IGameHubClient gameHubClient;

        [Inject]
        private IGameHubObserver gameHubObserver;

        [Inject]
        private IWebApiClient webApiClient;

        [Inject]
        private MapSceneManager mapSceneManager;

        private ConnectionState connectionState = ConnectionState.Disconnected;

        private UniTaskCompletionSource<MatchMakingResultData> matchMaked;
        private UniTaskCompletionSource<ClientGameState> gameStateReceived;

        internal ServerAddress CurrentMetaServerAddress { get; private set; }

        public MetaGame MetaGame { get; private set; }

        public NotificationController NotificationController { get; private set; }

        public ConnectionState ConnectionState
        {
            get => connectionState;
            private set
            {
                connectionState = value;
                ConnectionStateChanged?.Invoke(connectionState);
            }
        }

        public MatchType DefaultMatchType { get; set; } = MatchType.Domination;

        public MapNames DefaultMatchMap { get; set; } = MapNames.Mexico;

        // TODO: into observable
        public event Action<ConnectionState> ConnectionStateChanged;

        public event Action<MetaGame> GameStateReset;

        private static IEnumerable<UniTask> InitializeSocialNetworks => new[]
        {
            SocialNetworkManager.GooglePlay.Initialize(),
            // SocialNetworkManager.GameCenter.Initialize(),
        };

        private async void Start()
        {
            float startTimeSec = Time.time;
            screensController.ShowScreen<LoadingScreen>();

            GameSettings.Apply();
            MainThreadDispatcher.Initialize();
            AddEventHandlers();

            await UniTask.WhenAll(InitializeSocialNetworks);
            ClientGameState gameState = await ConnectToMetaServer(appConfiguration.MetaServerAddress);
            await InitializeGame(gameState);
            await WaitForMinimumLoadingTime(startTimeSec);

            var screen = screensController.ShowScreen<LobbyScreen>();
            screen.Setup(this);
        }

        private async void OnDestroy()
        {
            await RemoveEventHandlers();
        }

        private void AddEventHandlers()
        {
            gameHubClient.Subscribe(this);
            runBehavior.Connected += RunBehaviour_Connected;
            runBehavior.Disconnected += RunBehaviour_Disconnected;
        }

        private async UniTask RemoveEventHandlers()
        {
            runBehavior.Connected -= RunBehaviour_Connected;
            runBehavior.Disconnected -= RunBehaviour_Disconnected;

            await gameHubClient.DisconnectAsync();
            NetLog.Log.LogInformation("Stop game hub connection");
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public async UniTask<ClientGameState> ConnectToMetaServer(ServerAddress serverAddress)
        {
            CurrentMetaServerAddress = serverAddress;
            ConnectionState = ConnectionState.Connecting;

            NetLog.Log.LogInformation("Connecting to game hub...");

            gameStateReceived = new UniTaskCompletionSource<ClientGameState>();
            await gameHubClient.ConnectAsync();

            ConnectionState = ConnectionState.Connected;
            NetLog.Log.LogInformation("Connected successfully");

            ClientGameState gameState = await gameStateReceived.Task;

            return gameState;
        }

        public Task ReceiveGameState(ClientGameState gameState)
        {
            NetLog.Log.LogInformation("Game state received {name}, {group}", gameState.Player.Nickname, gameState.Player.Group);

            if (MetaGame == null)
            {
                gameStateReceived.TrySetResult(gameState);
            }
            else
            {
                MetaGame.UpdateState(gameState);
                GameStateReset?.Invoke(MetaGame);
            }

            return Task.CompletedTask;
        }

        private async UniTask InitializeGame(ClientGameState gameState)
        {
            var configResponse = await gameHubClient.SynchronizeConfigAsync(new Version("0.0.0"));

            if (configResponse.IsOk)
            {
                var config = configResponse.Ok.Data;
                if (config != null)
                {
                    NetLog.Log.LogInformation($"Sync config succeeded. New game config version {config.Version}");
                    configurationStorage.SetGameConfig(config);
                }
                else
                {
                    NetLog.Log.LogInformation($"Sync config succeeded. Game config is up to date.");
                }
            }
            else
            {
                NetLog.Log.LogWarning($"Get config error. Reason: {configResponse.Error.Message}");
            }

            MetaGame = new MetaGame(gameState, configurationsProvider, gameHubClient);
            NotificationController = InitializeNotificationController(MetaGame);

            NetLog.Log.LogInformation("Game initialization finished.");
        }

        public async UniTask StartMatchMaking()
        {
            ConnectionState = ConnectionState.MatchMaking;

            var screen = screensController.ShowScreen<MatchmakingScreen>();
            screen.Setup();

            AnalyticsManager.Instance.StartMatchmaking();

            try
            {
                NetLog.Log.LogInformation("Starting matchmaking of {matchType}...", DefaultMatchType);

                matchMaked = new UniTaskCompletionSource<MatchMakingResultData>();

                if (DefaultMatchType != MatchType.Domination)
                {
                    await gameHubClient.MakeMatchAsync(new MatchRequest(DefaultMatchType, DefaultMatchMap));
                }
                else
                {
                    await gameHubClient.MakeMatchAsync(new MatchRequest(DefaultMatchType));
                }

                var matchServer = await matchMaked.Task;
                NetLog.Log.LogInformation($"Match founded on server {matchServer.ServerAddress}");

                JoinRoom(new GameSession(matchServer.ServerAddress, new PlayerId(MetaGame.Player.Id)));
            }
            catch (Exception e)
            {
                AnalyticsManager.Instance.EndMatchmaking(DefaultMatchType, AnalyticsManager.MatchmakingResult.Disconnected);
                NetLog.Log.LogError(e, "Matchmaking failed.");
            }
            finally
            {
                ConnectionState = ConnectionState.Connected;
            }
        }

        // TODO: a.dezhurko Move to main flow and remove from subscriber.
        public Task MatchMaked(MatchMakingResultData matchMakingResult)
        {
            mapSceneManager.LoadMapScene(matchMakingResult.Map);

            matchMaked.TrySetResult(matchMakingResult);

            return Task.CompletedTask;
        }

        public void JoinRoom(GameSession gameSession)
        {
            ConnectionState = ConnectionState.JoiningRoom;
            runBehavior.Connect(new ServerAddress(gameSession.Server.Address.ToString(), gameSession.Server.Port), gameSession.Player);
        }

        private async UniTask WaitForMinimumLoadingTime(float startTimeSec)
        {
            float timeSinceStartSec = Time.time - startTimeSec;
            int delayMs = Math.Max(0, LoadingScreenMinTimeMs - (int)(timeSinceStartSec * 1000));
            await UniTask.Delay(delayMs);
        }

        private NotificationController InitializeNotificationController(MetaGame game)
        {
            var levelUpNotification = new AccountLevelUpNotification(game.Player, game.СonfigProvider.PlayerConfiguration);
            var contentUpgradeNotification = new ContentUpgradeNotification(game.Army);
            var controller = new NotificationController(new Notification[] { levelUpNotification, contentUpgradeNotification });

            return controller;
        }

        private void RunBehaviour_Disconnected(DisconnectReason reason)
        {
            ConnectionState = ConnectionState.Connected;

            // NOTE: if disconnected during battle, then clear resources here. Same in EndBattleScreen, maybe redesign.
            void UnloadMapAndGoToMenu()
            {
                mapSceneManager.UnloadCurrentMapScene();
                var screen = screensController.ShowScreen<LobbyScreen>();
                screen.Setup(this, true);
            }

            switch (reason)
            {
                case DisconnectReason.GameEnd:
                    NetLog.Log.LogInformation("Disconnected from server after game end.");
                    break;
                case DisconnectReason.ManualDisconnect:
                    NetLog.Log.LogInformation("Disconnected from server by user request. Exiting to lobby.");
                    UnloadMapAndGoToMenu();
                    break;
                case DisconnectReason.ConnectionTimeout:
                case DisconnectReason.JoinRoomTimeout:
                case DisconnectReason.RoomIsFull:
                case DisconnectReason.ServerConnectionError:
                case DisconnectReason.InGameDisconnect:
                    NetLog.Log.LogWarning("Disconnected from server during gameplay. Reason: {reason}. Exiting to lobby.", reason);
                    UnloadMapAndGoToMenu();
                    screensController.ShowMessageBox(
                        LocKeys.BattleDisconnectTitle.Localized(),
                        LocKeys.BattleDisconnectDescription.Localized(),
                        new PopupOptionInfo(LocKeys.BattleDisconnectButton.Localized()));
                    break;
                default:
                    throw new ArgumentException($"Disconnect reason {reason} not supported");
            }

            switch (reason)
            {
                case DisconnectReason.ConnectionTimeout:
                case DisconnectReason.JoinRoomTimeout:
                case DisconnectReason.RoomIsFull:
                case DisconnectReason.ServerConnectionError:
                    AnalyticsManager.Instance.EndMatchmaking(DefaultMatchType, AnalyticsManager.MatchmakingResult.Disconnected);
                    break;
            }

            AudioController.Instance.PostEvent(AudioKeys.Event.ResumeLobbyMusic);
        }

        private void RunBehaviour_Connected()
        {
            AnalyticsManager.Instance.EndMatchmaking(DefaultMatchType, AnalyticsManager.MatchmakingResult.Matched);
        }

        // NOTE: used at least in editor mode, to close connection gracefully without exception and background infinite disconnect
        // maybe handle onApplicationPause true\false for devices.
        private async void OnApplicationQuit()
        {
            await gameHubClient.DisconnectAsync();
        }
    }
}