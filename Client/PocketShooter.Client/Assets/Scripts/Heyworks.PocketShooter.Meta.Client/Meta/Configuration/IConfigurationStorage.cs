using Heyworks.PocketShooter.Meta.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IConfigurationStorage
    {
        GameConfig GameConfig { get; }

        void SetGameConfig(GameConfig gameConfig);
    }
}
