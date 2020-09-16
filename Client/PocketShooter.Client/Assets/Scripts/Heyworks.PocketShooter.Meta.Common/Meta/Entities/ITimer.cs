using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public interface ITimer
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="ITimer"/> was started and the remaining time less than or equal to zero.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Gets a remaining time to get finished.
        /// </summary>
        TimeSpan RemainingTime { get; }

        /// <summary>
        /// Gets the timer progress.
        /// </summary>
        float Progress { get; }
    }
}
