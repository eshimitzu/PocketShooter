using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NetStack.Serialization;

namespace Heyworks.Realtime.Serialization
{
    /// <summary>
    /// Stream with data.
    /// </summary>
    public partial class TypedBitInStream<TNamespace>
    {
        public readonly BitBufferReader<SevenBitDecoding> Stream;

        public TypedBitInStream(int capacity)
        {
            this.Stream = new BitBufferReader<SevenBitDecoding>(capacity);
        }

        public TypedBitInStream(byte[] data)
        {
            this.Stream = new BitBufferReader<SevenBitDecoding>();
            this.Stream.CopyFrom(data);
        }

        public TypedBitInStream(byte[] data, BitBufferOptions config)
        {
            this.Stream = new BitBufferReader<SevenBitDecoding>(config: config);
            this.Stream.CopyFrom(data);
        }

        public void FromArray(byte[] data)
        {
            this.Stream.CopyFrom(data);
        }

        public T ReadOne<T>()
            where T : unmanaged
        {
            T t = default;
            ReadInto(ref t);
            return t;
        }

        /// <summary>
        /// Could readout right into buffer.
        /// </summary>
        public void ReadInto<T>(ref T result)
            where T : unmanaged
        {
            var fields = UnmanagedTypeRegistry<TNamespace, T>.Parse();
            if (fields.Count == 0)
            {
                result = Stream.block<T>();
            }
            else
            {
                ref var a = ref Unsafe.As<T, byte>(ref result);
                for (var f = 0; f < fields.Count; f++)
                {
                    var field = fields[f];
                    if (field.Type == typeof(bool))
                    {
                        ref var s = ref Unsafe.As<byte, bool>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        s = Stream.b();
                    }
                    else
                    {
                        ReadPrimitive(ref a, field);
                    }
                }
            }
        }

        /// <summary>
        /// Reads UTF8 string.
        /// </summary>
        /// <returns>The data.</returns>
        public string ReadString() => Stream.String();

        /// <summary>
        /// Reads baselined UTF8 string.
        /// </summary>
        /// <param name="baseline">Used to diff with string.</param>
        /// <returns>The data.</returns>
        public string ReadStringDiff(string baseline) => Stream.b() ? Stream.String() : baseline;

        public void Reset() => Stream.Reset();

        /// <summary>
        /// Reads one element from stream.
        /// </summary>
        /// <param name="baseline">Used to diff with data.</param>
        /// <typeparam name="T">Element with predefined size.</typeparam>
        /// <returns>The value.</returns>
        public T ReadDiff<T>(in T baseline)
            where T : unmanaged
        {
            var fields = UnmanagedTypeRegistry<TNamespace, T>.Parse();
            if (fields.Count == 0)
            {
                var modified = Stream.b();
                return modified ? Stream.block<T>() : baseline;
            }
            else
            {
                T result = baseline;
                ref var a = ref Unsafe.As<T, byte>(ref result);
                for (var i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];
                    if (field.Type == typeof(bool))
                    {
                        ref var s = ref Unsafe.As<byte, bool>(ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset)));
                        s = Stream.b();
                    }
                    else
                    {
                        var modified = Stream.b();
                        if (modified)
                        {
                            ReadPrimitive(ref a, field);
                        }
                    }
                }

                return result;
            }
        }
    }
}
