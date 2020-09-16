using System;

namespace Heyworks.PocketShooter.UI
{
    public interface ILobbyCardPresenter : IDisposable
    {
        int Index { get; }

        ILobbyRosterItem Item { get; }

        void Setup();

        void SetSelected(bool selected);

        void UpdateWeaponIcon(WeaponName weaponName);

        void SetVisibleWeaponIcon(bool isVisible);
    }
}