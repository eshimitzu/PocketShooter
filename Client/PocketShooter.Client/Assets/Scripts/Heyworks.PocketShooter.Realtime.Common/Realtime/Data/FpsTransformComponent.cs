using System;
using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Represents position and rotation of the player.
    /// </summary>
    public struct FpsTransformComponent : IEquatable<FpsTransformComponent>, IForAll
    {
        /// <summary>
        /// Creates new player transform from spawn point.
        /// </summary>
        /// <param name="spawnPoint">Spawn point to spawn on.</param>
        public static FpsTransformComponent CreateTransform(SpawnPointInfo spawnPoint) =>
            new FpsTransformComponent((spawnPoint.X, spawnPoint.Y, spawnPoint.Z), spawnPoint.Yaw, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="FpsTransformComponent" /> struct.
        /// </summary>
        /// <param name="pitch">The pitch.</param>
        public FpsTransformComponent(Position position, float yaw, float pitch)
        {
            Position = position;
            Yaw = yaw;
            Pitch = pitch;
        }

        public Position Position;

        //NOTE: UnityFPS has 0  precision for angles, so may optimize

        /// <summary>
        /// Gets the yaw horizontal angle. Rotation.
        /// </summary>
        [Limit(-1f, 360f, 0.1f)] // TODO: UI sends -0.0001 sometimes. Fix it.
        public float Yaw;

        /// <summary>
        /// Gets the pitch vertical angle.
        /// </summary>
        // TODO: a.dezhurko Can be optimize to 0-180. Require InputController change.
        [Limit(-1f, 360f, 0.1f)] // TODO: UI sends -0.0001 sometimes. Fix it.
        public float Pitch;

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(FpsTransformComponent other) =>
             Position.Equals(other.Position) && Yaw.Equals(other.Yaw) && Pitch.Equals(other.Pitch);

        public bool NearEquals(in FpsTransformComponent other) =>
            // TODO: how to connect this to limits? Constants? Generator?
            Position.NearEquals(other.Position)
            && EcsHelpers.NearEquals(Pitch, other.Pitch, 0.1f)
            && EcsHelpers.NearEquals(Yaw, other.Yaw, 0.1f);

        /// <summary>Do not use. Slow. </returns>
        [Obsolete("Do not use. Slow.")]
        public override bool Equals(object obj) => EcsHelpers.ThrowEquals();

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();                
                hashCode = (hashCode * 397) ^ Yaw.GetHashCode();
                hashCode = (hashCode * 397) ^ Pitch.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{nameof(FpsTransformComponent)}{(Position, Yaw, Pitch)}";
    }
}