using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class WeaponStartWarmingUpSystem : OwnerSystem
    {
        public override bool Execute(OwnedPlayer initiator)
        {
            if (initiator.CurrentWeapon is IWarmingUpWeaponForSystem warmupOwnedWeapon)
            {
                if (warmupOwnedWeapon.State != WeaponState.Reloading)
                {
                    warmupOwnedWeapon.WarmupWeaponState = WarmupWeaponState.WarmingUp;

                    return true;
                }
            }

            return false;
        }
    }
}
