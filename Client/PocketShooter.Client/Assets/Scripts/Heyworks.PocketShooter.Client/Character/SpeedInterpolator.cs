using System.Text;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using ModestTree;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Interpolates player transforms using speed for smoother movement.
    /// </summary>
    public class SpeedInterpolator
    {
        private const float MaxLerpDistance = 10f;
        private const float MaxAcceleration = 1.33f;
        private const float MaxSkillAcceleration = 5f;
        // todo pass trooper speed;
        private const float MaxVelocityLinear = 5;

        private readonly ITickProvider tickProvider;
        private readonly IRealtimeConfiguration realtimeConfiguration;
        private readonly IRemotePlayer model;

        private Vector3 currentPosition;
        private Vector3 lastPosition;
        private Vector3 nextPosition;
        private Quaternion lastRotation;
        private Quaternion nextRotation;
        private float lastPitch;
        private float nextPitch;

        private int lastTick;
        private int tickUpdated;
        private float currentVelocityLinear;

        private float MaxVelocity => MaxVelocityLinear * (SpeedSkillActive ? MaxSkillAcceleration : MaxAcceleration);

        public SpeedInterpolator(ITickProvider tickProvider, IRealtimeConfiguration realtimeConfiguration, IRemotePlayer model)
        {
            Assert.IsNotNull(tickProvider);

            this.tickProvider = tickProvider;
            this.realtimeConfiguration = realtimeConfiguration;
            this.model = model;

            currentPosition = model.Transform.Position;
            lastPosition = currentPosition;
            nextPosition = currentPosition;
            lastRotation = Quaternion.Euler(0, model.Transform.Yaw, 0);
            nextRotation = lastRotation;
            lastPitch = model.Transform.Pitch;
            nextPitch = lastPitch;
        }

        public void Update(FpsTransformComponent nextTransform)
        {
            lastPosition = nextPosition;
            lastRotation = nextRotation;
            lastPitch = nextPitch;
            nextPosition = nextTransform.Position;
            nextRotation = Quaternion.Euler(0, nextTransform.Yaw, 0);
            nextPitch = nextTransform.Pitch;

            lastTick = tickUpdated;
            tickUpdated = tickProvider.WorldTick;
        }

        public (Vector3 position, Quaternion rotation, float pitch) Interpolate(float deltaTime)
        {
            if (currentPosition == nextPosition
                || !AllowLerp(currentPosition, nextPosition)
                || !realtimeConfiguration.EnableInterpolator)
            {
                currentPosition = lastPosition = nextPosition;
                return (nextPosition, nextRotation, nextPitch);
            }

            float progress = tickProvider.ElapsedWorldTicks(tickUpdated);
            if (progress < 0)
            {
                progress = 1;
                lastPosition = nextPosition;
            }

            Vector3 targetPosition = Vector3.Lerp(lastPosition, nextPosition, progress);
            Vector3 deltaPos = targetPosition - currentPosition;
            Vector3 targetVelocity = deltaPos / deltaTime;
            float targetVelocityLinear = targetVelocity.magnitude;
            currentVelocityLinear = Mathf.Lerp(
                    currentVelocityLinear,
                    Mathf.Min(targetVelocityLinear, MaxVelocity),
                    1 - Mathf.Exp(-100 * deltaTime));
            Vector3 factDPos = targetVelocityLinear == 0 ? deltaPos : deltaPos * (currentVelocityLinear / targetVelocityLinear);

            currentPosition += factDPos;

            var position = currentPosition;
            var pitch = Mathf.Lerp(lastPitch, nextPitch, progress);
            var rotation = Quaternion.Lerp(lastRotation, nextRotation, progress);

//            StringBuilder str = new StringBuilder().Append(lastPosition.x.ToString("F2")).Append(">").Append(currentPosition.x.ToString("F2")).Append(">").Append(nextPosition.x.ToString("F2")).Append(" ").Append($"{tickProvider.WorldTick} {tickUpdated} {tickProvider.WorldTickFraction} ");
//            Debug.Log(str);

            return (position, rotation, pitch);
        }

        public override string ToString()
        {
            return $"[{lastTick}] {lastPosition}, {lastRotation}, {lastPitch} -> [{tickUpdated}] {nextPosition}, {nextRotation}, {nextPitch}";
        }

        private bool AllowLerp(Vector3 a, Vector3 b)
        {
            return a.x - b.x < MaxLerpDistance && b.x - a.x < MaxLerpDistance &&
                   a.y - b.y < MaxLerpDistance && b.y - a.y < MaxLerpDistance &&
                   a.z - b.z < MaxLerpDistance && b.z - a.z < MaxLerpDistance;
        }

        // todo add sprint effect
        private bool SpeedSkillActive => model.Effects.Dash.IsDashing || model.Effects.Jump.IsJumping;
    }
}