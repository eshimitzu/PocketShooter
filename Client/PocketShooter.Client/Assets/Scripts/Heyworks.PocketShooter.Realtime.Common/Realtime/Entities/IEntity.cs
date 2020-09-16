using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Marker interface implemented by classes which do not store data but use view onto state.
    /// </summary>
    public interface IEntity<TId>
        where TId : struct
    {
        TId Id { get; }
    }
}