using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

#pragma warning disable SA1121 // Use built-in type alias
using primitive = System.Int16;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Non negative custom entity identifier with ability to limit and replace underlying number.
    /// </summary>
    [DebuggerDisplay("{value}")]
    public readonly struct EntityId : IEquatable<EntityId>
    {
        private readonly primitive value;

        public const primitive MaxValue = primitive.MaxValue;

        public const primitive MinValue = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityId(primitive value)
        {
            if (value < 0)
            {
                Throw.Argument($"{nameof(value)} should be non negative", nameof(value));
            }

            this.value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityId(byte value) => this.value = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator EntityId(primitive value) => new EntityId(value);

        // do not expect id to be larger than in ever
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(EntityId value) => value.value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator EntityId(byte value) => new EntityId(value);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in EntityId a, in EntityId b) => a.value == b.value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in EntityId a, in EntityId b) => a.value != b.value;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int operator %(in EntityId a, in int mod) => a.value % mod;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EntityId operator +(in EntityId a, in EntityId b)
        {
            var result = a.value + b.value;
            if (result > primitive.MaxValue)
            {
                Throw.InvalidOperation($"{result} cannot create identifier larger than {primitive.MaxValue})");
            }

            var casted = (primitive)result;
            return Unsafe.As<primitive, EntityId>(ref casted);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EntityId operator +(in EntityId a, in byte b)
        {
            var result = a.value + b;
            if (result > primitive.MaxValue)
            {
                Throw.InvalidOperation($"{result} cannot create identifier larger than {primitive.MaxValue})");
            }

            var casted = (primitive)result;
            return Unsafe.As<primitive, EntityId>(ref casted);
        }

        /// <inheritdoc/>
        public bool Equals(EntityId other) => value == other.value;

        /// <inheritdoc/>
        public override int GetHashCode() => value.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => value.ToString();
    }
}
