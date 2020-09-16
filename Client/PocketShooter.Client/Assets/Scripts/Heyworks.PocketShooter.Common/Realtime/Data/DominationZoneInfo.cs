namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class DominationZoneInfo
    {
        public DominationZoneInfo(byte id, float x, float y, float z, float radius)
        {
            Id = id;
            X = x;
            Y = y;
            Z = z;
            Radius = radius;
            RadiusSqr = radius * radius;
        }

        private DominationZoneInfo(){}

        public byte Id { get; private set; }

        public float X { get; private set; }

        public float Y { get; private set; }

        public float Z { get; private set; }

        public float Radius { get; private set; }

        public float RadiusSqr { get; }
    }
}
