using Heyworks.PocketShooter.Meta.Data;
using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents a simple timer.
    /// </summary>
    public class SimpleTimer : ITimer
    {
        private readonly IDateTimeProvider dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTimer"/> class.
        /// </summary>
        /// <param name="state">The timer state.</param>
        /// <param name="dateTimeProvider">The date time provider.</param>
        public SimpleTimer(SimpleTimerState state, IDateTimeProvider dateTimeProvider)
        {
            if (state.StartTime == DateTime.MinValue)
            {
                throw new ArgumentException("The value must be greater than min value.", nameof(state.StartTime));
            }

            if (state.Duration < TimeSpan.Zero)
            {
                throw new ArgumentException("The value can't be negative.", nameof(state.Duration));
            }

            StartTime = state.StartTime;
            Duration = state.Duration;
            FinishTime = StartTime.Add(Duration);

            this.dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTimer"/> class.
        /// </summary>
        /// <param name="duration">The timer duration.</param>
        /// <param name="dateTimeProvider">The date time provider.</param>
        public SimpleTimer(TimeSpan duration, IDateTimeProvider dateTimeProvider)
            : this(new SimpleTimerState { StartTime = dateTimeProvider.UtcNow, Duration = duration }, dateTimeProvider)
        {
        }

        /// <summary>
        /// Gets a timer start time.
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// Gets a timer total duration to get finished.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Gets a remaining time to get finished. Can be negative.
        /// </summary>
        public TimeSpan RemainingTime => FinishTime - dateTimeProvider.UtcNow;

        /// <summary>
        /// Gets a value indicating whether the timer is finished.
        /// </summary>
        public bool IsFinished => RemainingTime <= TimeSpan.Zero;

        /// <summary>
        /// Gets the progress of timer's countdown, starting from 0 and ending up with 1 (when countdown is finished).
        /// </summary>
        public float Progress => Math.Min(1.0f - (float)(RemainingTime.TotalMilliseconds / Duration.TotalMilliseconds), 1.0f);

        /// <summary>
        /// Gets a timer finish time.
        /// </summary>
        private DateTime FinishTime { get; }
    }
}
