using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Character;
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
    // in lobby this view used for different rosters
    public class LobbySelectionPresenter : BaseSelectionPresenter
    {
        private readonly LobbySelectionView lobbySelectionView;

        private bool IsProductSelected => SelectedItem.Product != null;

        public ILobbyRosterItem SelectedItem => CardPresenter.Item;

        public event System.Action OnSelectionChanged;

        public LobbySelectionPresenter(
            LobbySelectionView lobbySelectionView,
            TrooperCreator trooperCreator,
            IconsFactory itemsFactory)
            : base(lobbySelectionView, trooperCreator, itemsFactory)
        {
            this.lobbySelectionView = lobbySelectionView;
        }

        public override void ShowCard(ILobbyCardPresenter presenter, bool inBattle)
        {
            base.ShowCard(presenter, inBattle);

            // TODO: v.shimkovich PSH-844
            if (presenter == null)
            {
                return;
            }

            if (IsProductSelected)
            {
                lobbySelectionView.ItemsBar.gameObject.SetActive(false);
            }

            OnSelectionChanged?.Invoke();
        }
    }
}