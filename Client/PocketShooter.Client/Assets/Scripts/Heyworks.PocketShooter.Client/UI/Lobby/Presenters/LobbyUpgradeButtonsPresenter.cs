using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Lobby;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.UI.Popups;
using UniRx;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyUpgradeButtonsPresenter : IDisposablePresenter
    {
        private readonly LobbyButtonsView lobbyButtonsView;
        private readonly LobbySelectionPresenter selectionPresenter;
        private readonly SideItemProgressView sideItemProgressView;
        private readonly MetaGame game;
        private readonly IGameHubClient gameHubClient;
        private ILobbyRosterProvider rosterProvider;
        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        private ILobbyRosterItem SelectedItem => selectionPresenter.SelectedItem;

        private bool IsProductSelected => SelectedItem.Product != null;

        private ILobbyRosterPresenter ActiveRoster => rosterProvider.CurrentRoster;

        public LobbyUpgradeButtonsPresenter(
            LobbyButtonsView lobbyButtonsView,
            LobbySelectionPresenter selectionPresenter,
            SideItemProgressView sideItemProgressView,
            ILobbyRosterProvider rosterProvider,
            MetaGame game,
            IGameHubClient gameHubClient)
        {
            this.lobbyButtonsView = lobbyButtonsView;
            this.selectionPresenter = selectionPresenter;
            this.sideItemProgressView = sideItemProgressView;
            this.rosterProvider = rosterProvider;
            this.game = game;
            this.gameHubClient = gameHubClient;

            selectionPresenter.OnSelectionChanged += SelectionChanged;

            lobbyButtonsView.PurchaseButton.OnClick.AddListener(BuyButton_OnClick);
            lobbyButtonsView.EquipButton.onClick.AddListener(EquipButton_OnClick);
            lobbyButtonsView.LevelUpInstantButton.OnClick.AddListener(LevelUpInstantButton_OnClick);
            lobbyButtonsView.LevelUpRegularButton.OnClick.AddListener(LevelUpRegularButton_OnClick);
            lobbyButtonsView.GradeUpInstantButton.OnClick.AddListener(GradeUpInstantButton_OnClick);
            lobbyButtonsView.CompleteProgressButtonController.OnClick.AddListener(CompleteProgressButton_OnClick);
            sideItemProgressView.CompleteProgressButtonController.OnClick.AddListener(CompleteProgressButton_OnClick);

            game.Army.ItemProgressCompleted.Subscribe(OnItemProgressComplete).AddTo(subscriptions);
        }

        public void Dispose()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            selectionPresenter.OnSelectionChanged -= SelectionChanged;

            lobbyButtonsView.PurchaseButton.OnClick.RemoveListener(BuyButton_OnClick);
            lobbyButtonsView.EquipButton.onClick.RemoveListener(EquipButton_OnClick);
            lobbyButtonsView.LevelUpInstantButton.OnClick.RemoveListener(LevelUpInstantButton_OnClick);
            lobbyButtonsView.LevelUpRegularButton.OnClick.RemoveListener(LevelUpRegularButton_OnClick);
            lobbyButtonsView.GradeUpInstantButton.OnClick.RemoveListener(GradeUpInstantButton_OnClick);
            lobbyButtonsView.CompleteProgressButtonController.OnClick.RemoveListener(CompleteProgressButton_OnClick);
            sideItemProgressView.CompleteProgressButtonController.OnClick.RemoveListener(CompleteProgressButton_OnClick);
        }

        private void SelectionChanged()
        {
            UpdateBuyButton();
            UpdateEquipButton();
            UpdateItemInProgressViews();
            UpdatePowerUpButtons();
        }

        private void RefreshView()
        {
            // TODO
            ActiveRoster.Refresh();
        }

        private void UpdateItemInProgressViews()
        {
            if (!game.Army.HasItemProgress)
            {
                lobbyButtonsView.CompleteProgressButtonController.HideButton();
                lobbyButtonsView.ProgressTimerView.gameObject.SetActive(false);
                sideItemProgressView.gameObject.SetActive(false);

                return;
            }

            var itemInProgress = game.Army.GetItemProgress();
            TimerView currentTimerView = null;
            AutoRefreshPurchaseButtonController currentCompleteButtonController = null;
            if (itemInProgress.ItemId == SelectedItem.Id)
            {
                lobbyButtonsView.CompleteProgressButtonController.ShowButton();
                lobbyButtonsView.ProgressTimerView.gameObject.SetActive(true);
                sideItemProgressView.gameObject.SetActive(false);

                currentTimerView = lobbyButtonsView.ProgressTimerView;
                currentCompleteButtonController = lobbyButtonsView.CompleteProgressButtonController;
            }
            else
            {
                lobbyButtonsView.CompleteProgressButtonController.HideButton();
                lobbyButtonsView.ProgressTimerView.gameObject.SetActive(false);
                sideItemProgressView.gameObject.SetActive(true);

                currentTimerView = sideItemProgressView.ProgressTimerView;
                currentCompleteButtonController = sideItemProgressView.CompleteProgressButtonController;
            }

            // NOTE: for now only level up can be in progress. Change caption according to item in progress state in future.
            currentTimerView.Setup(itemInProgress, LocKeys.ItemInfo.LevelUpInProgress);

            currentCompleteButtonController.Setup(
                () => itemInProgress.CompletePrice,
                LocKeys.ItemInfo.InstantComplete,
                game.Player.CanPayPrice(itemInProgress.CompletePrice));
        }

        private void UpdateBuyButton()
        {
            if (IsProductSelected)
            {
                var realPrice = SelectedItem.Product is InAppPurchaseRosterProduct inapp
                    ? inapp.FormattedPrice
                    : string.Empty;

                lobbyButtonsView.PurchaseButton.gameObject.SetActive(!SelectedItem.Product.IsLocked);
                lobbyButtonsView.PurchaseButton.Setup(
                    SelectedItem.Product.Price,
                    LocKeys.ItemInfo.Buy,
                    game.Player.CanPayPrice(SelectedItem.Product.Price),
                    realPrice);
            }
            else
            {
                lobbyButtonsView.PurchaseButton.gameObject.SetActive(false);
            }
        }

        private void UpdateEquipButton()
        {
            if (IsProductSelected || SelectedItem is ITrooperItem)
            {
                lobbyButtonsView.EquipButton.gameObject.SetActive(false);
                return;
            }

            lobbyButtonsView.EquipButton.gameObject.SetActive(true);

            bool isCurrentItemEquipped = false;

            if (SelectedItem is IHelmetItem helmetItem)
            {
                isCurrentItemEquipped = !game.Army.CanEquipHelmet(ActiveRoster.CurrentTrooperClass, helmetItem.Name);
            }
            else if (SelectedItem is IArmorItem armorItem)
            {
                isCurrentItemEquipped = !game.Army.CanEquipArmor(ActiveRoster.CurrentTrooperClass, armorItem.Name);
            }
            else if (SelectedItem is IWeaponItem weaponItem)
            {
                isCurrentItemEquipped = !game.Army.CanEquipWeapon(ActiveRoster.CurrentTrooperClass, weaponItem.Name);
            }

            lobbyButtonsView.EquipButtonLabel.SetLocalizedText(isCurrentItemEquipped
                ? LocKeys.ItemInfo.Equipped
                : LocKeys.ItemInfo.Equip);
            lobbyButtonsView.EquipButtonBackground.Gradient =
                lobbyButtonsView.ColorsPreset.GetEquipButtonColor(isCurrentItemEquipped);
            lobbyButtonsView.EquipButtonShadow.enabled = !isCurrentItemEquipped;
            lobbyButtonsView.EquipButton.interactable = !isCurrentItemEquipped;
        }

        private void UpdatePowerUpButtons()
        {
            if (IsProductSelected || game.Army.HasItemProgress)
            {
                lobbyButtonsView.LevelUpInstantButton.gameObject.SetActive(false);
                lobbyButtonsView.LevelUpRegularButton.gameObject.SetActive(false);
                lobbyButtonsView.GradeUpInstantButton.gameObject.SetActive(false);

                return;
            }

            if (SelectedItem is IArmyItem armyItem)
            {
                lobbyButtonsView.GradeUpInstantButton.gameObject.SetActive(
                    !armyItem.Grade.IsMax() && armyItem.IsMaxLevel);
                lobbyButtonsView.GradeUpInstantButton.Setup(
                    armyItem.InstantGradeUpPrice,
                    LocKeys.ItemInfo.GradeUp,
                    game.Player.CanPayPrice(armyItem.InstantGradeUpPrice),
                    actionDuration: TimeSpan.Zero);

                string lvlButtonLabel =
                    SelectedItem is ITrooperItem ? LocKeys.ItemInfo.Train : LocKeys.ItemInfo.Upgrade;

                lobbyButtonsView.LevelUpInstantButton.gameObject.SetActive(!armyItem.IsMaxLevel);
                lobbyButtonsView.LevelUpInstantButton.Setup(
                    armyItem.InstantLevelUpPrice,
                    lvlButtonLabel,
                    game.Player.CanPayPrice(armyItem.InstantLevelUpPrice),
                    actionDuration: TimeSpan.Zero);

                lobbyButtonsView.LevelUpRegularButton.gameObject.SetActive(!armyItem.IsMaxLevel);
                lobbyButtonsView.LevelUpRegularButton.Setup(
                    armyItem.RegularLevelUpPrice,
                    lvlButtonLabel,
                    game.Player.CanPayPrice(armyItem.RegularLevelUpPrice),
                    actionDuration: armyItem.RegularLevelUpDuration);
            }
        }

        private void GoToCurrencyShop(Price price)
        {
            var screen = ScreensController.Instance.ShowScreen<ShopScreen>();

            if (price.GoldAmount > 0)
            {
                screen.Setup(ShopCategory.Hard);
            }
            else if (price.CashAmount > 0)
            {
                screen.Setup(ShopCategory.Soft);
            }
        }

        private void OnItemProgressComplete(ArmyItemProgress itemProgress)
        {
            RefreshView();
        }

        private void BuyButton_OnClick()
        {
            if (SelectedItem.Product != null)
            {
                var product = SelectedItem.Product;
                if (game.Player.CanPayPrice(product.Price))
                {
                    IProductAcquiring productAcquiring = product;
                    productAcquiring.AcquireFailed += ProductAcquiring_AcquireFailed;
                    productAcquiring.AcquireSucceeded += ProductAcquiring_AcquireSucceeded;
                    productAcquiring.Acquire();
                }
                else
                {
                    GoToCurrencyShop(product.Price);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Can't buy {SelectedItem.ItemName} item. There is no product assigned with the item.");
            }
        }

        private void ProductAcquiring_AcquireFailed(IProductAcquiring productAcquiring)
        {
            // TODO: a.dezhurko Handle Failed
            productAcquiring.AcquireFailed -= ProductAcquiring_AcquireFailed;
            productAcquiring.AcquireSucceeded -= ProductAcquiring_AcquireSucceeded;
        }

        private void ProductAcquiring_AcquireSucceeded(IProductAcquiring productAcquiring)
        {
            productAcquiring.AcquireSucceeded -= ProductAcquiring_AcquireSucceeded;

            var screen = ScreensController.Instance.ShowPopup<AcquireConfirmationPopup>();
            RosterProduct rosterProduct = (RosterProduct)productAcquiring;
            screen.Setup(new IContentIdentity[] { rosterProduct.Content });
        }

        private async void LevelUpInstantButton_OnClick()
        {
            if (SelectedItem is IArmyItem armyItem)
            {
                if (armyItem.CanLevelUp)
                {
                    if (game.Player.CanPayPrice(armyItem.InstantLevelUpPrice))
                    {
                        game.Player.PayPrice(armyItem.InstantLevelUpPrice);
                        armyItem.LevelUp();
                        RefreshView();

                        var screen = ScreensController.Instance.ShowPopup<UpgradeContentPopup>();
                        screen.Setup(armyItem.ToContentIdentity(), armyItem.IsFirstLevelOnGrade);

                        AnalyticsManager.Instance.SendBuyingLevelUp(armyItem, true);

                        await armyItem.LevelUpInstantRequest(gameHubClient);
                    }
                    else
                    {
                        GoToCurrencyShop(armyItem.InstantLevelUpPrice);
                    }
                }
            }
        }

        private async void LevelUpRegularButton_OnClick()
        {
            if (SelectedItem is IArmyItem armyItem)
            {
                if (game.Army.CanStartItemProgress(armyItem.Id))
                {
                    if (game.Player.CanPayPrice(armyItem.RegularLevelUpPrice))
                    {
                        game.Player.PayPrice(armyItem.RegularLevelUpPrice);
                        game.Army.StartItemProgress(armyItem.Id);
                        RefreshView();
                        AnalyticsManager.Instance.SendBuyingLevelUp(armyItem, false);

                        await armyItem.LevelUpRegularRequest(gameHubClient);
                    }
                    else
                    {
                        GoToCurrencyShop(armyItem.RegularLevelUpPrice);
                    }
                }
            }
        }

        private async void GradeUpInstantButton_OnClick()
        {
            if (SelectedItem is IArmyItem armyItem)
            {
                if (armyItem.CanGradeUp)
                {
                    if (game.Player.CanPayPrice(armyItem.InstantGradeUpPrice))
                    {
                        game.Player.PayPrice(armyItem.InstantGradeUpPrice);
                        armyItem.GradeUp();
                        RefreshView();

                        var screen = ScreensController.Instance.ShowPopup<UpgradeContentPopup>();
                        screen.Setup(armyItem.ToContentIdentity(), armyItem.IsFirstLevelOnGrade);

                        AnalyticsManager.Instance.SendBuyingEvolution(armyItem);

                        await armyItem.GradeUpInstantRequest(gameHubClient);
                    }
                    else
                    {
                        GoToCurrencyShop(armyItem.InstantGradeUpPrice);
                    }
                }
            }
        }

        private async void EquipButton_OnClick()
        {
            lobbyButtonsView.EquipButtonLabel.SetLocalizedText(LocKeys.ItemInfo.Equipped);

            if (SelectedItem is IHelmetItem helmetItem)
            {
                if (game.Army.CanEquipHelmet(ActiveRoster.CurrentTrooperClass, helmetItem.Name))
                {
                    await gameHubClient.EquipHelmetAsync(ActiveRoster.CurrentTrooperClass, helmetItem.Name);
                    game.Army.EquipHelmet(ActiveRoster.CurrentTrooperClass, helmetItem.Name);
                    RefreshView();
                }
            }
            else if (SelectedItem is IArmorItem armorItem)
            {
                if (game.Army.CanEquipArmor(ActiveRoster.CurrentTrooperClass, armorItem.Name))
                {
                    await gameHubClient.EquipArmorAsync(ActiveRoster.CurrentTrooperClass, armorItem.Name);
                    game.Army.EquipArmor(ActiveRoster.CurrentTrooperClass, armorItem.Name);
                    RefreshView();
                }
            }
            else if (SelectedItem is IWeaponItem weaponItem)
            {
                if (game.Army.CanEquipWeapon(ActiveRoster.CurrentTrooperClass, weaponItem.Name))
                {
                    await gameHubClient.EquipWeaponAsync(ActiveRoster.CurrentTrooperClass, weaponItem.Name);
                    game.Army.EquipWeapon(ActiveRoster.CurrentTrooperClass, weaponItem.Name);
                    RefreshView();
                }
            }
        }

        private void CompleteProgressButton_OnClick()
        {
            var itemInProgress = game.Army.GetItemProgress();

            if (game.Player.CanPayPrice(itemInProgress.CompletePrice))
            {
                game.Player.PayPrice(itemInProgress.CompletePrice);
                itemInProgress.Complete();
            }
            else
            {
                GoToCurrencyShop(itemInProgress.CompletePrice);
            }
        }
    }
}