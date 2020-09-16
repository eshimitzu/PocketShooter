using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    /// <summary>
    /// Systems which runs on server and client on the common owned player entity.
    /// </summary>
    public interface IOwnerSystem : ISystem
    {
         bool Execute(OwnedPlayer initiator);
    }
}