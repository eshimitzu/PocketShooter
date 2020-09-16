namespace Heyworks.Realtime.Serialization.Data
{
    public partial class TypedBitStreamTests
    {
        public struct WellCompressedStruct
        {
            [Limit(-1000f, 1000f, 0.001f)]
            public float x;// -1_000 to 1_000 with 0.001

            [Limit(0f, 360f, 0.01f)]
            public float angle; // 0 to 360 with 0.01, same as 360_01

            [Limit(0, 10000)]
            public int health; // 0 to 10000

            [Limit(typeof(MyByteEnum))]
            public MyByteEnum enumeration;
        }

        public struct NotCompressedStruct
        {
            public float x;
            public float angle;

            public int health;
            public MyByteEnum enumeration;
        }
    }
}
