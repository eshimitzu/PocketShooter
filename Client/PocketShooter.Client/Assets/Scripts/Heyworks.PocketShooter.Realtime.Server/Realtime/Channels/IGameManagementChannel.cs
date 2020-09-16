using Heyworks.PocketShooter.Realtime.Entities;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    public interface IGameManagementChannel
    {
        void JoinRoom(PlayerId playerId, ChannelWriter<IMessage> playerChannel);

        void LeaveRoom(PlayerId playerId);

        void CloseRoom(RoomId roomId);

        void RequestBotControl(PlayerId playerId);

        void ApplyMatchResults(RoomId roomId, IReadOnlyList<PlayerMatchResults> matchResults);

        void UpdateConsumables(PlayerId playerId, int usedOffensives, int usedSupports);
    }
}
