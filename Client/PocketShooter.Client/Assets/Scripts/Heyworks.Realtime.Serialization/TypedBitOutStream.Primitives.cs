using System;
using System.Runtime.CompilerServices;
using NetStack.Serialization;
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
    public partial class TypedBitOutStream<TNamespace>
    {
        public void WriteInt(i32 value, i32 min, i32 max) => Stream.i32(value, min, max);

        /// <summary>
        /// Writes limited non negative value.
        /// </summary>
        public void WriteByteCount(u8 value, u8 max) => Stream.u8(value, 0, max);

        public void WriteIntCount(i32 value, i32 max) => Stream.i32(value, 0, max);

        public void WriteByte(u8 value, u8 min, u8 max) => Stream.u8(value, min, max);

        public void WriteShort(i16 value, i16 min, i16 max) => Stream.i16(value, min, max);

        public void WriteByte(u8 value) => Stream.u8(value);

        public void WriteInt(i32 value) => Stream.i32(value);

        public void WriteBool(bool value) => Stream.b(value);

        public void WriteShort(i16 value) => Stream.i16(value);

        public void WriteDiff(i16 baseline, i16 update) => Stream.i16BDiff(baseline, update);

        public void WriteDiff(i16 baseline, i16 update, i16 min, i16 max) => Stream.i16BDiff(baseline, update, min, max);

        public void WriteDiff(f32 baseline, f32 update) => Stream.f32BDiff(baseline, update);

        public void WriteDiff(f64 baseline, f64 update) => Stream.f64BDiff(baseline, update);

        public void WriteDiff(u64 baseline, u64 update) => Stream.u64BDiff(baseline, update);

        public void WriteDiff(i8 baseline, i8 update) => Stream.i8BDiff(baseline, update);

        public void WriteDiff(u16 baseline, u16 update) => Stream.u16BDiff(baseline, update);

        public void WriteDiff(i64 baseline, i64 update) => Stream.i64BDiff(baseline, update);

        public void WriteDiff(u32 baseline, u32 update) => Stream.u32BDiff(baseline, update);

        public void WriteDiff(u8 baseline, u8 update) => Stream.u8BDiff(baseline, update);

        public void WriteDiff(i32 baseline, i32 update) => Stream.i32BDiff(baseline, update);

        private void WritePrimitive<T>(StructFieldInfo fieldType, ref T valueRef)
            where T : unmanaged
        {
            if (fieldType.compress)
            {
                switch (fieldType.KnownType)
                {
                    case KnownTypes.i32:
                        Stream.i32(Unsafe.As<T, i32>(ref valueRef), fieldType.compressions.i32.MinValue, fieldType.compressions.i32.MaxValue);
                        break;
                    case KnownTypes.u32:
                        Stream.u32(Unsafe.As<T, u32>(ref valueRef), fieldType.compressions.uint32Min, fieldType.compressions.uint32Max);
                        break;
                    case KnownTypes.u8:
                        Stream.u8(Unsafe.As<T, u8>(ref valueRef), fieldType.compressions.u8.MinValue, fieldType.compressions.u8.MaxValue);
                        break;
                    case KnownTypes.f32:
                        Stream.f32(Unsafe.As<T, f32>(ref valueRef), fieldType.compressions.singleMin, fieldType.compressions.singleMax, fieldType.compressions.singlePrecision);
                        break;
                    case KnownTypes.i16:
                        Stream.i16(Unsafe.As<T, i16>(ref valueRef), fieldType.compressions.i16.MinValue, fieldType.compressions.i16.MaxValue);
                        break;
                    case KnownTypes.u16:
                        Stream.u16(Unsafe.As<T, u16>(ref valueRef), fieldType.compressions.u16.MinValue, fieldType.compressions.u16.MaxValue);
                        break;
                    case KnownTypes.i8:
                        Stream.i8(Unsafe.As<T, i8>(ref valueRef), fieldType.compressions.sbyteMin, fieldType.compressions.sbyteMax);
                        break;
                    default:
                        throw new NotImplementedException($"{fieldType.Type} is not registered as primitive");
                }
            }
            else
            {
                switch (fieldType.KnownType)
                {
                    case KnownTypes.i32:
                        Stream.i32(Unsafe.As<T, i32>(ref valueRef));
                        break;
                    case KnownTypes.u32:
                        Stream.u32(Unsafe.As<T, u32>(ref valueRef));
                        break;
                    case KnownTypes.u8:
                        Stream.u8(Unsafe.As<T, u8>(ref valueRef));
                        break;
                    case KnownTypes.f32:
                        Stream.f32(Unsafe.As<T, f32>(ref valueRef));
                        break;
                    case KnownTypes.i16:
                        Stream.i16(Unsafe.As<T, i16>(ref valueRef));
                        break;
                    case KnownTypes.u16:
                        Stream.u16(Unsafe.As<T, u16>(ref valueRef));
                        break;
                    case KnownTypes.i8:
                        Stream.i8(Unsafe.As<T, i8>(ref valueRef));
                        break;
                    case KnownTypes.u64:
                        Stream.u64(Unsafe.As<T, u64>(ref valueRef));
                        break;
                    case KnownTypes.i64:
                        Stream.i64(Unsafe.As<T, i64>(ref valueRef));
                        break;
                    case KnownTypes.f64:
                        Stream.f64(Unsafe.As<T, f64>(ref valueRef));
                        break;
                    case KnownTypes.Boolean:
                        Stream.b(Unsafe.As<T, Boolean>(ref valueRef));
                        break;
                    default:
                        throw new NotImplementedException($"{fieldType.Type} is not registered as primitive");
                }
            }
        }

        private void DiffPrimitive<T>(StructFieldInfo fieldType, ref T baseline, ref T update)
            where T : unmanaged
        {
            if (fieldType.compress)
            {
                var compress = fieldType.compressions;
                switch (fieldType.KnownType)
                {
                    case KnownTypes.Boolean:
                        Stream.b(Unsafe.As<T, Boolean>(ref update));
                        break;
                    case KnownTypes.i32:
                        Stream.i32BDiff(Unsafe.As<T, i32>(ref baseline), Unsafe.As<T, i32>(ref update), compress.i32.MinValue, compress.i32.MaxValue);
                        break;
                    case KnownTypes.f32:
                        Stream.f32BDiff(Unsafe.As<T, f32>(ref baseline), Unsafe.As<T, f32>(ref update), compress.singleMin, compress.singleMax, compress.singlePrecision);
                        break;
                    case KnownTypes.u32:
                        Stream.u32BDiff(Unsafe.As<T, u32>(ref baseline), Unsafe.As<T, u32>(ref update), compress.uint32Min, compress.uint32Max);
                        break;
                    case KnownTypes.u16:
                        Stream.u16BDiff(Unsafe.As<T, u16>(ref baseline), Unsafe.As<T, u16>(ref update), compress.u16.MinValue, compress.u16.MaxValue);
                        break;
                    case KnownTypes.i16:
                        Stream.i16BDiff(Unsafe.As<T, i16>(ref baseline), Unsafe.As<T, i16>(ref update), compress.i16.MinValue, compress.i16.MaxValue);
                        break;
                    case KnownTypes.u8:
                        Stream.u8BDiff(Unsafe.As<T, u8>(ref baseline), Unsafe.As<T, u8>(ref update), compress.u8.MinValue, compress.u8.MaxValue);
                        break;
                    case KnownTypes.i8:
                        Stream.i8BDiff(Unsafe.As<T, i8>(ref baseline), Unsafe.As<T, i8>(ref update), compress.sbyteMin, compress.sbyteMax);
                        break;
                    default:
                        throw new NotImplementedException($"{fieldType.KnownType}");
                }
            }
            else
            {
                switch (fieldType.KnownType)
                {
                    case KnownTypes.Boolean:
                        Stream.b(Unsafe.As<T, bool>(ref update));
                        break;
                    case KnownTypes.i32:
                        WriteDiff(Unsafe.As<T, i32>(ref baseline), Unsafe.As<T, i32>(ref update));
                        break;
                    case KnownTypes.i64:
                        WriteDiff(Unsafe.As<T, i64>(ref baseline), Unsafe.As<T, i64>(ref update));
                        break;
                    case KnownTypes.f32:
                        WriteDiff(Unsafe.As<T, f32>(ref baseline), Unsafe.As<T, f32>(ref update));
                        break;
                    case KnownTypes.u32:
                        WriteDiff(Unsafe.As<T, u32>(ref baseline), Unsafe.As<T, u32>(ref update));
                        break;
                    case KnownTypes.f64:
                        WriteDiff(Unsafe.As<T, f64>(ref baseline), Unsafe.As<T, f64>(ref update));
                        break;
                    case KnownTypes.u16:
                        WriteDiff(Unsafe.As<T, u16>(ref baseline), Unsafe.As<T, u16>(ref update));
                        break;
                    case KnownTypes.u64:
                        WriteDiff(Unsafe.As<T, u64>(ref baseline), Unsafe.As<T, u64>(ref update));
                        break;
                    case KnownTypes.i16:
                        WriteDiff(Unsafe.As<T, i16>(ref baseline), Unsafe.As<T, i16>(ref update));
                        break;
                    case KnownTypes.u8:
                        WriteDiff(Unsafe.As<T, u8>(ref baseline), Unsafe.As<T, u8>(ref update));
                        break;
                    case KnownTypes.i8:
                        WriteDiff(Unsafe.As<T, i8>(ref baseline), Unsafe.As<T, i8>(ref update));
                        break;
                    default:
                        throw new NotImplementedException($"{fieldType.KnownType}");
                }
            }
        }
    }
}
