using System.Runtime.InteropServices;

namespace Heyworks.PocketShooter.Data
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ExplicitLayoutOverlapComponent
    {
        [FieldOffset(0)]
        public float a;

        [FieldOffset(2)]
        public long b;
    }
}
