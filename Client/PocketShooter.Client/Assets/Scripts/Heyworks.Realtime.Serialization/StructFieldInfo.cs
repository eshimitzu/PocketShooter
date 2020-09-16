using System;
using System.Reflection;
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
    internal class StructFieldInfo
    {
        public u16 Offset;
        public u16 Size;

        public Type Type;
        public KnownTypes KnownType;

        public FieldInfo Field;

        public Compressions compressions;

        public bool compress;

        public override String ToString() =>
            $"Name {Field.FieldType.Name}, Offset {Offset}, size {Size},  {Type} {KnownType}, isEnum {Field.FieldType.IsEnum}, compress {compress} {compressions}";
    }
}
