using Collections.Pooled;
using Heyworks.PocketShooter.Camera;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;
using Heyworks.PocketShooter.Realtime.Simulation;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponControllerFirearm : WeaponController
    {
        [Inject] 
        private CameraShake cameraShake;

        private Vector3 lastTestedAim;

        protected WeaponRaycasterFirearm WeaponRaycasterFirearm => WeaponRaycaster as WeaponRaycasterFirearm;

        public override void Setup(IOwnedWeapon weapon, IClientSimulation clientSimulation, ITickEvents tickEvents,
            LocalCharacter localCharacter, WeaponRaycaster weaponRaycaster, WeaponFireVisualiser weaponFireVisualiser)
        {
            base.Setup(weapon, clientSimulation, tickEvents, localCharacter, weaponRaycaster, weaponFireVisualiser);

            LocalCharacter.Model.Events.AmmoChanged.Subscribe(UpdateAmmo).AddTo(this);
        }

        protected override bool TestIfEnemyIsInCrosshair()
        {
            if (WeaponRaycasterFirearm.Test(Weapon.Info.MaxRange, out lastTestedAim))
            {
                Vector3 position = WeaponRaycaster.ShotOriginTransfrom.position;
                var ray = new Ray(position, (lastTestedAim - position).normalized);

                if (WeaponRaycasterFirearm.Test(ray, Weapon.Info.MaxRange, out lastTestedAim))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void Attack()
        {
            var position = WeaponRaycaster.ShotOriginTransfrom.position;
            var ray = new Ray(position, (lastTestedAim - position).normalized);

            LastShotInfo.Clear();

            WeaponRaycasterFirearm.Shoot(ray, Weapon.Info.MaxRange, Weapon.Info.Dispersion, Weapon.Info.Fraction,
                Weapon.Info.FractionDispersion, LastShotInfo);

            var shots = new PooledList<ShotInfo>();

            for (int i = 0; i < LastShotInfo.Count; i++)
            {
                var id = LastShotInfo[i].Target == null ? default(EntityId) : LastShotInfo[i].Target.Id;
                shots.Add(new ShotInfo(id, Weapon.Name, LastShotInfo[i].IsCritical));
            }

            if (shots.Count > 0)
            {
                var commandData = new AttackCommandData(LocalCharacter.Model.Id, shots);
                LocalCharacter.AddCommand(commandData);
            }
        }

        protected override void UpdateWeaponState(WeaponStateChangedEvent e)
        {
            base.UpdateWeaponState(e);

            if (e.Next == WeaponState.Reloading)
            {
                Reload();
            }
        }

        protected override void ProcessShot()
        {
            base.ProcessShot();

            if (cameraShake != null)
            {
                cameraShake.ShakeCamera(WeaponName.M16A4);
            }
        }

        private void Reload()
        {
            AnimationController.Reload();
        }

        private void UpdateAmmo(int ammoValue)
        {
            switch (Weapon)
            {
                case IConsumableWeapon ammo when ammo.AmmoInClip == 0:
                    LocalCharacter.AddCommand(new ReloadCommandData(LocalCharacter.Model.Id));
                    break;
            }
        }
    }
}