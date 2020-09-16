using Heyworks.PocketShooter.Communication;

namespace Heyworks.PocketShooter.Configuration
{
    public interface IAppConfiguration
    {
        string Version { get; }

        string BundleId { get; }

        ServerAddress MetaServerAddress { get; }
    }
}
