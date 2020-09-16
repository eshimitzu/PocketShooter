using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class SimpleTimerState
    {
        /// <summary>
        /// Gets or sets a timer start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets a timer total duration to get completed.
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}
