using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class ClientGameState
    {
        public ClientPlayerState Player { get; set; }

        public ArmyState Army { get; set; }

        public DateTime ServerTimeUtc { get; set; }
    }
}
