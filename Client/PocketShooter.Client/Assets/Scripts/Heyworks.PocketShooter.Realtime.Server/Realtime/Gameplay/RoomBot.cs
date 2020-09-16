using System.Linq;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Gameplay
{
    internal sealed class RoomBot
    {
        public RoomBot(PlayerInfo playerInfo, TeamNo teamNo, EntityId trooperId)
        {
            Info = playerInfo;
            TeamNo = teamNo;
            TrooperId = trooperId;
        }

        public BotId Id => Info.Id;

        public PlayerInfo Info { get; }

        public TeamNo TeamNo { get; }

        public EntityId TrooperId { get; }

        public TrooperInfo GetTrooperInfo(TrooperClass trooperClass)
            => Info.Troopers.First(item => item.Class == trooperClass);
    }
}
