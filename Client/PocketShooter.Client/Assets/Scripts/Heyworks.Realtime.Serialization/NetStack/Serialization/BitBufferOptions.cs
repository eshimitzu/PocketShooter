﻿#define NETSTACK_VALIDATE
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Except;
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
#if !(ENABLE_MONO || ENABLE_IL2CPP)
using System.Diagnostics;
using System.Numerics;
#else
using UnityEngine;
#endif

// Until migration to .NET Standard 2.1
using BitOperations = System.Numerics.BitOperations;

namespace NetStack.Serialization
{
    public class  BitBufferOptions
    {
        public static readonly BitBufferOptions Default = new BitBufferOptions(charSpanBitsLength: DefaultCharSpanBitsLength, u8SpanBitsLength: DefaultU8SpanBitsLength);

        public const i32 DefaultU8SpanBitsLength = 9;

        public const i32 DefaultCharSpanBitsLength = 8;

        private readonly i32 u8SpanLengthMax;

        public i32 U8SpanLengthMax => u8SpanLengthMax;

        public i32 CharSpanLengthMax => charSpanLengthMax;

        private readonly i32 u8SpanBitsLength;

        public i32 U8SpanBitsLength => u8SpanBitsLength;

        private readonly i32 charSpanBitsLength;

        public i32 CharSpanBitsLength => charSpanBitsLength;

        private readonly i32 charSpanLengthMax;

        /// <summary>
        /// Creates new instance with its own buffer. 
        /// </summary>
        /// <param name="charSpanBitsLength">Bits used to store length of strings.</param>
        /// <param name="u8SpanBitsLength">Bits used to store length of byte arrays.</param>
        public BitBufferOptions(i32 charSpanBitsLength = DefaultCharSpanBitsLength, i32 u8SpanBitsLength = DefaultU8SpanBitsLength)
        {
            if (charSpanBitsLength <= 0)
                throw Argument("Should be positive", nameof(charSpanBitsLength));

            if (u8SpanBitsLength <= 0)
                throw Argument("Should be positive", nameof(u8SpanBitsLength));

            // one time setup
            this.u8SpanBitsLength = u8SpanBitsLength;
            u8SpanLengthMax = (1 << u8SpanBitsLength) - 1;
            this.charSpanBitsLength = charSpanBitsLength;
            charSpanLengthMax = (1 << charSpanBitsLength) - 1;
        }
    }
}