using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Heyworks.PocketShooter.Data
{
    [StructLayout(LayoutKind.Explicit)]
    public struct UnionComponent
    {
        [FieldOffset(0)]
        public float a;

        [FieldOffset(0)]
        public long b;
    }
}
