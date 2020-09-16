using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    /// <summary>
    /// Can contain common owned systems logic, including through extensions.
    /// </summary>
    public abstract class OwnerSystem : IOwnerSystem
    {
        public abstract bool Execute(OwnedPlayer player);
    }
}