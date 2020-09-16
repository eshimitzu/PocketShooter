#define NETSTACK_VALIDATE
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Buffers;
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

namespace NetStack.Serialization
{
    public static class AllocationBitBufferExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string String(this BitBufferReader<SevenBitDecoding> self)
        {
            var size = MemoryPool<char>.Shared.Rent(self.Options.CharSpanLengthMax);
            var length = self.c(size.Memory.Span);
            using (var pin = size.Memory.Pin())
                return new String((char*)pin.Pointer,0, (int)length);         
        }

        /// <summary>
        /// Dot not use for production. GC allocated array.
        /// </summary>
        /// <returns></returns>
        public static byte[] ToArray<T>(this BitBufferWriter<T> self) where T:unmanaged, ICompression<BitBufferWriter<T>> 
        {
            var data = new byte[self.LengthWritten];
            self.ToSpan(data);
            return data;
        }

        /// <summary>
        /// Rents array 
        /// </summary>
        public static byte[] ToArray<T>(this BitBufferWriter<T> self, ArrayPool<byte> pool = null) where T:unmanaged, ICompression<BitBufferWriter<T>> 
        {
            pool = pool ?? ArrayPool<byte>.Shared;
            var data = pool.Rent(self.LengthWritten);
            self.ToSpan(data);
            return data;
        }               
    }
}