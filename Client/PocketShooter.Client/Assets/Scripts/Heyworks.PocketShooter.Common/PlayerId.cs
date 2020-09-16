using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Heyworks.PocketShooter
{
    /// <summary>
    /// Represents a unique identifier shared across meta, client and realtime.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct PlayerId : IEquatable<PlayerId>
    {
        private readonly Guid value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PlayerId(Guid value)
        {
            if (Guid.Empty.Equals(value)) Throw.InvalidOperation("Must be non empty id");
            this.value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator PlayerId(Guid value) => new PlayerId(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Guid(PlayerId playerId) => playerId.value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in PlayerId a, in PlayerId b) => a.value == b.value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in PlayerId a, in PlayerId b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(PlayerId other) => value == other.value;

        /// <inheritdoc/>
        public override int GetHashCode() => value.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => value.ToString();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is PlayerId id && value == id.value;
    }
}
