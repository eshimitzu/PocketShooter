using System;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Runnables
{
    /// <summary>
    /// Time dependent entity is used for processes that depends on time.
    /// </summary>
    public abstract class Runnable : IRunnable
    {
        private readonly SimpleTimer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Runnable"/> class.
        /// </summary>
        /// <param name="timer">The timer.</param>
        protected Runnable(SimpleTimer timer)
        {
            this.timer = timer;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IRunnable"/> was started and the remaining time less than or equal to zero.
        /// </summary>
        public bool IsFinished => timer.IsFinished;

        /// <summary>
        /// Gets the progress of timer's countdown, starting from 0 and ending up with 1 (when countdown is finished).
        /// </summary>
        public float Progress => timer.Progress;

        /// <summary>
        /// Gets a remaining time to get finished. Can be negative.
        /// </summary>
        public TimeSpan RemainingTime => timer.RemainingTime;

        /// <summary>
        /// Finishes runnable.
        /// </summary>
        protected abstract void Finish();

        /// <summary>
        /// Finishes the running.
        /// </summary>
        void IRunnable.Finish() => Finish();

        private TimeSpan Duration => timer.Duration;
    }
}