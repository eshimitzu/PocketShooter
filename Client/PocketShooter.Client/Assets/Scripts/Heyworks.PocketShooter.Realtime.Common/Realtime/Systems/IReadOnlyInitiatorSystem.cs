using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public interface IReadOnlyInitiatorSystem : ISystem
    {
         bool Execute(IOwnedPlayer initiator);
    }
}