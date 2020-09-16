using Heyworks.PocketShooter.Communication;
using Heyworks.PocketShooter.Meta.Communication;

namespace Heyworks.PocketShooter.Meta.Realtime
{
    public class LocalMatchMakingService : IMatchMakingService
    {
        private string ip = "127.0.0.1";
        private int port = 5055;

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

        public ServerAddress GetServer() => new ServerAddress(Ip, port);
    }
}