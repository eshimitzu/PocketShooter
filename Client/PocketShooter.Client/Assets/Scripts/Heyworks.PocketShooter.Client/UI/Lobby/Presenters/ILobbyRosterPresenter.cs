using System;
using Heyworks.PocketShooter.UI.Core;

namespace Heyworks.PocketShooter.UI
{
    public interface ILobbyRosterPresenter : IDisposablePresenter
    {
        RosterType RosterType { get; }

        ILobbyCardPresenter SelectedCard { get; }

        LobbyRosterView RosterView { get; }

        TrooperClass CurrentTrooperClass { get; }

        event Action<ILobbyCardPresenter> OnSelectedCardChanged;

        void UpdateSelected(string itemName);

        void SetVisibleView(bool isVisible);

        void Refresh();
    }
}