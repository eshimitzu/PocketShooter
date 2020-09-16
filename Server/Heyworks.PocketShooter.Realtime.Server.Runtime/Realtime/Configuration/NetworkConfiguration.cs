using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Configuration
{
    internal class NetworkConfiguration : INetworkConfiguration
    {
        private readonly IConfiguration configuration;
        private readonly IPUtils ipUtils;

        public NetworkConfiguration(IConfiguration configuration, ILogger<NetworkConfiguration> logger)
        {
            this.configuration = configuration;
            this.ipUtils = new IPUtils(new HttpClient(), logger);
        }

        public async Task<IPEndPoint> GetPublicIPAddress()
        {
            var address = await ipUtils.GetPublicIpAddress(configuration.GetValue<string>("Photon:PublicIPAddress"));
            var port = configuration.GetValue<int>("PocketShooter:UDPListeners:UDPListener:Port");

            return new IPEndPoint(address, port);
        }
    }
}