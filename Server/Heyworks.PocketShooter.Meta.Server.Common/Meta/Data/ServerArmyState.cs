using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class ServerArmyState : ArmyState
    {
        public ServerArmyState()
        {
        }

        public ServerArmyState(Guid playerId)
        {
            PlayerId = playerId;
        }

        public Guid PlayerId { get; set; }

        public ArmyState ToClientState()
        {
            return new ArmyState()
            {
                Troopers = Troopers,
                Weapons = Weapons,
                Helmets = Helmets,
                Armors = Armors,
                Offensives = Offensives,
                Supports = Supports,
                ItemProgress = ItemProgress,
                NextItemId = NextItemId,
            };
        }
    }
}
