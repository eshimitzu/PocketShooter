namespace Heyworks.Realtime.Serialization.Data
{
    public struct RangedIntComponent
    {
        public int a;

        public int MaxValue => short.MaxValue;

        public int MinValue => 0;
    }
}
