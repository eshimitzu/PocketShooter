using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using Microsoft.Extensions.Logging;
using ModestTree;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Interpolates player transforms between ticks for smoother movement.
    /// </summary>
    public struct Interpolator
    {
        // TODO: a.dezhurko Replace with speed.
        private const float MaxLerpDistance = 10f;

        private readonly Vector3 lastPosition;
        private readonly Vector3 nextPosition;
        private readonly Quaternion lastRotation;
        private readonly Quaternion nextRotation;
        private readonly float lastPitch;
        private readonly float nextPitch;
        private readonly int lastTick;
        private readonly int tickCreated;

        private readonly ITickProvider tickProvider;
        private readonly IRealtimeConfiguration realtimeConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interpolator"/> struct.
        /// Creates interpolator for next transform from previous one.
        /// </summary>
        /// <param name="lastInterpolator">The interpolator for previous tick transform.</param>
        /// <param name="nextTransform">The player transform from next tick.</param>
        /// <param name="tickProvider">The tick-related data provider.</param>
        /// <param name="realtimeConfiguration">The realtime parameters.</param>
        public Interpolator(Interpolator lastInterpolator, FpsTransformComponent nextTransform, ITickProvider tickProvider, IRealtimeConfiguration realtimeConfiguration)
        {
            Assert.IsNotNull(tickProvider);

            lastPosition = lastInterpolator.nextPosition;
            nextPosition = nextTransform.Position;

            lastRotation = lastInterpolator.nextRotation;
            nextRotation = Quaternion.Euler(0, nextTransform.Yaw, 0);

            lastPitch = lastInterpolator.nextPitch;
            nextPitch = nextTransform.Pitch;

            lastTick = lastInterpolator.tickCreated;
            tickCreated = tickProvider.WorldTick;

            this.tickProvider = tickProvider;
            this.realtimeConfiguration = realtimeConfiguration;

            SimulationLog.Trace("From interpolator: {Values}.", this);
        }

        /// <summary>
        /// Interpolates between last and next ticks transforms.
        /// </summary>
        /// <returns>Interpolated transform.</returns>
        public (Vector3 position, Quaternion rotation, float pitch) Interpolate()
        {
            if (!realtimeConfiguration.EnableInterpolator)
            {
                return (nextPosition, nextRotation, nextPitch);
            }

            var progress = tickProvider.WorldTickFraction;
            if (progress >= 1 || tickProvider.WorldTick != tickCreated || !AllowLerp(lastPosition, nextPosition))
            {
                return (nextPosition, nextRotation, nextPitch);
            }

            var position = Vector3.Lerp(lastPosition, nextPosition, progress);
            var pitch = Mathf.Lerp(lastPitch, nextPitch, progress);
            var rotation = Quaternion.Lerp(lastRotation, nextRotation, progress);

            return (position, rotation, pitch);
        }

        /// <summary>
        /// Returns a <see cref="System.string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return $"[{lastTick}] {lastPosition}, {lastRotation}, {lastPitch} -> [{tickCreated}] {nextPosition}, {nextRotation}, {nextPitch}";
        }

        private bool AllowLerp(Vector3 a, Vector3 b)
        {
            return a.x - b.x < MaxLerpDistance && b.x - a.x < MaxLerpDistance &&
                   a.y - b.y < MaxLerpDistance && b.y - a.y < MaxLerpDistance &&
                   a.z - b.z < MaxLerpDistance && b.z - a.z < MaxLerpDistance;
        }
    }
}