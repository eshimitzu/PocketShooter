namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents class for calculating variance and mean of the time sequence.
    /// </summary>
    public class Telemetry
    {
        private const double MeanSensitivity = 8;
        private const double VarianceSensitivity = 16;

        private bool isInitialized;

        /// <summary>
        /// Gets the variance.
        /// </summary>
        public double Variance { get; private set; }

        /// <summary>
        /// Gets the mean.
        /// </summary>
        public double Mean { get; private set; }

        /// <summary>
        /// Updates the telemetry with new value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Update(double value)
        {
            if (!isInitialized)
            {
                Mean = value;
                isInitialized = true;
            }

            Mean = Mean + ((value - Mean) / MeanSensitivity);

            Variance = Variance - (Variance / VarianceSensitivity);

            if (value >= Mean)
            {
                Variance = Variance + ((value - Mean) / VarianceSensitivity);
            }
            else
            {
                Variance = Variance - ((value - Mean) / VarianceSensitivity);
            }
        }

        /// <summary>
        /// Resets telemetry data by setting values to zero.
        /// </summary>
        public void Reset()
        {
            isInitialized = false;
            Mean = 0;
            Variance = 0;
        }
    }
}