using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Lobby;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.UI.Popups;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyScreen : BaseScreen, ILobbyRosterProvider
    {
        [SerializeField]
        private LobbyProfileBar lobbyProfileBar;
        [SerializeField]
        private LobbySelectionView lobbySelectionView;
        [SerializeField]
        private LobbyButtonsView lobbyButtonsView;
        [SerializeField]
        private SideItemProgressView sideItemProgressView;
        [SerializeField]
        private LobbyRosterView trooperRosterView;
        [SerializeField]
        private LobbyRosterView weaponRosterView;
        [SerializeField]
        private LobbyRosterView helmetRosterView;
        [SerializeField]
        private LobbyRosterView armorRosterView;
        [SerializeField]
        private IconsFactory itemsFactory;
        [SerializeField]
        private OfferItemsFactory offerItemsFactory;
        [SerializeField]
        private LobbyConsumablesView consumablesView;

        [Inject]
        private TrooperCreator trooperCreator;
        [Inject]
        private ScreensController screensController;

        private readonly List<ILobbyRosterPresenter> rosterPresenters = new List<ILobbyRosterPresenter>();

        private LobbyRosterPresenter trooperPresenter;
        private LobbyRosterPresenter weaponPresenter;
        private LobbyRosterPresenter armorPresenter;
        private LobbyRosterPresenter helmetPresenter;
        private LobbyScreenPresenter screenPresenter;
        private LobbySelectionPresenter selectionPresenter;
        private OfferPopupPresenter offerPopupPresenter;
        private LobbyConsumablesPresenter consumablesPresenter;
        private Main main;

        public ILobbyRosterPresenter CurrentRoster { get; private set; }

        private void Update()
        {
            consumablesPresenter?.Update();
            selectionPresenter?.Update();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        protected override void OnDestroy()
        {
            base.OnDestroy();
            main.MetaGame.Shop.ProductsListUpdated -= Shop_ProductsListUpdated;
            main.MetaGame.GameStateChanged -= Lobby_GameStateReset;
        }

        public void Setup(Main main, bool isSetupAfterBattle = false)
        {
            this.main = main;

            DisposePresenters();

            screenPresenter = new LobbyScreenPresenter(lobbyProfileBar, main);
            AddDisposablePresenter(screenPresenter);

            var fightButtonPresenter = new LobbyFightButtonPresenter(lobbyProfileBar, main);
            AddDisposablePresenter(fightButtonPresenter);

            var trooperSelection = new TrooperSelectionHandler(main.MetaGame);

            trooperPresenter = new LobbyRosterPresenter(
                trooperRosterView,
                itemsFactory,
                RosterType.Trooper,
                main.MetaGame.GetTrooperItems(),
                trooperSelection,
                main.MetaGame);
            rosterPresenters.Add(trooperPresenter);

            weaponPresenter = new LobbyRosterPresenter(
                weaponRosterView,
                itemsFactory,
                RosterType.Weapon,
                main.MetaGame.GetWeaponItems(),
                trooperSelection,
                main.MetaGame);
            rosterPresenters.Add(weaponPresenter);

            helmetPresenter = new LobbyRosterPresenter(
                helmetRosterView,
                itemsFactory,
                RosterType.Helmet,
                main.MetaGame.GetHelmetItems(),
                trooperSelection,
                main.MetaGame);
            rosterPresenters.Add(helmetPresenter);

            armorPresenter = new LobbyRosterPresenter(
                armorRosterView,
                itemsFactory,
                RosterType.Armor,
                main.MetaGame.GetArmorItems(),
                trooperSelection,
                main.MetaGame);
            rosterPresenters.Add(armorPresenter);

            foreach (ILobbyRosterPresenter roster in rosterPresenters)
            {
                AddDisposablePresenter(roster);
                roster.OnSelectedCardChanged += presenter => selectionPresenter.ShowCard(presenter, false);
            }

            selectionPresenter = new LobbySelectionPresenter(
                lobbySelectionView,
                trooperCreator,
                itemsFactory);

            var upgradeButtonsPresenter = new LobbyUpgradeButtonsPresenter(
                lobbyButtonsView,
                selectionPresenter,
                sideItemProgressView,
                this,
                main.MetaGame,
                main.MetaGame.GameHubClient);

            AddDisposablePresenter(selectionPresenter);
            AddDisposablePresenter(upgradeButtonsPresenter);

            lobbyProfileBar.OnStoreClick += LobbyProfileBar_OnStoreClick;
            lobbyProfileBar.OnBackClick += () => SetCurrentRoster(trooperPresenter);
            lobbySelectionView.WeaponButton.OnClick += () => SetCurrentRoster(weaponPresenter, LocKeys.GetWeaponNameKey(((TrooperRosterItem)selectionPresenter.SelectedItem).WeaponName));
            lobbySelectionView.ArmorButton.OnClick += () => SetCurrentRoster(armorPresenter, LocKeys.GetArmorNameKey(((TrooperRosterItem)selectionPresenter.SelectedItem).ArmorName));
            lobbySelectionView.HelmetButton.OnClick += () => SetCurrentRoster(helmetPresenter, LocKeys.GetHelmetNameKey(((TrooperRosterItem)selectionPresenter.SelectedItem).HelmetName));

            main.MetaGame.GameStateChanged += Lobby_GameStateReset;
            main.MetaGame.Shop.ProductsListUpdated += Shop_ProductsListUpdated;
            SetCurrentRoster(trooperPresenter);

            offerPopupPresenter = new OfferPopupPresenter(main.MetaGame.СonfigProvider, main.MetaGame.Shop, offerItemsFactory);

            consumablesPresenter = new LobbyConsumablesPresenter(screensController, main.MetaGame, consumablesView);
            AddDisposablePresenter(consumablesPresenter);

            AddDisposablePresenter(offerPopupPresenter);

            if (isSetupAfterBattle)
            {
                offerPopupPresenter.LobbyScreenLoadedAfterBattle();
            }

            main.NotificationController.Notify();
        }

        public void SelectItem(IContentIdentity itemContentIdentity)
        {
            switch (itemContentIdentity)
            {
                case TrooperIdentity trooper:
                    SetCurrentRoster(trooperPresenter, LocKeys.GetTooperNameKey(trooper.Class));
                    break;
                case ArmorIdentity armor:
                    SetCurrentRoster(armorPresenter, LocKeys.GetArmorNameKey(armor.Name));
                    break;
                case HelmetIdentity helmet:
                    SetCurrentRoster(helmetPresenter, LocKeys.GetHelmetNameKey(helmet.Name));
                    break;
                case WeaponIdentity weapon:
                    SetCurrentRoster(weaponPresenter, LocKeys.GetWeaponNameKey(weapon.Name));
                    break;
            }
        }

        private void SetCurrentRoster(ILobbyRosterPresenter roster, string selectedItemName)
        {
            roster.UpdateSelected(selectedItemName);
            SetCurrentRoster(roster);
        }

        private void SetCurrentRoster(ILobbyRosterPresenter roster)
        {
            CurrentRoster = roster;

            foreach (var rosterPresenter in rosterPresenters)
            {
                rosterPresenter.SetVisibleView(rosterPresenter == roster);
            }

            lobbyProfileBar.BackButton.gameObject.SetActive(roster.RosterView != trooperRosterView);
            lobbyProfileBar.ProfileView.gameObject.SetActive(roster.RosterView == trooperRosterView);
            selectionPresenter.ShowCard(roster.SelectedCard, false);
        }

        private void SetupRosters()
        {
            trooperPresenter.SetupCards(main.MetaGame.GetTrooperItems(), true);
            weaponPresenter.SetupCards(main.MetaGame.GetWeaponItems(), true);
            helmetPresenter.SetupCards(main.MetaGame.GetHelmetItems(), true);
            armorPresenter.SetupCards(main.MetaGame.GetArmorItems(), true);
        }

        private void Lobby_GameStateReset()
        {
            SetupRosters();
        }

        private void Shop_ProductsListUpdated()
        {
            SetupRosters();
        }

        private void LobbyProfileBar_OnStoreClick(ShopCategory categoryType)
        {
            screensController.ShowStoreWhenReadyAsync(main.MetaGame, categoryType);
        }
    }
}