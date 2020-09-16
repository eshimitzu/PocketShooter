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
    public readonly struct BotId : IEquatable<BotId>
    {
        private readonly Guid value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BotId(Guid value)
        {
            if (Guid.Empty.Equals(value)) Throw.InvalidOperation("Must be non empty id");
            this.value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator BotId(Guid value) => new BotId(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Guid(BotId botId) => botId.value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in BotId a, in BotId b) => a.value == b.value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in BotId a, in BotId b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(BotId other) => value == other.value;

        /// <inheritdoc/>
        public override int GetHashCode() => value.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => value.ToString("N");

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is BotId id && value == id.value;
    }
}
