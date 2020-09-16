namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class DominationZoneConfig
    {
        public DominationZoneConfig(byte id, float x, float y, float z, float radius)
        {
            Id = id;
            X = x;
            Y = y;
            Z = z;
            Radius = radius;
        }

        public DominationZoneConfig() { }

        public byte Id { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float Radius { get; set; }
    }
}
