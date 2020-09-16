using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class ServerGameState
    {
        public ServerPlayerState Player { get; set; }

        public ServerArmyState Army { get; set; }

        public DateTime UtcNow { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ClientGameState"/> class.
        /// </summary>
        public ClientGameState ToClientState()
        {
            return new ClientGameState()
            {
                Player = Player.ToClientState(),
                Army = Army.ToClientState(),
                ServerTimeUtc = UtcNow,
            };
        }
    }
}
