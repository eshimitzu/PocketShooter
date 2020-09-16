using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    // tick for events
    [DebuggerDisplay("{value}")]
    public readonly struct Tick : IEquatable<Tick>, IComparable<Tick>
    {
        private readonly int value;

        public Tick(int value)
        {
            if (value < 0) Throw.Argument("Tick must be zero or positive", nameof(value));
            this.value = value;
        }

        public int CompareTo(Tick other) => value - other.value;

        public bool Equals(Tick other) => value == other.value;
        public override int GetHashCode() => value.GetHashCode();
        public override string ToString() => value.ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Tick(int value) => new Tick(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(Tick value) => value.value;
    }
}
