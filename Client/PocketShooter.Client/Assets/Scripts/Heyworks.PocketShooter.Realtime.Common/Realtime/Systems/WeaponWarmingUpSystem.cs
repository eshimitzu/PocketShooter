using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class WeaponWarmingUpSystem : OwnerSystem
    {
        public override bool Execute(OwnedPlayer initiator)
        {
            if (initiator.CurrentWeapon is IWarmingUpWeaponForSystem warmupOwnedWeapon)
            {
                if (warmupOwnedWeapon.WarmupWeaponState == WarmupWeaponState.WarmingUp)
                {
                    warmupOwnedWeapon.WarmupWeaponState = WarmupWeaponState.Cooling;

                    if (warmupOwnedWeapon.WarmupProgress >= 1f)
                    {
                        return false;
                    }

                    warmupOwnedWeapon.WarmupProgress += warmupOwnedWeapon.WarmingSpeed * Constants.TickIntervalMs;
                }
                else if (warmupOwnedWeapon.WarmupWeaponState == WarmupWeaponState.Cooling)
                {
                    if (warmupOwnedWeapon.WarmupProgress <= 0f)
                    {
                        return false;
                    }

                    warmupOwnedWeapon.WarmupProgress -= warmupOwnedWeapon.CoolingSpeed * Constants.TickIntervalMs;
                }

                warmupOwnedWeapon.WarmupProgress = Math.Min(Math.Max(0f, warmupOwnedWeapon.WarmupProgress), 1f);

                return true;
            }

            return false;
        }
    }
}
