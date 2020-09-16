using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;

namespace Heyworks.PocketShooter.Client.Configuration
{
    public class ConfigurationStorage : IConfigurationStorage
    {
        public GameConfig GameConfig { get; private set; }

        public void SetGameConfig(GameConfig gameConfig)
        {
            GameConfig = gameConfig;
            GameConfig.BuildIndexes();
        }
    }
}
