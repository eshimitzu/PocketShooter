using System;
using i8 = System.SByte;
using i16 = System.Int16;
using i32 = System.Int32;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using f32 = System.Single;
using f64 = System.Double;

namespace Heyworks.Realtime.Serialization
{
    public interface ILimit<out T>
        where T : unmanaged, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable // number
    {
        T MinValue { get; }
        T MaxValue { get; }
    }

    public readonly struct i32Limit : ILimit<i32>, IEquatable<i32Limit>
    {
        public i32Limit(i32 min, i32 max)
        {
            if (min >= max) throw new ArgumentException();
            MinValue = min;
            MaxValue = max;
        }

        public i32 MinValue { get; }
        public i32 MaxValue { get; }
        public override string ToString() => (MinValue, MaxValue).ToString();
        public bool Equals(i32Limit other) => MinValue == other.MinValue && MaxValue == other.MaxValue;
    }

    public readonly struct u8Limit : ILimit<u8>, IEquatable<u8Limit>
    {
        public u8Limit(u8 min, u8 max)
        {
            if (min >= max) throw new ArgumentException();
            MinValue = min;
            MaxValue = max;
        }

        public u8 MinValue { get; }
        public u8 MaxValue { get; }
        public bool Equals(u8Limit other) => MinValue == other.MinValue && MaxValue == other.MaxValue;
        public override string ToString() => (MinValue, MaxValue).ToString();
    }

    public readonly struct u16Limit : ILimit<u16>, IEquatable<u16Limit>
    {
        public u16Limit(u16 min, u16 max)
        {
            if (min >= max) throw new ArgumentException();
            MinValue = min;
            MaxValue = max;
        }

        public u16 MinValue { get; }

        public u16 MaxValue { get; }
        public override string ToString() => (MinValue, MaxValue).ToString();

        public bool Equals(u16Limit other) => MinValue == other.MinValue && MaxValue == other.MaxValue;
    }

    public readonly struct i16Limit : ILimit<i16>, IEquatable<i16Limit>
    {

        public i16Limit(i16 min, i16 max)
        {
            if (min >= max) throw new ArgumentException();
            MinValue = min;
            MaxValue = max;
        }

        public i16 MinValue { get; }

        public i16 MaxValue { get; }

        public override string ToString() => (MinValue, MaxValue).ToString();

        public bool Equals(i16Limit other) => MinValue == other.MinValue && MaxValue == other.MaxValue;
    }
}