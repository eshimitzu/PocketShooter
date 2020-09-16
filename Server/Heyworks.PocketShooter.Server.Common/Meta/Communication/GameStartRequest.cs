using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Communication;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class GameStartRequest
    {
        public GameStartRequest(){}

        public GameStartRequest(Guid room, DominationModeInfo modeInfo, MatchType matchType, int initialRealPlayers, int desiredBots, MapNames map)
        {
            RoomId = room;
            ModeInfo = modeInfo;
            MatchType = matchType;
            InitialRealPlayers = initialRealPlayers;
            DesiredBots = desiredBots;
            Map = map;
        }
        public Guid RoomId {get;set;}
        public DominationModeInfo ModeInfo {get;set;} 
        public MatchType MatchType {get;set;}
        public int InitialRealPlayers { get; }
        public int DesiredBots { get; }
        public MapNames Map { get; }
    }
}
