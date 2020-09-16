using System;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.Audio;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD.PainHUD
{
    /// <summary>
    /// Represents a pain HUD presenter.
    /// </summary>
    public class PainHUDPresenter : IDisposablePresenter
    {
        private readonly ICharacterContainer container;
        private readonly ClientPlayer localPlayer;
        private readonly LocalCharacter localCharacter;
        private readonly PainHUDStateController painHudStateController;

        public PainHUDPresenter(PainHUDView view, ClientPlayer localPlayer, ICharacterContainer container)
        {
            view.NotNull();
            this.container = container.NotNull();
            this.localPlayer = localPlayer.NotNull();

            localCharacter = (LocalCharacter)container.GetCharacter(localPlayer.Id);
            painHudStateController = new PainHUDStateController(localCharacter.transform);
            painHudStateController.Clear();

            view.Init(painHudStateController);
            view.gameObject.SetActive(true);

            localCharacter.Damaged += LocalCharacter_Damaged;

            // localPlayer.Dead += LocalPlayer_Dead;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            localCharacter.Damaged -= LocalCharacter_Damaged;
        }

        private void LocalCharacter_Damaged(int attackerId, float damage)
        {
            NetworkCharacter attackerCharacter = container.GetCharacter(attackerId);

            AudioController.Instance.PostEvent(AudioKeys.Event.PlaySuffer, attackerCharacter.gameObject);

            // character may be dead or missing, should attach to player model ?
            if (attackerCharacter)
            {
                painHudStateController.Damaged(attackerCharacter.transform, damage, attackerCharacter.Model.Health.MaxHealth);
            }
        }
    }
}
