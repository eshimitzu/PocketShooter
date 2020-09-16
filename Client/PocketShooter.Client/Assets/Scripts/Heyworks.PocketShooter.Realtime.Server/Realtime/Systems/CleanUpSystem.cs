using System;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    /// <summary>
    /// We store all resources up until next buffer cycle. So need to clean up them before use.
    /// </summary>
    // NOTE: can make clean up in parrallel in background
    internal class CleanUpSystem : IServerGameSystem
    {
        public CleanUpSystem()
        {
        }

        public bool Execute(IServerGame game)
        {
            foreach (var player in game.Players.Values)
            {
                player.Shots?.Clear();
                player.Damages?.Clear();
                player.Heals?.Clear();
            }

            return true;
        }
    }
}