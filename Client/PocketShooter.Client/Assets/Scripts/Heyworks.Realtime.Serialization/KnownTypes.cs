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
    // known primitive types for fast switch (so we get fast jump table in CPU)
    internal enum KnownTypes : u8
    {
        Unknown,
        Boolean,
        u8,
        i8,
        i32,
        i16,
        i64,
        u16,
        u32,
        u64,
        f32,
        f64,
        IntPtr,
        UIntPtr,
        Decimal,
        Guid
    }
}
