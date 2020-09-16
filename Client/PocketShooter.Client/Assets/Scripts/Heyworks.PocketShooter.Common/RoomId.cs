using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Heyworks.PocketShooter
{
    /// <summary>
    /// Represents a unique identifier shared accross meta, client and realtime..
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct RoomId : IEquatable<RoomId>
    {
        private readonly Guid value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RoomId(Guid value)
        {
            if (Guid.Empty.Equals(value)) Throw.InvalidOperation("Must be non empty id");
            this.value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RoomId(Guid value) => new RoomId(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Guid(RoomId roomId) => roomId.value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in RoomId a, in RoomId b) => a.value == b.value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in RoomId a, in RoomId b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(RoomId other) => value == other.value;

        /// <inheritdoc/>
        public override int GetHashCode() => value.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => value.ToString("N");

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RoomId id && value == id.value;
    }
}
