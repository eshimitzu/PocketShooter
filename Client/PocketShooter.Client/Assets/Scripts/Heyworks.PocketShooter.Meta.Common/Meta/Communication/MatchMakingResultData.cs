using System.Net;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class MatchMakingResultData
    {
        // for serializer
        private MatchMakingResultData() { }

        public MatchMakingResultData(IPEndPoint serverAddress, MapNames map) 
        {
            ServerAddress = serverAddress;
            Map = map;
        }

        public IPEndPoint ServerAddress { get; set; }
        public MapNames Map { get; set;}
    }
}