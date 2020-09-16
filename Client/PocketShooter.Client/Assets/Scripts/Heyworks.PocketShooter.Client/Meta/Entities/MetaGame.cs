using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Runnables;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Purchasing.Core;
using Heyworks.PocketShooter.Purchasing.ProductManagers;
using Heyworks.PocketShooter.UI.Notifications;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class MetaGame
    {
        private readonly Scheduler scheduler;

        public MetaGame(ClientGameState gameState, IConfigurationsProvider configProvider, IGameHubClient gameHubClient)
        {
            GameHubClient = gameHubClient;
            this.СonfigProvider = configProvider;

            var timeProvider = new ServerTimeProvider(gameState.ServerTimeUtc);

            Player = new Player(gameState.Player, timeProvider, configProvider.PlayerConfiguration);

            Army = new Army(
                gameState.Army,
                timeProvider,
                gameHubClient,
                configProvider.ArmyConfiguration,
                configProvider.TrooperConfiguration,
                configProvider.WeaponConfiguration,
                configProvider.HelmetConfiguration,
                configProvider.ArmorConfiguration);

            Shop = CreateShop(configProvider);
            Shop.InitializeInAppPurchases();
            Shop.ConfirmPendingInAppPurchases();

            Roster = new Roster(
                Shop,
                Army,
                configProvider.TrooperConfiguration,
                configProvider.WeaponConfiguration,
                configProvider.HelmetConfiguration,
                configProvider.ArmorConfiguration);

            scheduler = new Scheduler(Metronome.Instance);
            scheduler.Register(Army);
        }

        public IConfigurationsProvider СonfigProvider;

        public IGameHubClient GameHubClient { get; }

        public Player Player { get; }

        public Army Army { get; }

        public Roster Roster { get; }

        public Shop Shop { get; }

        public event Action GameStateChanged;

        public void UpdateState(ClientGameState gameState)
        {
            Player.UpdateState(gameState.Player);
            Army.UpdateState(gameState.Army);

            OnGameChanged();
        }

        public void ApplyContent(IEnumerable<IContentIdentity> content)
        {
            foreach (var contentIdentity in content)
            {
                switch (contentIdentity)
                {
                    case TrooperIdentity trooper:
                        Army.AddTrooper(trooper);
                        break;
                    case WeaponIdentity weapon:
                        Army.AddWeapon(weapon);
                        break;
                    case HelmetIdentity helmet:
                        Army.AddHelmet(helmet);
                        break;
                    case ArmorIdentity armor:
                        Army.AddArmor(armor);
                        break;
                    case ResourceIdentity resource:
                        Player.AddResource(resource);
                        break;
                    case OffensiveIdentity offensive:
                        Army.AddOffensive(offensive);
                        break;
                    case SupportIdentity support:
                        Army.AddSupport(support);
                        break;
                    default:
                        throw new NotImplementedException($"The type of content identity {contentIdentity.GetType().Name} is not supported");
                }
            }

            OnGameChanged();
        }

        public void ApplyPlayerReward(PlayerReward reward)
        {
            var levelBefore = Player.Level;
            Player.ApplyReward(reward);
            AnalyticsManager.Instance.SendCurrencyIncomingForBattle(reward);
            var levelAfter = Player.Level;

            if (levelAfter > levelBefore)
            {
                AnalyticsManager.Instance.SendLevelUp(levelAfter);
                var content = СonfigProvider.PlayerConfiguration.GetLevelUpReward(levelBefore, levelAfter);
                ApplyContent(content);
                AnalyticsManager.Instance.SendCurrencyIncomingForLevelUp(content);
            }
        }

        private Shop CreateShop(IConfigurationsProvider configProvider)
        {
            var inAppPurchasesFactory = new InAppPurchaseProductsManager(GameHubClient, configProvider.ShopConfiguration, this);
            var managers = new List<ProductsManager>()
            {
                new InGameProductsManager(GameHubClient, configProvider.ShopConfiguration, this),
            };

            return new Shop(inAppPurchasesFactory, managers);
        }

        private void OnGameChanged()
        {
            GameStateChanged?.Invoke();
        }
    }
}