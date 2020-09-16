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
    public struct Compressions
    {
        internal f32 singleMin;
        internal f32 singleMax;
        internal f32 singlePrecision;

        internal f64 doubleMin;
        internal f64 doubleMax;
        internal f64 doublePrecision;

        internal u8Limit u8;

        internal i8 sbyteMin;
        internal i8 sbyteMax;

        internal i16Limit i16;

        internal u16Limit u16;

        internal i32Limit i32;

        internal u32 uint32Min;
        internal u32 uint32Max;

        internal i64 int64Min;
        internal i64 int64Max;

        internal u64 uint64Min;
        internal u64 uint64Max;

        // collection length/count limit
        internal u32 length;

        public override System.String ToString() =>
            !u8.Equals(default) ? u8.ToString() :
            !i16.Equals(default) ? i16.ToString() :
            !u16.Equals(default) ? u16.ToString() :
            !i32.Equals(default) ? i32.ToString()
            : "()";
    }
}