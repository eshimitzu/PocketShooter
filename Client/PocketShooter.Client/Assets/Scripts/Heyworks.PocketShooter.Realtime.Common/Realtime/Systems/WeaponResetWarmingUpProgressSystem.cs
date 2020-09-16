using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class WeaponResetWarmingUpProgressSystem : OwnerSystem
    {
        public override bool Execute(OwnedPlayer player)
        {
            if (player.CurrentWeapon is IWarmingUpWeaponForSystem warmupOwnedWeapon)
            {
                if (warmupOwnedWeapon.ResetProgressOnShot && warmupOwnedWeapon.State == WeaponState.Attacking)
                {
                    warmupOwnedWeapon.WarmupProgress = 0f;

                    return true;
                }
            }

            return false;
        }
    }
}
