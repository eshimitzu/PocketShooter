using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Numerics;
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

namespace NetStack.Serialization
{
    /// <summary>
    /// Store whole update value only if different from baseline prefixed by bool.
    /// </summary>
    public static class BDiffBitBufferExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void f32BDiff(this BitBufferWriter<SevenBitEncoding> self, f32 baseline, f32 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.f32(update);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void f32BDiff(this BitBufferWriter<SevenBitEncoding> self, f32 baseline, f32 update, f32 min, f32 max, f32 precision)
        {
            if (baseline != update)
            {
                self.b(true);
                self.f32(update, min, max, precision);
            }
            else
                self.b(false);
        } 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 f32BDiff(this BitBufferReader<SevenBitDecoding> self, f32 baseline) =>
            self.b() ? self.f32() : baseline;     

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 f32BDiff(this BitBufferReader<SevenBitDecoding> self, f32 baseline, f32 min, f32 max, f32 precision) =>
            self.b() ? self.f32(min, max, precision) : baseline;     

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void i32BDiff(this BitBufferWriter<SevenBitEncoding> self, i32 baseline, i32 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.i32(update);
            }
            else
                self.b(false);
        }         

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 i32BDiff(this BitBufferReader<SevenBitDecoding> self, i32 baseline) =>
            self.b() ? self.i32() : baseline;      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 i32BDiff(this BitBufferReader<SevenBitDecoding> self, i32 baseline, i32 min, i32 max) =>
            self.b() ? self.i32(min, max) : baseline;      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void i32BDiff(this BitBufferWriter<SevenBitEncoding> self, i32 baseline, i32 update, i32 min, i32 max)
        {
            if (baseline != update)
            {
                self.b(true);
                self.i32(update,min, max);
            }
            else
                self.b(false);
        }     

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void u32BDiff(this BitBufferWriter<SevenBitEncoding> self, u32 baseline, u32 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.u32(update);
            }
            else
                self.b(false);
        }         

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static u32 u32BDiff(this BitBufferReader<SevenBitDecoding> self, u32 baseline) =>
            self.b() ? self.u32() : baseline;      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static u32 u32BDiff(this BitBufferReader<SevenBitDecoding> self, u32 baseline, u32 min, u32 max) =>
            self.b() ? self.u32(min, max) : baseline;      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void u32BDiff(this BitBufferWriter<SevenBitEncoding> self, u32 baseline, u32 update, u32 min, u32 max)
        {
            if (baseline != update)
            {
                self.b(true);
                self.u32(update,min, max);
            }
            else
                self.b(false);
        }             

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void u64BDiff(this BitBufferWriter<SevenBitEncoding> self, u64 baseline, u64 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.u64(update);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static u64 u64BDiff(this BitBufferReader<SevenBitDecoding> self, u64 baseline) =>
            self.b() ? self.u64() : baseline;  


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void f64BDiff(this BitBufferWriter<SevenBitEncoding> self, f64 baseline, f64 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.f64(update);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f64 f64BDiff(this BitBufferReader<SevenBitDecoding> self, f64 baseline) =>
            self.b() ? self.f64() : baseline;  

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void i16BDiff(this BitBufferWriter<SevenBitEncoding> self, i16 baseline, i16 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.i16(update);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i16 i16BDiff(this BitBufferReader<SevenBitDecoding> self, i16 baseline) =>
            self.b() ? self.i16() : baseline;  


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i16 i16BDiff(this BitBufferReader<SevenBitDecoding> self, i16 baseline, i16 min, i16 max) =>
            self.b() ? self.i16(min, max) : baseline;      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void i16BDiff(this BitBufferWriter<SevenBitEncoding> self, i16 baseline, i16 update, i16 min, i16 max)
        {
            if (baseline != update)
            {
                self.b(true);
                self.i16(update,min, max);
            }
            else
                self.b(false);
        }   


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void i8BDiff(this BitBufferWriter<SevenBitEncoding> self, i8 baseline, i8 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.i8(update);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i8 i8BDiff(this BitBufferReader<SevenBitDecoding> self, i8 baseline) =>
            self.b() ? self.i8() : baseline;  

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i8 i8BDiff(this BitBufferReader<SevenBitDecoding> self, i8 baseline, i8 min, i8 max) =>
            self.b() ? self.i8(min, max) : baseline;      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void i8BDiff(this BitBufferWriter<SevenBitEncoding> self, i8 baseline, i8 update, i8 min, i8 max)
        {
            if (baseline != update)
            {
                self.b(true);
                self.i8(update,min, max);
            }
            else
                self.b(false);
        }   


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void u8BDiff(this BitBufferWriter<SevenBitEncoding> self, u8 baseline, u8 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.u8(update);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static u8 u8BDiff(this BitBufferReader<SevenBitDecoding> self, u8 baseline) =>
            self.b() ? self.u8() : baseline;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static u8 u8BDiff(this BitBufferReader<SevenBitDecoding> self, u8 baseline, u8 min, u8 max) =>
            self.b() ? self.u8(min, max) : baseline;      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void u8BDiff(this BitBufferWriter<SevenBitEncoding> self, u8 baseline, u8 update, u8 min, u8 max)
        {
            if (baseline != update)
            {
                self.b(true);
                self.u8(update,min, max);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void u16BDiff(this BitBufferWriter<SevenBitEncoding> self, u16 baseline, u16 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.u16(update);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static u16 u16BDiff(this BitBufferReader<SevenBitDecoding> self, u16 baseline) =>
            self.b() ? self.u16() : baseline;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static u16 u16BDiff(this BitBufferReader<SevenBitDecoding> self, u16 baseline, u16 min, u16 max) =>
            self.b() ? self.u16(min, max) : baseline;      

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void u16BDiff(this BitBufferWriter<SevenBitEncoding> self, u16 baseline, u16 update, u16 min, u16 max)
        {
            if (baseline != update)
            {
                self.b(true);
                self.u16(update,min, max);
            }
            else
                self.b(false);
        }  


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void i64BDiff(this BitBufferWriter<SevenBitEncoding> self, i64 baseline, i64 update)
        {
            if (baseline != update)
            {
                self.b(true);
                self.i64(update);
            }
            else
                self.b(false);
        }   

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i64 i64BDiff(this BitBufferReader<SevenBitDecoding> self, i64 baseline) =>
            self.b() ? self.i64() : baseline;                                     
    }
}