using Collections.Pooled;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;
using Heyworks.PocketShooter.Realtime.Simulation;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    /// <summary>
    /// Represents trooper's weapon. Is responsible for shooting frequency and auto aim control.
    /// </summary>
    public abstract class WeaponController : MonoBehaviour
    {
        private WeaponFireVisualiser weaponFireVisualiser;

        public IOwnedWeapon Weapon { get; private set; }

        protected IClientSimulation Simulation { get; private set; }

        protected LocalCharacter LocalCharacter { get; private set; }

        protected WeaponRaycaster WeaponRaycaster { get; private set; }

        protected AnimationController AnimationController { get; private set; }

        protected PooledList<ClientShotInfo> LastShotInfo { get; private set; }

        public virtual void Setup(IOwnedWeapon weapon, IClientSimulation clientSimulation, ITickEvents tickEvents, LocalCharacter localCharacter, WeaponRaycaster weaponRaycaster, WeaponFireVisualiser weaponFireVisualiser)
        {
            LocalCharacter = localCharacter;
            AnimationController = localCharacter.CharacterCommon.AnimationController;
            WeaponRaycaster = weaponRaycaster;
            Weapon = weapon;
            Simulation = clientSimulation;
            this.weaponFireVisualiser = weaponFireVisualiser;

            LastShotInfo = new PooledList<ClientShotInfo>
            {
                Capacity = Weapon.Info.Fraction,
            };

            LocalCharacter.Model.Events.WeaponStateChanged.Subscribe(UpdateWeaponState).AddTo(this);
            tickEvents.SimulationTick.TakeUntilDisable(this).Subscribe(UpdateOnTick).AddTo(this);
        }

        protected abstract bool TestIfEnemyIsInCrosshair();

        protected virtual void HandleEnemyIsInCrosshair()
        {
            if (Weapon is IWarmingUpWeapon &&
                !LocalCharacter.Model.Effects.Stun.IsStunned)
            {
                var commandData = new WarmingUpCommandData(LocalCharacter.Model.Id);
                LocalCharacter.AddCommand(commandData);
            }
        }

        protected abstract void Attack();

        protected virtual void UpdateWeaponState(WeaponStateChangedEvent e)
        {
            if (e.Next == WeaponState.Attacking)
            {
                ProcessShot();
            }
        }

        protected virtual void ProcessShot()
        {
            AnimationController.Shoot();

            for (int i = 0; i < LastShotInfo.Count; i++)
            {
                weaponFireVisualiser.VizualizeAttack(new[] { LastShotInfo[i].Point });
            }
        }

        private void UpdateOnTick(TickEvent e)
        {
            var isEnemyInCrosshair = TestIfEnemyIsInCrosshair();

            AnimationController.Aim(isEnemyInCrosshair);

            if (isEnemyInCrosshair)
            {
                HandleEnemyIsInCrosshair();
            }

            if (isEnemyInCrosshair && LocalCharacter.Model.CanAttack())
            {
                Attack();
            }
        }
    }
}
