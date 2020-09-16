using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Weapons;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD
{
    public class SniperViewPresenter : IDisposablePresenter
    {
        private readonly GameObject sniperViewRoot;
        private readonly LocalCharacter localCharacter;
        private readonly IScopeControl scopeControl;
        private readonly WeaponScopeBehaviour weaponScopeBehaviour;
        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        public SniperViewPresenter(GameObject sniperViewRoot, LocalCharacter localCharacter, IScopeControl scopeControl)
        {
            AssertUtils.NotNull(sniperViewRoot, nameof(sniperViewRoot));
            AssertUtils.NotNull(localCharacter, nameof(localCharacter));
            AssertUtils.NotNull(scopeControl, nameof(scopeControl));

            this.localCharacter = localCharacter;
            this.sniperViewRoot = sniperViewRoot;
            this.scopeControl = scopeControl;
            weaponScopeBehaviour = localCharacter.GetComponent<WeaponScopeBehaviour>();

            scopeControl.Scope += View_Scope;
            weaponScopeBehaviour.ScopeOut += WeaponScopeBehaviour_ScopeOut;
        }

        public void Dispose()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            subscriptions.Clear();

            scopeControl.Scope -= View_Scope;
            weaponScopeBehaviour.ScopeOut -= WeaponScopeBehaviour_ScopeOut;

            ShowSniperHud(false);
        }

        private void ShowSniperHud(bool isShowing)
        {
            sniperViewRoot.SetActive(isShowing);
            scopeControl.CrosshairView.SetSniperScope(isShowing);
        }

        private void View_Scope()
        {
            weaponScopeBehaviour.Use(!weaponScopeBehaviour.IsScopeActive);
            ShowSniperHud(weaponScopeBehaviour.IsScopeActive);
        }

        private void WeaponScopeBehaviour_ScopeOut()
        {
            ShowSniperHud(false);
        }
    }
}
