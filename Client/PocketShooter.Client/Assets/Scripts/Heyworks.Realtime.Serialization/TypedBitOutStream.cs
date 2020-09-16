using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using NetStack.Serialization;

namespace Heyworks.Realtime.Serialization
{
    /// <summary>
    /// Reflects schema and used it to write bits.
    /// </summary>
    public partial class TypedBitOutStream<TNamespace>
    {
        public readonly BitBufferWriter<SevenBitEncoding> Stream;

        public TypedBitOutStream(int capacity)
        {
            Stream = new BitBufferWriter<SevenBitEncoding>(capacity);
        }

        public TypedBitOutStream(int capacity, BitBufferOptions config)
        {
            Stream = new BitBufferWriter<SevenBitEncoding>(capacity, config);
        }
        public int BitsWritten => Stream.BitsWritten;

        public void WriteString(string value) => Stream.c(value.AsSpan());

        public void WriteStringDiff(string baseline, string update)
        {
            if (update != baseline)
            {
                Stream.b(true);
                Stream.c(update.AsSpan());
            }
            else
            {
                Stream.b(false);
            }
        }

        public void WriteDiff<T>(in T baseline, in T update)
            where T : unmanaged
        {
            var fields = UnmanagedTypeRegistry<TNamespace, T>.Parse();
            if (fields.Count == 0)
            {
                throw new NotImplementedException("Cannot diff of type with no fields. Custom primitive types are not implemented.");
            }
            else
            {
                ref var baselineRef = ref Unsafe.AsRef<T>(in baseline);
                ref var updateRef = ref Unsafe.AsRef<T>(in update);
                for (var i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];
                    var fieldType = field.Type;
                    ref var moveA = ref Unsafe.AddByteOffset(ref baselineRef, new IntPtr(field.Offset));
                    ref var moveB = ref Unsafe.AddByteOffset(ref updateRef, new IntPtr(field.Offset));
                    DiffPrimitive(field, ref moveA, ref moveB);
                }
            }
        }

        public void WriteOne<T>(in T readonlyValue)
            where T : unmanaged
        {
            var fields = UnmanagedTypeRegistry<TNamespace, T>.Parse();
            ref var a = ref Unsafe.AsRef(in readonlyValue);

            if (fields.Count == 0) // && notConsideredCustomPrimitive
            {
                throw new InvalidOperationException($"Type {typeof(T)} has no fields. Please write or use non generic method which adds value directly");
            }

            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var fieldType = field.Type;
                ref var moveA = ref Unsafe.AddByteOffset(ref a, new IntPtr(field.Offset));
                WritePrimitive(field, ref moveA);
            }
        }

        public void WriteDiff<T>(ReadOnlySpan<T> baseline, ReadOnlySpan<T> update, int maxLength)
            where T : unmanaged
        {
            if (baseline.Length != update.Length)
            {
                throw new InvalidOperationException("Please provide equal collections to compare");
            }

            UnmanagedTypeRegistry<TNamespace, T>.Parse();
            for (var i = 0; i < baseline.Length; i++)
            {
                ref readonly var a = ref baseline[i];
                ref readonly var b = ref update[i];
                WriteDiff<T>(in a, in b);
            }
        }

        public byte[] ToArray()
        {
            var result = Stream.ToArray();
            Stream.Reset();
            return result;
        }
    }
}
