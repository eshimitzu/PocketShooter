using System;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.UI.Core;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyFightButtonPresenter : IDisposablePresenter
    {
        private LobbyProfileBar profileBar;
        private Main main;

        public LobbyFightButtonPresenter(LobbyProfileBar profileBar, Main main)
        {
            this.profileBar = profileBar;
            this.main = main;

            main.ConnectionStateChanged += Lobby_ConnectionStateChanged;
            profileBar.OnFightClick += OnFightClick;

            UpdateStartMatchButton(main.ConnectionState);
        }

        public void Dispose()
        {
            main.ConnectionStateChanged -= Lobby_ConnectionStateChanged;
            profileBar.OnFightClick -= OnFightClick;
        }

        private void Lobby_ConnectionStateChanged(ConnectionState connectionState)
        {
            UpdateStartMatchButton(connectionState);
        }

        private void UpdateStartMatchButton(ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case ConnectionState.Disconnected:
                case ConnectionState.Connecting:
                case ConnectionState.MatchMaking:
                case ConnectionState.JoiningRoom:
                    profileBar.FightButton.interactable = false;
                    break;
                case ConnectionState.Connected:
                    profileBar.FightButton.interactable = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionState), connectionState, null);
            }
        }

        private async void OnFightClick()
        {
            await main.StartMatchMaking();
        }
    }
}
