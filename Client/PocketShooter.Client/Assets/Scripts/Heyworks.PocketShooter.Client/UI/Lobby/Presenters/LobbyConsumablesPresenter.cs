using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.UI.Popups;
using Heyworks.PocketShooter.Utils;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyConsumablesPresenter : IDisposablePresenter
    {
        private MetaGame metaGame;
        private LobbyConsumablesView view;
        private ScreensController screensController;
        private SimpleScheduler scheduler;

        public LobbyConsumablesPresenter(ScreensController screensController, MetaGame metaGame, LobbyConsumablesView view)
        {
            this.metaGame = metaGame;
            this.view = view;
            this.screensController = screensController;

            metaGame.GameStateChanged += Lobby_GameStateChanged;
            view.GrenadeItem.OnButtonClick += GrenadeItemOnButtonClick;
            view.MedkitItem.OnButtonClick += MedkitItemOnButtonClick;
            view.ChestItem.OnButtonClick += ChestItemOnButtonClick;

            scheduler = new SimpleScheduler(UpdateTimer, 1);

            UpdateView();
        }

        public void Dispose()
        {
            metaGame.GameStateChanged -= Lobby_GameStateChanged;
        }

        public void Update()
        {
            scheduler.TryExecute();
        }

        private void UpdateView()
        {
            view.GrenadeItem.Label.text = metaGame.Army.GetSelectedOffensivesCount().ToString();
            view.MedkitItem.Label.text = metaGame.Army.GetSelectedSupportsCount().ToString();

            UpdateTimer();
        }

        private void UpdateTimer()
        {
            if (metaGame.Player.RewardProvider.CanGetReward())
            {
                view.ChestItem.Label.text = LocKeys.ClaimReward.Localized();
            }
            else
            {
                TimeSpan remainingTime = metaGame.Player.RewardProvider.RemainingTime;
                view.ChestItem.Label.text = remainingTime.ToShortCoupleTimeComponentString();
            }
        }

        private async void ChestItemOnButtonClick()
        {
            if (metaGame.Player.RewardProvider.CanGetReward())
            {
                await metaGame.GameHubClient.CollectRepeatingReward();

                var reward = metaGame.Player.RewardProvider.GetReward();
                var content = new List<IContentIdentity>(reward);
                metaGame.ApplyContent(content);

                var popup = ScreensController.Instance.ShowPopup<AcquireConfirmationPopup>();
                popup.Setup(content);
            }
        }

        private void MedkitItemOnButtonClick()
        {
            screensController.ShowStoreWhenReadyAsync(metaGame, ShopCategory.Consumables);
        }

        private void GrenadeItemOnButtonClick()
        {
            screensController.ShowStoreWhenReadyAsync(metaGame, ShopCategory.Consumables);
        }

        private void Lobby_GameStateChanged()
        {
            UpdateView();
        }
    }
}