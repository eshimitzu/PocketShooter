using System.Runtime.InteropServices;

namespace Heyworks.PocketShooter.Data
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ExplicitLayoutNoOverlapComponent
    {
        [FieldOffset(0)]
        public float a;

        [FieldOffset(10)]
        public long b;
    }
}
