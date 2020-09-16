using System;
using System.Linq;
using ObjectLayoutInspector.Helpers;
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
    // TODO: consider usage of Range
    /// <summary>
    /// Mark field or autoimplemented property to compress.
    /// </summary>
    [AttributeUsageAttribute(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.GenericParameter | AttributeTargets.Struct,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class LimitAttribute : Attribute
    {
        internal Type type;
        internal Compressions compressions;

        /// <summary>
        /// Use for collections of some specific maximal count.
        /// </summary>
        /// <param name="length">Maximal possible count of elements.</param>
        public LimitAttribute(u32 length)
        {
            compressions.length = length;
        }

        /// <summary>
        /// Quantization of floats.
        /// </summary>
        public LimitAttribute(f32 min, f32 max, f32 precision)
        {
            if (min >= max)
            {
                throw new ArgumentException();
            }

            if (precision < 0 || precision >= max - min)
            {
                throw new ArgumentException();
            }

            type = typeof(f32);
            compressions.singleMin = min;
            compressions.singleMax = max;
            compressions.singlePrecision = precision;
        }

        /// <summary>
        /// Primitive like stucts with know limited range.
        /// </summary>
        public LimitAttribute(Type typeOfStruct, i16 min, i16 max)
        {
            if (!typeOfStruct.IsValueType || ReflectionHelper.GetInstanceFields(typeOfStruct).Length != 1)
            {
                throw new ArgumentException("Should be structure with single field");
            }

            compressions.i16 = new i16Limit(min, max);
        }

        // TODO: consider usage of EnumDataType to allow annotation without project coupling
        /// <summary>
        /// Known limited enumerations.
        /// </summary>
        public LimitAttribute(Type typeOfEnum) // <T>(): where T : Enum in C# 8
        {
            if (!typeOfEnum.IsEnum)
            {
                throw new ArgumentException();
            }

            type = typeOfEnum;

            if (typeOfEnum.GetEnumUnderlyingType() == typeof(u8))
            {
                var values = typeOfEnum.GetEnumValues().Cast<u8>().ToArray();
                var min = values.Min();
                var max = values.Max();
                compressions.u8 = new u8Limit(min, max);
            }
            else if (typeOfEnum.GetEnumUnderlyingType() == typeof(u16))
            {
                var values = typeOfEnum.GetEnumValues().Cast<u16>().ToArray();
                var min = values.Min();
                var max = values.Max();
                compressions.u16 = new u16Limit(min, max);
            }
            else if (typeOfEnum.GetEnumUnderlyingType() == typeof(i32))
            {
                var values = typeOfEnum.GetEnumValues().Cast<i32>().ToArray();
                var min = values.Min();
                var max = values.Max();
                compressions.i32 = new i32Limit(min, max);
            }
            else if (typeOfEnum.GetEnumUnderlyingType() == typeof(i16))
            {
                var values = typeOfEnum.GetEnumValues().Cast<i16>().ToArray();
                var min = values.Min();
                var max = values.Max();
                compressions.i16 = new i16Limit(min, max);
            }
            else if (typeOfEnum.GetEnumUnderlyingType() == typeof(u32))
            {
                var values = typeOfEnum.GetEnumValues().Cast<u32>().ToArray();
                var min = values.Min();
                var max = values.Max();
                compressions.uint32Min = min;
                compressions.uint32Max = max;
            }
            else
            {
                throw new NotImplementedException($"Please add {typeOfEnum.GetEnumUnderlyingType()} in to handlers");
            }
        }

        public LimitAttribute(u32 min, u32 max)
        {
            if (min >= max) throw new ArgumentException();
            type = typeof(u32);
            compressions.uint32Min = min;
            compressions.uint32Max = max;
        }

        public LimitAttribute(i32 min, i32 max)
        {
            if (min >= max) throw new ArgumentException();
            type = typeof(i32);
            compressions.i32 = new i32Limit(min, max);
        }

        public LimitAttribute(u64 min, u64 max)
        {
            if (min >= max)
            {
                throw new ArgumentException();
            }

            type = typeof(u64);
            compressions.uint64Min = min;
            compressions.uint64Max = max;
        }

        public LimitAttribute(i64 min, i64 max)
        {
            if (min >= max) throw new ArgumentException();
            type = typeof(i64);
            compressions.int64Min = min;
            compressions.int64Max = max;
        }

        public LimitAttribute(i16 min, i16 max)
        {
            if (min >= max) throw new ArgumentException();
            type = typeof(i16);
            compressions.i16 = new i16Limit(min, max);
        }

        public LimitAttribute(u8 min, u8 max)
        {
            type = typeof(u8);
            compressions.u8 = new u8Limit(min, max);
        }

        public LimitAttribute(i8 min, i8 max)
        {
            if (min >= max) throw new ArgumentException();
            type = typeof(i8);
            compressions.sbyteMin = min;
            compressions.sbyteMax = max;
        }

        public LimitAttribute(u16 min, u16 max)
        {
            if (min >= max) throw new ArgumentException();
            type = typeof(u16);
            compressions.u16 = new u16Limit(min, max);
        }
    }
}