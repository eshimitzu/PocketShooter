using System.Collections.Generic;

namespace Heyworks.PocketShooter.Meta.Runnables
{
    /// <summary>
    /// Represents interface for entities that has IRunnable entities.
    /// </summary>
    public interface IHasRunnables
    {
        /// <summary>
        /// Gets the runnables.
        /// </summary>
        IEnumerable<IRunnable> GetRunnables();
    }
}
