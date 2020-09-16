using System;
using Heyworks.PocketShooter.Authorization;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Client.Configuration;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Serialization;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.SocialConnections.Core;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Weapons;
using Microsoft.Extensions.Logging;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Heyworks.PocketShooter.Core
{
    /// <summary>
    /// Represents main zenject installer.
    /// </summary>
    public class ZenjectInstaller : MonoInstaller
    {
        // TODO y.liavonchyk wait for other code
        [SerializeField]
        private GameObject trooperCreatorPrefab = null;

        [SerializeField]
        private GameObject realtimeRunBehaviorPrefab = null;

        [SerializeField]
        private GameObject lobbyPrefab;

        [SerializeField]
        private GameObject screensController = null;

        [SerializeField]
        private GameObject progressHud = null;

        [SerializeField]
        private GameObject analyticsManager = null;

        [SerializeField]
        private GameObject mapSceneManager;

        [SerializeField]
        private GameVisualSettings gameVisualSettings;

        /// <summary>
        /// Installs the bindings.
        /// </summary>
        public override void InstallBindings()
        {
            var options = new LoggerFilterOptions();

            var providers = new ILoggerProvider[]
            {
                new ConsoleLoggerProvider(),
                new FileLoggerProvider(new FileLoggerSettings("PocketShooter.log")),
            };

            var configuration = new LoggerConfiguration();

            var loggerFactory = new LoggerFactory(providers, new LoggerFilterOptionsMonitor(options, configuration));
            Container.Bind<ILoggerFactory>().FromInstance(loggerFactory);

            // may need hook Debug.logger.logHandler
            MLog.Setup(loggerFactory, options, configuration);

            var configProvider = new ConfigurationProvider();
            IAppConfiguration appConfiguration = configProvider.AppConfiguration;
            var realtimeConfig = new RealtimeConfiguration();

            Container.BindInstance<IRealtimeConfiguration>(realtimeConfig);
            Container.BindInstance(appConfiguration);

            var configurationStorage = new ConfigurationStorage();
            Container.BindInstance<IConfigurationStorage>(configurationStorage).AsSingle();
            Container.Bind<IConfigurationsProvider>().To<ConfigurationsProvider>().AsSingle();

            var dataSerializer = new JsonSerializer(new DefaultSerializerSettings());
            var disconnectListener = new UIConnectionListener();

            var webApiClient = new WebApiClient(
                appConfiguration.MetaServerAddress.Address,
                appConfiguration.Version,
                dataSerializer,
                loggerFactory,
                disconnectListener);

            var authorizationController = new AuthorizationController(webApiClient, appConfiguration);
            var accessTokenProvider = new UIAccessTokenProvider(authorizationController);

            var authorizedWebApiClient = new AuthorizedWebApiClient(
                appConfiguration.MetaServerAddress.Address,
                appConfiguration.Version,
                accessTokenProvider,
                dataSerializer,
                loggerFactory,
                disconnectListener);

            var gameHubClient = new GameHubClient(
                appConfiguration.MetaServerAddress.Address,
                appConfiguration.Version,
                accessTokenProvider,
                UnitySynchronizationContext.Current,
                loggerFactory,
                disconnectListener);

            var googleSocialConnection = new SocialConnectionController(authorizedWebApiClient);

            Container.BindInstance<IWebApiClient>(webApiClient).AsSingle();
            Container.BindInstance<IAuthorizedWebApiClient>(authorizedWebApiClient).AsSingle();
            Container.BindInstance<IGameHubClient>(gameHubClient).AsSingle();
            Container.BindInstance(googleSocialConnection).AsSingle();

            Container.BindFactory<Object, WeaponView, WeaponView.Factory>()
                .FromFactory<PrefabFactory<WeaponView>>();
            Container.BindFactory<Object, BaseScreen, BaseScreen.Factory>()
                .FromFactory<PrefabFactory<BaseScreen>>();

            Container.BindFactory<Object, NetworkCharacter, NetworkCharacter.Factory>()
                .FromFactory<PrefabFactory<NetworkCharacter>>();
            Container.BindFactory<Object, DummyCharacter, DummyCharacter.Factory>()
                .FromFactory<PrefabFactory<DummyCharacter>>();
            Container.BindFactory<Object, BotCharacter, BotCharacter.Factory>()
                .FromFactory<PrefabFactory<BotCharacter>>();

            Container.Bind<TrooperCreator>()
                .FromComponentInNewPrefab(trooperCreatorPrefab)
                .UnderTransformGroup("Managers")
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<RealtimeRunBehavior>()
                .FromComponentInNewPrefab(realtimeRunBehaviorPrefab)
                .UnderTransformGroup("Managers")
                .AsSingle()
                .NonLazy();

            Container.Bind<AnalyticsManager>()
                     .FromComponentInNewPrefab(analyticsManager)
                     .UnderTransformGroup("Managers")
                     .AsSingle()
                     .NonLazy();

            Container.Bind<MapSceneManager>()
                     .FromComponentInNewPrefab(mapSceneManager)
                     .UnderTransformGroup("Managers")
                     .AsSingle()
                     .NonLazy();

            Container.Bind<ScreensController>()
                .FromComponentInNewPrefab(screensController)
                .UnderTransformGroup("UICanvas")
                .AsSingle()
                .NonLazy();

            Container.Bind<ProgressHUD>()
                .FromComponentInNewPrefab(progressHud)
                .UnderTransformGroup("UICanvas")
                .AsSingle()
                .NonLazy();

            Container.Bind<GameVisualSettings>().FromInstance(gameVisualSettings);

            // Temporary lobby controller
            Container.BindInterfacesAndSelfTo<Main>()
                .FromComponentInNewPrefab(lobbyPrefab)
                .UnderTransformGroup("Managers")
                .AsSingle()
                .NonLazy();
        }
    }
}