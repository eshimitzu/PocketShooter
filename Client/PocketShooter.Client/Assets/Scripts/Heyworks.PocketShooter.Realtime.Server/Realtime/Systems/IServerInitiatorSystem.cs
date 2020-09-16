using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    /// <summary>
    /// System executed for the whole game started by initiator.
    /// </summary>
    public interface IServerInitiatorSystem : ISystem
    {
         bool Execute(ServerPlayer initiator, IServerGame game);
    }
}