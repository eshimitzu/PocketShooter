using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public static class OwnerSystemExtensions
    {
        public static void Execute(this OwnerSystem system, IServerGame game)
        {
            foreach (var player in game.Players.Values)
            {
                system.Execute(player);
            }
        }
    }
}

