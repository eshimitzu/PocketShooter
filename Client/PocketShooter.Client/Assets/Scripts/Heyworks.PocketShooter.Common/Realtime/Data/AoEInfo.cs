namespace Heyworks.PocketShooter.Realtime.Data
{
    public sealed class AoEInfo
    {
        public AoEInfo(float radius, float height)
        {
            Radius = radius;
            RadiusSqr = radius * radius;
            Height = height;
        }

        public float Radius { get; private set; }

        public float RadiusSqr { get; }

        public float Height { get; private set; }

    }
}
