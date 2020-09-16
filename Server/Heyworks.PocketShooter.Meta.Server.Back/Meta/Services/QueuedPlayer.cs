using System;

namespace Heyworks.PocketShooter.Meta.Services
{
    public class QueuedPlayer
    {
        public QueuedPlayer(PlayerId player, DateTime requestedAt, int level)
        {
            this.Player = player;
            this.RequestedAt = requestedAt;
            this.Level = level;
        }

        public PlayerId Player { get; set; }
        public DateTime RequestedAt { get; set; }

        public int Level { get; set; }
    }
}
