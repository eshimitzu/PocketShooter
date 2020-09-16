namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class SpawnPointInfo
    {
        public SpawnPointInfo(float x, float y, float z, float yaw)
        {
            X = x;
            Y = y;
            Z = z;
            Yaw = yaw;
        }

        private SpawnPointInfo(){}

        // TODO: use Position or Vector3
        public float X { get; private set; }

        public float Y { get; private set; }

        public float Z { get; private set; }

        public float Yaw { get; private set; }
    }
}
