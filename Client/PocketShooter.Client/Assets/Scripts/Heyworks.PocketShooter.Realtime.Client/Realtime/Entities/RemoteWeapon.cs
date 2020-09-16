using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class RemoteWeapon : WeaponBase
    {
        public RemoteWeapon(IRef<PlayerState> playerStateRef)
            : base(playerStateRef)
        {
        }

        internal void ApplyState(IRef<PlayerState> playerStateRef) => this.playerStateRef = playerStateRef;
    }
}