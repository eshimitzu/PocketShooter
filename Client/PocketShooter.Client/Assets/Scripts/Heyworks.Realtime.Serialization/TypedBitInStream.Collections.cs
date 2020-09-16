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
        // TODO: allow to read array into preallocated span-array or use pool
        /// <summary>
        /// Reads span of unmanaged structs.
        /// </summary>
        /// <typeparam name="T">Any struct with no references to managed heap.</typeparam>
        public T[] ReadArray<T>(int maxLength)
            where T : unmanaged
        {
            var length = Stream.i32(maxLength);
            var data = new T[length];
            for (var i = 0; i < length; i++)
            {
                ref T result = ref data[i];
                ReadInto(ref result);
            }

            return data;
        }

        public T[] ReadArray<T>()
            where T : unmanaged
        {
            var length = Stream.i32();
            var data = new T[length];
            for (var i = 0; i < length; i++)
            {
                ref T result = ref data[i];
                ReadInto(ref result);
            }

            return data;
        }
    }
}
