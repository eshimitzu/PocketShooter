using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
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
    public partial class TypedBitInStream<TNamespace>
    {
        /// <summary>
        /// Reads true or false.
        /// </summary>
        /// <returns>The value.</returns>
        public bool ReadBool() => Stream.b();

        public i32 ReadInt() => Stream.i32();

        public u8 ReadByte() => Stream.u8();

        public i32 ReadInt(i32 min, i32 max) => Stream.i32(min, max);

        public u8 ReadByte(u8 min, u8 max) => Stream.u8(min, max);

        /// <summary>
        /// Reads limited non negative value.
        /// </summary>
        public u8 ReadByteCount(u8 max) => Stream.u8(0, max);

        public i32 ReadIntCount(i32 max) => Stream.i32(0, max);

        public i16 ReadShort(i16 min, i16 max) => Stream.i16(min, max);

        // TODO: Use TNamespace in Limit tag instead of compress paramter
        private void ReadPrimitive(ref u8 a, StructFieldInfo field)
        {
            if (field.compress)
            {
                switch (field.KnownType)
                {
                    case KnownTypes.u8:
                        ref var u8Value = ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset));
                        u8Value = Stream.u8(field.compressions.u8.MinValue, field.compressions.u8.MaxValue);
                        break;
                    case KnownTypes.i8:
                        ref var i8Value = ref Unsafe.As<u8, i8>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        i8Value = Stream.i8(field.compressions.sbyteMin, field.compressions.sbyteMax);
                        break;
                    case KnownTypes.i16:
                        ref var i16Value = ref Unsafe.As<u8, i16>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        i16Value = Stream.i16(field.compressions.i16.MinValue, field.compressions.i16.MaxValue);
                        break;
                    case KnownTypes.u16:
                        ref var u16Value = ref Unsafe.As<u8, u16>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        u16Value = Stream.u16(field.compressions.u16.MinValue, field.compressions.u16.MaxValue);
                        break;
                    case KnownTypes.i32:
                        ref var int32Value = ref Unsafe.As<u8, i32>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        int32Value = Stream.i32(field.compressions.i32.MinValue, field.compressions.i32.MaxValue);
                        break;
                    case KnownTypes.u32:
                        ref var u32Value = ref Unsafe.As<u8, u32>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        u32Value = Stream.u32(field.compressions.uint32Min, field.compressions.uint32Max);
                        break;
                    case KnownTypes.f32:
                        ref var f32Value = ref Unsafe.As<u8, f32>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        f32Value = Stream.f32(field.compressions.singleMin, field.compressions.singleMax, field.compressions.singlePrecision);
                        break;
                    default:
                        throw new NotImplementedException($"{field.Type} is not registered as primitive");
                }
            }
            else
            {
                switch (field.KnownType)
                {
                    case KnownTypes.u8:
                        ref var u8Value = ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset));
                        u8Value = Stream.u8();
                        break;
                    case KnownTypes.i8:
                        ref var i8Value = ref Unsafe.As<u8, i8>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        i8Value = Stream.i8();
                        break;
                    case KnownTypes.i16:
                        ref var i16Value = ref Unsafe.As<u8, i16>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        i16Value = Stream.i16();
                        break;
                    case KnownTypes.u16:
                        ref var u16Value = ref Unsafe.As<u8, u16>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        u16Value = Stream.u16();
                        break;
                    case KnownTypes.i32:
                        ref var i32Value = ref Unsafe.As<u8, i32>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        i32Value = Stream.i32();
                        break;
                    case KnownTypes.u32:
                        ref var u32Value = ref Unsafe.As<u8, u32>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        u32Value = Stream.u32();
                        break;
                    case KnownTypes.f32:
                        ref var f32Value = ref Unsafe.As<u8, f32>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        f32Value = Stream.f32();
                        break;
                    case KnownTypes.f64:
                        ref var f64Value = ref Unsafe.As<u8, f64>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        f64Value = Stream.f64();
                        break;
                    case KnownTypes.Boolean:
                        ref var boolValue = ref Unsafe.As<u8, Boolean>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        boolValue = Stream.b();
                        break;
                    case KnownTypes.i64:
                        ref var i64Value = ref Unsafe.As<u8, i64>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        i64Value = Stream.i64();
                        break;
                    case KnownTypes.u64:
                        ref var u64Value = ref Unsafe.As<u8, u64>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        u64Value = Stream.u64();
                        break;
                    default:
                        throw new NotImplementedException($"{field.Type} is not registered as primitive");
                }
            }
        }
    }
}
