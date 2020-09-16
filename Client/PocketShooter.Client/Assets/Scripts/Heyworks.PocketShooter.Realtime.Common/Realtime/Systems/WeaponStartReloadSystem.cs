using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class WeaponStartReloadSystem : OwnerSystem
    {
        private readonly ITicker ticks;

        public WeaponStartReloadSystem(ITicker ticks)
        {
            this.ticks = ticks;
        }

        public override bool Execute(OwnedPlayer player)
        {
            if (player.CurrentWeapon is IConsumableWeaponForSystem weapon &&
                weapon.AmmoInClip < weapon.Info.ClipSize &&
                weapon.State != WeaponState.Reloading)
            {
                weapon.State = WeaponState.Reloading;
                weapon.StateExpireAt = ticks.Current + weapon.Info.ReloadTime;

                return true;
            }

            return false;
        }
    }
}
