namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    public class SpawnPointConfig
    {
        public SpawnPointConfig(float x, float y, float z, float yaw)
        {
            X = x;
            Y = y;
            Z = z;
            Yaw = yaw;
        }

        public SpawnPointConfig() { }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float Yaw { get; set; }
    }
}
