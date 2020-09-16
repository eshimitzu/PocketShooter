using System.Net;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Realtime.Configuration
{
    internal interface INetworkConfiguration
    {
        Task<IPEndPoint> GetPublicIPAddress();
    }
}