namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class StoredServerGameConfig
    {
        // for storage
        private StoredServerGameConfig() { }

        public StoredServerGameConfig(string id, ServerGameConfig config)
        {
            Id = id;
            Config = config;
        }

        /// <summary>
        /// Gets the id of the config.
        /// </summary>
        public string Id { get; }

        public ServerGameConfig Config { get; }
    }
}
