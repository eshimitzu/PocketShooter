using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public abstract class ServerInitiatorSystem
    {
        public void Execute(IServerGame game)
        {
            foreach (ServerPlayer player in game.Players.Values)
            {
                Execute(player, game);
            }
        }

        protected abstract void Execute(ServerPlayer initiator, IServerGame game);
    }
}