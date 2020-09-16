using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Heyworks.Realtime.State
{
    // TODO: Move into Heyworks.Realtime.State project
    public unsafe static class UnsafeClone
    {
        /// <summary>
        /// Clones raw bytes of struct from to. Copies raw references as well.
        /// </summary>
        public static void Clone<T>(in T from, ref T to)
            where T : struct
        {
            var fromPtr = Unsafe.AsPointer(ref Unsafe.AsRef<T>(in from));
            Unsafe.Copy<T>(ref to, fromPtr);
        }
    }
}
