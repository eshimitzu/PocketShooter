using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    // ISSUE: event without direct link unity prevents build with same name
    public sealed class WeaponAttackSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public WeaponAttackSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer player)
        {
            if (player.CanAttack())
            {
                var weapon = (IWeaponForSystem)player.CurrentWeapon;
                weapon.State = WeaponState.Attacking;
                weapon.StateExpireAt = ticker.Current + weapon.Info.AttackInterval;

                if (weapon is IConsumableWeaponForSystem consumableOwnedWeapon)
                {
                    consumableOwnedWeapon.AmmoInClip--;
                }

                return true;
            }

            return false;
        }
    }
}
