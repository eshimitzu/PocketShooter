using System.Net;
using Heyworks.PocketShooter.Realtime;

namespace Heyworks.PocketShooter.Networking
{
    public class GameSession
    {
        public GameSession(IPEndPoint server, PlayerId player)
        {
            Server = server;
            Player = player;
        }

        public IPEndPoint Server {get;}
        public PlayerId Player {get;}
    }
}