using System;
using Heyworks.PocketShooter.Communication;
using UnityEngine;

namespace Heyworks.PocketShooter.Configuration
{
    [Serializable]
    public class AppConfiguration : IAppConfiguration
    {
        [SerializeField]
        private string metaServerIp;
        [SerializeField]
        private int metaServerPort;
        [SerializeField]
        private string version;
        [SerializeField]
        private string bundleId;

        private ServerAddress? debugServerAddress;

        public string Version => version;

        public string BundleId => bundleId;


        public ServerAddress MetaServerAddress => debugServerAddress ?? new ServerAddress(metaServerIp, metaServerPort);

        internal void SetDebugServerAddress(in ServerAddress address)
        {
            debugServerAddress = address;
        }
    }
}