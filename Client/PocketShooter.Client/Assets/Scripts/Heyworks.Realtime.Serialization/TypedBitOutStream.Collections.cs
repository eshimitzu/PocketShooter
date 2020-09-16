using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using NetStack.Serialization;

namespace Heyworks.Realtime.Serialization
{

    partial class TypedBitOutStream<TNamespace>
    {
        /// <summary>
        /// Write span of unmanaged stucts. Respects limits rules applied to members of stucts.
        /// </summary>
        /// <typeparam name="T">Any stuct with no references to managed heap.</typeparam>
        /// <param name="readonlySpan">The value.</param>
        public void Write<T>(ReadOnlySpan<T> readonlySpan)
            where T : unmanaged
        {
            UnmanagedTypeRegistry<TNamespace, T>.Parse();
            Stream.i32(readonlySpan.Length);
            for (var i = 0; i < readonlySpan.Length; i++)
                WriteOne(in readonlySpan[i]);
        }

        /// <summary>
        /// Write span of unmanaged stucts. Respects limits rules applied to members of stucts.
        /// </summary>
        /// <typeparam name="T">Any stuct with no references to managed heap.</typeparam>
        /// <param name="readonlySpan">The value.</param>
        public void Write<T>(in Span<T> readonlySpan)
            where T : unmanaged
        {
            UnmanagedTypeRegistry<TNamespace, T>.Parse();
            Stream.i32(readonlySpan.Length);
            for (var i = 0; i < readonlySpan.Length; i++)
                WriteOne(in readonlySpan[i]);
        }

        public void Write<T>(in T[] readonlySpan)
           where T : unmanaged
        {
            UnmanagedTypeRegistry<TNamespace, T>.Parse();
            Stream.i32(readonlySpan.Length);
            for (var i = 0; i < readonlySpan.Length; i++)
                WriteOne(in readonlySpan[i]);
        }

        /// <summary>
        /// Write span of unmanaged stucts. Respects limits rules applied to members of stucts.
        /// </summary>
        /// <typeparam name="T">Any stuct with no references to managed heap.</typeparam>
        /// <param name="readonlySpan">The value.</param>
        public void Write<T>(ReadOnlySpan<T> readonlySpan, int maxLength)
            where T : unmanaged
        {
            UnmanagedTypeRegistry<TNamespace, T>.Parse();
            Stream.i32(readonlySpan.Length, maxLength);
            for (var i = 0; i < readonlySpan.Length; i++)
                WriteOne(in readonlySpan[i]);
        }

        /// <summary>
        /// Write span of unmanaged stucts. Respects limits rules applied to members of stucts.
        /// </summary>
        /// <typeparam name="T">Any stuct with no references to managed heap.</typeparam>
        /// <param name="readonlySpan">The value.</param>
        public void Write<T>(in Span<T> readonlySpan, int maxLength)
            where T : unmanaged
        {
            UnmanagedTypeRegistry<TNamespace, T>.Parse();
            Stream.i32(readonlySpan.Length, maxLength);
            for (var i = 0; i < readonlySpan.Length; i++)
                WriteOne(in readonlySpan[i]);
        }

        public void Write<T>(in T[] readonlySpan, int maxLength)
           where T : unmanaged
        {
            UnmanagedTypeRegistry<TNamespace, T>.Parse();
            Stream.i32(readonlySpan.Length, maxLength);
            for (var i = 0; i < readonlySpan.Length; i++)
                WriteOne(in readonlySpan[i]);
        }
    }
}
