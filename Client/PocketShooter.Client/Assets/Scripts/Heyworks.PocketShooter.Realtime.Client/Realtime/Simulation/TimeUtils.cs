using System;
using Microsoft.Extensions.Logging;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represent utilities methods for calculating ticks offset and adjustments.
    /// </summary>
    public static class TimeUtils
    {
        /// <summary>
        /// Calculates the initial client tick offset.
        /// </summary>
        /// <param name="serverDelayMs">Delay from server start in ms.</param>
        /// <param name="tickInterval">The tick interval.</param>
        /// <param name="maxTickerOffsetInTicks">Max initial client offset.</param>
        /// <returns>Initial client offset.</returns>
        public static int CalculateInitialTickersOffset(
            int serverDelayMs,
            int tickInterval,
            int maxTickerOffsetInTicks)
        {
            Assert.IsTrue(tickInterval > 0, $"Invalid {nameof(tickInterval)} parameter.");

            const int minimumInitialTickersOffsetInTicks = 5;
            Assert.IsTrue(maxTickerOffsetInTicks > minimumInitialTickersOffsetInTicks, "max offset is less than max.");

            int initialTickersOffset = serverDelayMs / tickInterval + minimumInitialTickersOffsetInTicks;

            if (initialTickersOffset < minimumInitialTickersOffsetInTicks)
            {
                NetLog.Log.LogWarning("Calculated initial client offset is not in the valid range (< {min}). Using min value.", minimumInitialTickersOffsetInTicks);

                initialTickersOffset = minimumInitialTickersOffsetInTicks;
            }

            if (initialTickersOffset > maxTickerOffsetInTicks)
            {
                NetLog.Log.LogWarning("Calculated initial client offset is not in the valid range (> {max}). Using max value.", maxTickerOffsetInTicks);

                initialTickersOffset = maxTickerOffsetInTicks;
            }

            NetLog.Debug("Calculating initial tickers offset: {initialTickersOffset}, server delay ms: {serverDelayMs}", initialTickersOffset, serverDelayMs);

            return initialTickersOffset;
        }

        /// <summary>
        /// Calculates the client tick offset.
        /// </summary>
        /// <param name="rtt">The RTT.</param>
        /// <param name="tickInterval">The tick interval.</param>
        /// <returns>Initial client offset.</returns>
        public static int CalculateClientTickOffset(
            int rtt,
            int tickInterval)
        {
            Assert.IsTrue(rtt >= 0, $"Invalid {nameof(rtt)} parameter.");
            Assert.IsTrue(tickInterval > 0, $"Invalid {nameof(tickInterval)} parameter.");

            const int minimumInitialClientOffsetInMilliseconds = 30 * 5;
            const int defaultInitialClientOffsetInMilliseconds = 30 * 15;

            int initialClientOffset = (rtt / tickInterval) + minimumInitialClientOffsetInMilliseconds / tickInterval;

            if (initialClientOffset <= 0 || initialClientOffset > 100)
            {
                NetLog.Log.LogWarning("Calculated initial client offset is not in the valid range. Using default value.");

                initialClientOffset = defaultInitialClientOffsetInMilliseconds / tickInterval;
            }

            NetLog.Log.LogTrace("Calculating client tick offset. Offset: {clientOffset}, rtt: {rtt}", initialClientOffset, rtt);

            return initialClientOffset;
        }

        /// <summary>
        /// Adjusts the stopwatch.
        /// </summary>
        /// <param name="rtt">The RTT.</param>
        /// <param name="tickInterval">The tick interval.</param>
        /// <param name="currentBufferSize">Size of the current buffer.</param>
        /// <param name="bufferSizeMean">The buffer mean.</param>
        /// <param name="bufferSizeVariance">The buffer variance.</param>
        /// <param name="clamp">The adjustment will be clamped between -clamp and clamp value range. </param>
        /// <param name="stats">The statistics data: (targetBufferSize, minBufferSize, maxBufferSize).</param>
        /// <returns>
        /// Returns stopwatch adjustment.
        /// </returns>
        public static int CalculateClientStopwatchAdjustment(
            int rtt,
            int tickInterval,
            float currentBufferSize,
            double bufferSizeMean,
            double bufferSizeVariance,
            double clamp,
            out (double, double, double) stats)
        {
            Assert.IsTrue(rtt >= 0, $"Invalid {nameof(rtt)} parameter.");
            Assert.IsTrue(tickInterval > 0, $"Invalid {nameof(tickInterval)} parameter.");

            const double adjustmentSpeed = 0.5;
            const double bufferSizeShift = 1;
            const double defaultBufferSize = 1.5;

            double minBufferSize = bufferSizeShift;
            double maxBufferSize;
            double targetBufferSize;

            if (bufferSizeVariance > 0.001)
            {
                targetBufferSize = (4.0 * bufferSizeVariance) + bufferSizeShift;
                maxBufferSize = (8.0 * bufferSizeVariance) + bufferSizeShift;
            }
            else
            {
                targetBufferSize = defaultBufferSize;
                maxBufferSize = defaultBufferSize + 1;
            }

            stats = (targetBufferSize, minBufferSize, maxBufferSize);

            var rttSize = ((double)rtt / tickInterval) + 2;

            if (currentBufferSize < minBufferSize || currentBufferSize > maxBufferSize)
            {
                var adjustment = adjustmentSpeed * (targetBufferSize - bufferSizeMean) / rttSize * tickInterval;

                NetLog.Trace(
                    "adjusting server buffer. " +
                    "cur: {currentBufferSize}, " +
                    "target: {targetBufferSize:0.00} " +
                    "min: {minBufferSize:0.00} " +
                    "max: {maxBufferSize:0.00} " +
                    "adj: {adjustment:0.000}",
                    currentBufferSize,
                    targetBufferSize,
                    minBufferSize,
                    maxBufferSize,
                    adjustment);

                if (currentBufferSize < 0)
                {
                    NetLog.Log.LogDebug("Negative client command buffer size on server is {currentBufferSize}. Some commands may not be processed.", currentBufferSize);
                }

                return (int)Math.Max(Math.Min(adjustment, clamp), -clamp);
            }

            return 0;
        }

        /// <summary>
        /// Adjusts the stopwatch.
        /// </summary>
        /// <param name="tickInterval">The tick interval.</param>
        /// <param name="currentBufferSize">Size of the current buffer.</param>
        /// <param name="bufferSizeMean">The buffer mean.</param>
        /// <param name="bufferSizeVariance">The buffer variance.</param>
        /// <param name="clamp">The adjustment will be clamped between -clamp and clamp value range. </param>
        /// <param name="stats">The statistics data: (targetBufferSize, minBufferSize, maxBufferSize).</param>
        /// <returns>
        /// Returns stopwatch adjustment.
        /// </returns>
        public static int CalculateWorldStopwatchAdjustment(
            int tickInterval,
            float currentBufferSize,
            double bufferSizeMean,
            double bufferSizeVariance,
            double clamp,
            out (double, double, double) stats)
        {
            Assert.IsTrue(tickInterval > 0, $"Invalid {nameof(tickInterval)} parameter.");

            const double adjustmentSpeed = 1;
            const double bufferSizeShift = 0.1;
            const double defaultBufferSize = 1.0;

            double targetBufferSize;
            double minBufferSize = bufferSizeShift;
            double maxBufferSize;

            if (bufferSizeVariance > 0.001)
            {
                targetBufferSize = (3.0 * bufferSizeVariance) + bufferSizeShift;
                maxBufferSize = (8.0 * bufferSizeVariance) + bufferSizeShift;
            }
            else
            {
                targetBufferSize = defaultBufferSize;
                maxBufferSize = defaultBufferSize + 1;
            }

            stats = (targetBufferSize, minBufferSize, maxBufferSize);

            if (currentBufferSize < minBufferSize || currentBufferSize > maxBufferSize)
            {
                var adjustment = adjustmentSpeed * (bufferSizeMean - targetBufferSize) * tickInterval;

                NetLog.Trace(
                    "adjusting client buffer. " +
                    "cur: {currentBufferSize}, " +
                    "target: {targetBufferSize:0.00} " +
                    "min: {minBufferSize:0.00} " +
                    "max: {maxBufferSize:0.00} " +
                    "adj: {adjustment:0.000}",
                    currentBufferSize,
                    targetBufferSize,
                    minBufferSize,
                    maxBufferSize,
                    adjustment);

                return (int)Math.Max(Math.Min(adjustment, clamp), -clamp);
            }

            return 0;
        }
    }
}