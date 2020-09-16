using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    /// <summary>
    /// Systems which runs on server and client on the common owned player entity and needs access to common game state.
    /// </summary>
    public interface IOwnerGameSystem : ISystem
    {
        bool Execute(OwnedPlayer initiator, IGame game);
    }
}