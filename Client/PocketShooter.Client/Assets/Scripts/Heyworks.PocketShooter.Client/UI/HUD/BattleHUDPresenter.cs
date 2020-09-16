using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.HUD.Buttons;
using Heyworks.PocketShooter.Utils;
using SRF;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD
{
    /// <summary>
    /// Represents an object responsible for collecting and managing player's HUD information.
    /// </summary>
    internal class BattleHUDPresenter : IDisposablePresenter
    {
        private readonly BattleHUDView view;
        private readonly LocalCharacter localCharacter;
        private readonly IClientPlayer localPlayer;
        private readonly Game game;
        private readonly Room room;
        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        private readonly Dictionary<EntityId, IDisposable> killedPlayerSubscriptions =
            new Dictionary<EntityId, IDisposable>();

        private readonly List<IDisposable> presenters = new List<IDisposable>();

        private GameObject reloadEffect;

        private BattleNotificationPresenter battleNotificationPresenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BattleHUDPresenter" /> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="localCharacter">The local character.</param>
        /// <param name="matchInfo">The matchInfo.</param>
        /// <param name="game">The game.</param>
        /// <param name="room">The room.</param>
        /// <param name="weaponViewsConfig">Weapons configuration.</param>
        public BattleHUDPresenter(
            BattleHUDView view,
            LocalCharacter localCharacter,
            Game game,
            Room room,
            WeaponsConfig weaponViewsConfig,
            GameVisualSettings gameVisualSettings)
        {
            this.view = view.NotNull();
            this.localCharacter = localCharacter.NotNull();
            this.localPlayer = localCharacter.Model.NotNull();
            this.game = game;
            this.room = room;

            // TODO: player only events via LocalPlayer. ServerPlayer has not events.
            // Observables(to allow ref data in future) - not Properties.

            localPlayer.Events.AmmoChanged.Subscribe(UpdateAmmo).AddTo(subscriptions);
            localPlayer.Events.ExpendablesChanged.Subscribe(UpdateHealth).AddTo(subscriptions);
            localPlayer.Events.WarmingUpProgressChanged.Subscribe(UpdateWarmingUpProgress).AddTo(subscriptions);
            localPlayer.Events.WeaponStateChanged.Subscribe(Weapon_OnStateChanged).AddTo(subscriptions);

            game.RemotePlayerJoined.Subscribe(Game_RemotePlayerJoined).AddTo(subscriptions);
            game.RemotePlayerRespawned.Subscribe(RemotePlayer_OnRespawned).AddTo(subscriptions);
            game.RemotePlayerLeaved.Select(x => x.RemotePlayer).Subscribe(Game_RemotePlayerLeaved).AddTo(subscriptions);
            game.WorldCleared.Subscribe(Game_WorldCleared).AddTo(subscriptions);

            IDisposable localPlayerSubscribtion = localPlayer.Events.Killed.Subscribe(Player_OnKilled);
            killedPlayerSubscriptions.Add(localPlayer.Id, localPlayerSubscribtion);

            foreach (IRemotePlayer player in game.Players.Values)
            {
                IDisposable subscribtion = player.Events.Killed.Subscribe(Player_OnKilled);
                killedPlayerSubscriptions.Add(player.Id, subscribtion);
            }

            view.ReloadWeapon += View_ReloadWeapon;
            view.OpenMenu += View_OpenMenu;

            var characterConfig = localCharacter.Model.Info;
            view.SetNickname(localPlayer.Nickname);
            view.SetHealthViewBars(characterConfig.MaxHealth, characterConfig.MaxArmor, gameVisualSettings.HealthForSegment, gameVisualSettings.ArmorForSegment);
            view.UpdateHealth(
                localPlayer.Health.Health,
                localPlayer.Armor.Armor,
                characterConfig.MaxHealth,
                characterConfig.MaxArmor);
            UpdateAmmo();

            presenters.Add(CreateSkillButtonPresenter(view.SkillButtonFirst, localCharacter.FirstSkillController));
            presenters.Add(CreateSkillButtonPresenter(view.SkillButtonSecond, localCharacter.SecondSkillController));
            presenters.Add(CreateSkillButtonPresenter(view.SkillButtonThird, localCharacter.ThirdSkillController));
            presenters.Add(CreateSkillButtonPresenter(view.SkillButtonFourth, localCharacter.FourthSkillController));
            presenters.Add(CreateSkillButtonPresenter(view.SkillButtonFifth, localCharacter.FifthSkillController));

            var weaponConfig = weaponViewsConfig.GetWeaponByName(localPlayer.CurrentWeapon.Name);
            var crosshair = weaponConfig.WeaponCrosshairIcon;
            view.SetCrosshairImage(crosshair, weaponConfig.CrosshairRelativeSize);

            bool hasReloading = localPlayer.CurrentWeapon is IConsumableWeapon;
            view.TrooperInfoBarView.SetReloadState(hasReloading);

            view.SetScopeButtonVisible(weaponConfig.SniperScope);

            battleNotificationPresenter = new BattleNotificationPresenter(view.BattleNotificationView, room, gameVisualSettings);
            presenters.Add(battleNotificationPresenter);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            view.ReloadWeapon -= View_ReloadWeapon;
            view.OpenMenu -= View_OpenMenu;

            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            subscriptions.Clear();

            foreach (var subscription in killedPlayerSubscriptions.Values)
            {
                subscription.Dispose();
            }

            killedPlayerSubscriptions.Clear();

            foreach (var presenter in presenters)
            {
                presenter.Dispose();
            }

            presenters.Clear();
        }

        public void Update()
        {
            battleNotificationPresenter.Update();
        }

        private void UpdateHealth(EntityId any)
        {
            var characterConfig = localPlayer.Info;

            view.UpdateHealth(
                localPlayer.Health.Health,
                localPlayer.Armor.Armor,
                characterConfig.MaxHealth,
                characterConfig.MaxArmor);
        }

        private void UpdateAmmo(int ammoValue = 0)
        {
            switch (localPlayer.CurrentWeapon)
            {
                case IConsumableWeapon ammo:
                    view.UpdateAmmo(ammo.AmmoInClip, localPlayer.CurrentWeapon.Info.ClipSize);
                    break;
            }
        }

        private void UpdateWarmingUpProgress(float progress)
        {
            view.UpdateCrosshairProgress(progress);
        }

        private SkillButtonPresenter CreateSkillButtonPresenter(
            HUDButtonCooldown hudbuttonCooldown,
            SkillController skillController)
        {
            if (skillController.Model.Skill is ConsumableSkill)
            {
                if (skillController.SkillName == SkillName.MedKit)
                {
                    return new MedKitSkillButtonPresenter(
                        hudbuttonCooldown,
                        skillController,
                        localCharacter,
                        game);
                }
                else
                {
                    return new ConsumableSkillButtonPresenter(hudbuttonCooldown, skillController, localCharacter, game);
                }
            }
            else
            {
                return new SkillButtonPresenter(hudbuttonCooldown, skillController, localCharacter, game);
            }
        }

        private void View_ReloadWeapon() => localCharacter.Reload();

        private void View_OpenMenu() => room.ManualDisconnect();

        private void Game_RemotePlayerJoined(RemotePlayerJoinedEvent rpj)
        {
            IRemotePlayer remotePlayer = rpj.RemotePlayer;

            if (killedPlayerSubscriptions.ContainsKey(remotePlayer.Id))
            {
                DisposeKilledPlayerSubscribe(remotePlayer.Id);
            }

            IDisposable subscribtion = remotePlayer.Events.Killed.Subscribe(Player_OnKilled);
            killedPlayerSubscriptions.Add(remotePlayer.Id, subscribtion);
        }

        private void Game_RemotePlayerLeaved(IRemotePlayer remotePlayer)
        {
            DisposeKilledPlayerSubscribe(remotePlayer.Id);
        }

        private void Game_WorldCleared(ResetWorldEvent rwe)
        {
            foreach (var playerSubscription in killedPlayerSubscriptions.ToList())
            {
                killedPlayerSubscriptions[playerSubscription.Key].Dispose();
                killedPlayerSubscriptions.Remove(playerSubscription.Key);
            }
        }

        private void RemotePlayer_OnRespawned(RemotePlayerRespawnedEvent rse)
        {
            EntityId remotePlayerID = rse.RemotePlayer.Id;

            if (killedPlayerSubscriptions.ContainsKey(remotePlayerID))
            {
                DisposeKilledPlayerSubscribe(remotePlayerID);
            }

            IDisposable subscribtion = game.Players[remotePlayerID].Events.Killed.Subscribe(Player_OnKilled);
            killedPlayerSubscriptions.Add(remotePlayerID, subscribtion);
        }

        private void Player_OnKilled(KilledServerEvent kse)
        {
            if (kse.Killed != localPlayer.Id)
            {
                DisposeKilledPlayerSubscribe(kse.Killed);
            }

            var killer = game.GetPlayer(kse.ActorId);
            var victim = game.GetPlayer(kse.Killed);
            var myTeam = localPlayer.Team;

            view.ShowKill(killer.Nickname, victim.Nickname, killer.Team == myTeam, victim.Team == myTeam, kse.ActorId == game.LocalPlayerId, kse.Killed == game.LocalPlayerId);

            if (reloadEffect)
            {
                UnityEngine.Object.Destroy(reloadEffect);
                reloadEffect = null;
            }

            if (kse.ActorId == localCharacter.Id)
            {
                battleNotificationPresenter.ProcessKill();
            }
            else if (kse.Killed == localCharacter.Id)
            {
                battleNotificationPresenter.Killed();
            }
        }

        private void Weapon_OnStateChanged(WeaponStateChangedEvent vse)
        {
            if (vse.Next == WeaponState.Reloading)
            {
                view.CrosshairView.SetVisible(false);
                reloadEffect = EffectsManager.Instance.PlayEffect(
                    EffectType.Reload,
                    view.CrosshairView.transform,
                    Vector3.zero,
                    true);
                reloadEffect.transform.localScale = Vector3.one * 400;
                reloadEffect.SetLayerRecursive(view.gameObject.layer);
            }
            else
            {
                view.CrosshairView.SetVisible(true);
                if (reloadEffect)
                {
                    UnityEngine.Object.Destroy(reloadEffect);
                    reloadEffect = null;
                }
            }
        }

        private void DisposeKilledPlayerSubscribe(EntityId playerID)
        {
            killedPlayerSubscriptions[playerID].Dispose();
            killedPlayerSubscriptions.Remove(playerID);
        }
    }
}