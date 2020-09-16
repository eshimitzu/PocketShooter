namespace Heyworks.PocketShooter.Meta.Entities
{
    public class User
    {
        public User(string id, string deviceId)
        {
            Id = id;
            DeviceId = deviceId;
            SocialConnections = new SocialConnections();
        }

        public string Id { get; private set; }

        public string DeviceId { get; private set; }

        public SocialConnections SocialConnections { get; private set; }
    }
}
