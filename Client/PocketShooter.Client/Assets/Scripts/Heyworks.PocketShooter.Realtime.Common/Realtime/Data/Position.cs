using System;
using System.Runtime.CompilerServices;
using Heyworks.Realtime.Serialization;
using System.Numerics;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public struct Position : IEquatable<Position>
    {
        //NOTE: UnityFPS has 2 precision for X/Y/Z and 0 for angles, so may optimize
        /// <summary>
        /// Gets the x.
        /// </summary>        
        [Limit(-100f, 100f, 0.001f)]
        public float X;

        /// <summary>
        /// Gets the y.
        /// </summary>
        [Limit(-3f, 13f, 0.001f)] // TODO: create limit per map id
        public float Y;

        /// <summary>
        /// Gets the z.
        /// </summary>
        [Limit(-100f, 100f, 0.001f)]
        public float Z;

        public Position(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // TODO: use Vector3 field with offset 0 with [IgnoreForSerialization] for instan cast with no copy
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UnityEngine.Vector3(Position value) => new UnityEngine.Vector3(value.X, value.Y, value.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Position(UnityEngine.Vector3 value) => new Position(value.x, value.y, value.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3(Position value) => new Vector3(value.X, value.Y, value.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Position((float x, float y, float z) value) => new Position(value.x, value.y, value.z);

        public override string ToString() => ((Vector3)this).ToString();
        public override int GetHashCode() => ((Vector3)this).GetHashCode();
        public bool Equals(Position other) => ((Vector3)this).Equals((Vector3)other);

        public static Position operator +(Position self, int value) => new Position(self.X + value, self.Y + value, self.Z + value);

        /// <summary>Do not use. Slow. </returns>
        [Obsolete("Do not use. Slow.")]
        public override bool Equals(object obj) => EcsHelpers.ThrowEquals();

        public bool NearEquals(in Position other) =>
            // TODO: how to connect this to limits? Constants? Generator?
            EcsHelpers.NearEquals(X, other.X, 0.01f)
            && EcsHelpers.NearEquals(Y, other.Y, 0.01f)
            && EcsHelpers.NearEquals(Z, other.Z, 0.01f);
    }
}