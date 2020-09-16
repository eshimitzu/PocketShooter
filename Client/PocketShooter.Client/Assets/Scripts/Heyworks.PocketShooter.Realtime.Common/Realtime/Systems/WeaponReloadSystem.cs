using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class WeaponReloadSystem : OwnerSystem
    {
        private readonly ITicker ticks;

        public WeaponReloadSystem(ITicker ticks)
        {
            this.ticks = ticks;
        }

        public override bool Execute(OwnedPlayer player)
        {
            if (player.CurrentWeapon is IConsumableWeaponForSystem weapon)
            {
                if (weapon.State == WeaponState.Reloading && ticks.Current >= weapon.StateExpireAt)
                {
                    Reload(weapon);

                    return true;
                }
            }

            return false;
        }

        private void Reload(IConsumableWeaponForSystem weapon)
        {
            weapon.State = WeaponState.Default;
            weapon.AmmoInClip = weapon.Info.ClipSize;
        }
    }
}