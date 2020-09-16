using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Runnables
{
    /// <summary>
    /// Represents interface for scheduled entity.
    /// </summary>
    public interface IRunnable : ITimer
    {
        /// <summary>
        /// Finishes the running.
        /// </summary>
        void Finish();
    }
}
