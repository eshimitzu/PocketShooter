using System;
using Heyworks.PocketShooter.Communication;
using Heyworks.PocketShooter.Configuration;

namespace Heyworks.PocketShooter.Tests
{
    public class Config : IAppConfiguration
    {
        public Config(string ip)
        {
            MetaServerAddress = new ServerAddress(ip, 5000);
        }

        public string Version => new Version(0, 2).ToString();

        public string BundleId => "com.pocket.shooter.test";

        public ServerAddress MetaServerAddress { get; }
    }
}