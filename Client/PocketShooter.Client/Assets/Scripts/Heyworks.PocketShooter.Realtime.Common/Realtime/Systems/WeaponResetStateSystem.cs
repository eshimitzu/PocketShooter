using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class WeaponResetStateSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public WeaponResetStateSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer player)
        {
            var weapon = (IWeaponForSystem)player.CurrentWeapon;
            if (weapon.State != WeaponState.Default && ticker.Current >= weapon.StateExpireAt)
            {
                weapon.State = WeaponState.Default;
                return true;
            }

            return false;
        }
    }
}