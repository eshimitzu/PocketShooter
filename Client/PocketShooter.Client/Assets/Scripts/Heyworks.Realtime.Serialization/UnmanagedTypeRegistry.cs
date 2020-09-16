using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using ObjectLayoutInspector;
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
    public static class UnmanagedTypeRegistry
    {
        public static ILogger Log { get; set; }
    }

    /// <summary>
    /// Does not support type reloading. Thread safe.
    /// </summary>
    /// <typeparam name="TSharing">Use marker interface to do split serialization.</typeparam>
    /// <typeparam name="TComponent">The type of component.</typeparam>
    public static class UnmanagedTypeRegistry<TSharing, TComponent>
        where TComponent : unmanaged
    {
        /// <summary>
        /// Gets.
        /// </summary>
        internal static IReadOnlyList<StructFieldInfo> FieldsInfo { get; private set; }

        /// <summary>
        /// Gets all metadata of fields.
        /// </summary>
        internal static IReadOnlyList<StructFieldInfo> Parse()
        {
            if (FieldsInfo == null)
            {
                if (!IsExplicitLayout(typeof(TComponent)))
                {
                    throw new NotSupportedException("Structures should have Sequential or Explicit layout. {typeof(TComponent)} not.");
                }

                if (ToKnown(typeof(TComponent)) != KnownTypes.Unknown)
                {
                    throw new InvalidOperationException($"Must not parse well known {typeof(TComponent)} type ");
                }

                var layout = UnsafeLayout.GetFieldsLayout<TComponent>();

                FieldLayout fieldInfo = null;
                for (var i = 0; i < layout.Count; i++)
                {
                    fieldInfo = layout[i];
                    if (!IsExplicitLayout(fieldInfo.FieldInfo.FieldType))
                    {
                        throw new NotSupportedException($"Structures should have Sequential or Explicit layout. {fieldInfo.FieldInfo.FieldType} not.");
                    }

                    if (fieldInfo.FieldInfo.DeclaringType != null && !IsExplicitLayout(fieldInfo.FieldInfo.DeclaringType))
                    {
                        throw new NotSupportedException("Structures should have Sequential or Explicit layout. {fieldInfo.FieldInfo.DeclaringType} not.");
                    }
                }

                var fieldsInfo = layout
                                .Select(FromLayout)
                                .ToList();
                FieldsInfo = TuneFieldInfo(layout, fieldsInfo);
                DumpLayout();
            }

            return FieldsInfo;
        }

        // dumps layout into UnmanagedTypeRegistry.Log
        public static void DumpLayout()
        {
            UnmanagedTypeRegistry.Log?.LogInformation("{Type} has {X} fields", typeof(TComponent), FieldsInfo.Count);
            foreach (var item in FieldsInfo)
            {
                UnmanagedTypeRegistry.Log?.LogInformation(item.ToString());
            }
        }

        private static List<StructFieldInfo> TuneFieldInfo(IReadOnlyList<FieldLayout> layout, List<StructFieldInfo> fieldsInfo)
        {
            if (fieldsInfo.Count == 0 && typeof(TComponent).IsEnum)
            {
                throw new NotImplementedException($"{typeof(TComponent)} are single field structs on Andoroid and Windows!");
            }

            if (layout.Count == 1)
            {

                var fi = fieldsInfo[0];
                // limit all enums by default
                if (typeof(TComponent).IsEnum)
                {
                    fi.compressions = new LimitAttribute(typeof(TComponent)).compressions;
                    fi.compress = true;
                    UnmanagedTypeRegistry.Log?.LogInformation("Enum {Component} has one field", typeof(TComponent));
                }
                else // primitive like types with single field
                {
                    var attribute = typeof(TComponent).GetCustomAttribute<LimitAttribute>();
                    if (attribute != null)
                    {
                        fi.compressions = attribute.compressions;
                        fi.compress = true;
                    }
                    else if (!typeof(TComponent).Namespace.StartsWith("System"))
                    {
                        // try convetion (very common one - so will work)
                        var (compress, compressions) = TuneType(typeof(TComponent));
                        fi.compress = compress;
                        fi.compressions = compressions;
                    }
                    else
                    {
                        UnmanagedTypeRegistry.Log?.LogInformation("{Component} has one field", typeof(TComponent));
                    }
                }

                fieldsInfo[0] = fi;
            }

            return fieldsInfo;
        }

        private static (bool compress, Compressions compressions) TuneType(Type type)
        {
            var compress = false;
            Compressions compressions = default;
            var constants = type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.IsLiteral || x.IsInitOnly);
            
            // TODO: use Range instead of custom hack
            var minValueConst = constants.Where(x => x.Name == "MinValue").SingleOrDefault();
            var maxValueConst = constants.Where(x => x.Name == "MaxValue").SingleOrDefault();

            if (typeof(TComponent).Name.Equals("EntityId") && !constants.Any())
            {
                throw new NotImplementedException($"{typeof(TComponent)} has conventional limits on Android and Windows!");
            }

            if (minValueConst != null && maxValueConst != null && minValueConst.FieldType == maxValueConst.FieldType)
            {
                UnmanagedTypeRegistry.Log?.LogInformation("Min {Min}, Max {Max} of {Type}", minValueConst.GetValue(null), maxValueConst.GetValue(null), typeof(TComponent));
                var fieldType = minValueConst.FieldType;
                //TODO: check that there is at least on operator to convert TComponent in Value and TComponent from value
                if (fieldType == typeof(i32))
                {
                    var minValue = (i32)minValueConst.GetValue(null);
                    var maxValue = (i32)maxValueConst.GetValue(null);
                    if (minValue < maxValue && minValue != maxValue)
                    {
                        compressions = new LimitAttribute(minValue, maxValue).compressions;
                        compress = true;
                    }
                }
                else if (fieldType == typeof(i16))
                {
                    var minValue = (i16)minValueConst.GetValue(null);
                    var maxValue = (i16)maxValueConst.GetValue(null);
                    if (minValue < maxValue && minValue != maxValue)
                    {
                        compressions = new LimitAttribute(minValue, maxValue).compressions;
                        compress = true;
                    }
                }
                else if (fieldType == typeof(u8))
                {
                    var minValue = (u8)minValueConst.GetValue(null);
                    var maxValue = (u8)maxValueConst.GetValue(null);
                    if (minValue < maxValue && minValue != maxValue)
                    {
                        compressions = new LimitAttribute(minValue, maxValue).compressions;
                        compress = true;
                    }
                }
                else if (fieldType == typeof(u16))
                {
                    var minValue = (u16)minValueConst.GetValue(null);
                    var maxValue = (u16)maxValueConst.GetValue(null);
                    if (minValue < maxValue && minValue != maxValue)
                    {
                        compressions = new LimitAttribute(minValue, maxValue).compressions;
                        compress = true;
                    }
                }
                else if (fieldType == typeof(i8))
                {
                    var minValue = (i8)minValueConst.GetValue(null);
                    var maxValue = (i8)maxValueConst.GetValue(null);
                    if (minValue < maxValue && minValue != maxValue)
                    {
                        compressions = new LimitAttribute(minValue, maxValue).compressions;
                        compress = true;
                    }
                }
                else if (fieldType == typeof(u32))
                {
                    var minValue = (u32)minValueConst.GetValue(null);
                    var maxValue = (u32)maxValueConst.GetValue(null);
                    if (minValue < maxValue && minValue != maxValue)
                    {
                        compressions = new LimitAttribute(minValue, maxValue).compressions;
                        compress = true;
                    }
                }
                else if (fieldType == typeof(i64))
                {
                    var minValue = (i64)minValueConst.GetValue(null);
                    var maxValue = (i64)maxValueConst.GetValue(null);
                    if (minValue < maxValue && minValue != maxValue)
                    {
                        compressions = new LimitAttribute(minValue, maxValue).compressions;
                        compress = true;
                    }
                }
                else if (fieldType == typeof(u64))
                {
                    var minValue = (u64)minValueConst.GetValue(null);
                    var maxValue = (u64)maxValueConst.GetValue(null);
                    if (minValue < maxValue && minValue != maxValue)
                    {
                        compressions = new LimitAttribute(minValue, maxValue).compressions;
                        compress = true;
                    }
                }
                else
                {
                    throw new NotImplementedException(fieldType.ToString());
                }
            }

            return (compress, compressions);
        }

        private static StructFieldInfo FromLayout(FieldLayout x)
        {
            var attribute = x.FieldInfo.GetCustomAttribute<LimitAttribute>();
            bool compress = default;
            Compressions compressions = default;
            var fieldType = x.FieldInfo.FieldType;
            if (attribute != null)
            {
                if (attribute.type != fieldType)
                {
                    throw new InvalidProgramException($"Typeof attribute({attribute.type}) should be same as field type {x.FieldInfo.FieldType}");
                }

                if (!fieldType.IsEnum && ToKnownCompressable(fieldType) == KnownTypes.Unknown)
                {
                    throw new InvalidProgramException($"Cannot compress field of {fieldType} type. Only some known primitives types are compressable.");
                }

                compressions = attribute.compressions;
                compress = true;
            }
            else if (fieldType.IsEnum)
            {
                compressions = new LimitAttribute(fieldType).compressions;
                compress = true;
            }
            else
            {
                (compress, compressions) = TuneType(x.FieldInfo.DeclaringType);
            }

            var knownType = fieldType.IsEnum ? ToKnown(fieldType.GetEnumUnderlyingType()) : ToKnown(fieldType);

            return new StructFieldInfo
            {
                Offset = (ushort)x.Offset,
                Size = (ushort)x.Size,
                Type = x.FieldInfo.FieldType,
                Field = x.FieldInfo,
                compressions = compressions,
                compress = compress,
                KnownType = knownType
            };
        }


        private static KnownTypes ToKnownCompressable(Type fieldType)
        {
            if (fieldType == typeof(u8))
                return KnownTypes.u8;
            else if (fieldType == typeof(i32))
                return KnownTypes.i32;
            else if (fieldType == typeof(u32))
                return KnownTypes.u32;
            else if (fieldType == typeof(i8))
                return KnownTypes.i8;
            else if (fieldType == typeof(u16))
                return KnownTypes.u16;
            else if (fieldType == typeof(i16))
                return KnownTypes.i16;
            else if (fieldType == typeof(f32))
                return KnownTypes.f32;
            else if (fieldType == typeof(f64))
                return KnownTypes.f64;
            else if (fieldType == typeof(i64))
                return KnownTypes.i64;
            else if (fieldType == typeof(u64))
                return KnownTypes.u64;
            else
                return KnownTypes.Unknown;
        }


        private static KnownTypes ToKnown(Type fieldType)
        {
            if (fieldType == typeof(u8))
                return KnownTypes.u8;
            if (fieldType == typeof(i32))
                return KnownTypes.i32;
            else if (fieldType == typeof(u32))
                return KnownTypes.u32;
            else if (fieldType == typeof(Boolean))
                return KnownTypes.Boolean;
            else if (fieldType == typeof(i8))
                return KnownTypes.i8;
            else if (fieldType == typeof(u16))
                return KnownTypes.u16;
            else if (fieldType == typeof(i16))
                return KnownTypes.i16;
            else if (fieldType == typeof(f32))
                return KnownTypes.f32;
            else if (fieldType == typeof(f64))
                return KnownTypes.f64;
            else if (fieldType == typeof(Decimal))
                return KnownTypes.Decimal;
            else if (fieldType == typeof(Guid))
                return KnownTypes.Guid;
            else if (fieldType == typeof(i64))
                return KnownTypes.i64;
            else if (fieldType == typeof(IntPtr))
                return KnownTypes.IntPtr;
            else if (fieldType == typeof(UIntPtr))
                return KnownTypes.UIntPtr;
            else if (fieldType == typeof(u64))
                return KnownTypes.u64;
            else
                return KnownTypes.Unknown;
        }

        private static bool IsExplicitLayout(Type t) =>
            t.StructLayoutAttribute.Value == LayoutKind.Explicit
            || t.StructLayoutAttribute.Value == LayoutKind.Sequential
            || (t.IsEnum && Enum.GetUnderlyingType(t).IsPrimitive);
    }
}
