using Heyworks.PocketShooter.UI.Core;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.TrooperSelection
{
    public class TrooperSelectionPresenter : IDisposablePresenter
    {
        private readonly LobbyRosterPresenter rosterPresenter;
        private readonly Button fightButton;
        private readonly ITrooperSelectionHandler selectionHandler;

        public TrooperSelectionPresenter(
            LobbyRosterPresenter rosterPresenter,
            Button fightButton,
            ITrooperSelectionHandler selectionHandler)
        {
            this.rosterPresenter = rosterPresenter;
            this.fightButton = fightButton;
            this.selectionHandler = selectionHandler;

            fightButton.onClick.AddListener(FightOnClick);
        }

        public void Dispose()
        {
            fightButton.onClick.RemoveListener(FightOnClick);
        }

        private void FightOnClick()
        {
            if (rosterPresenter.SelectedCard.Item is TrooperInfoRosterItem item)
            {
                var selection = new TrooperSelectionParameters(item.Class, item.WeaponName, null);
                selectionHandler.OnSelected(selection);
            }
        }
    }
}