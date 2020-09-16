using System;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Meta
{
    internal sealed class MatchMakingObserver : IMatchMakingObserver
    {
        private readonly GameManagementChannel channel;

        public MatchMakingObserver(GameManagementChannel channel) => this.channel = channel;

        public void StartGame(GameStartRequest request) =>
            channel.SendMessage(new StartGameMessage(request));

        public void JoinServer(RoomId roomId, PlayerInfo playerInfo) =>
            channel.SendMessage(new JoinServerMessage(roomId, playerInfo));

        public void AddBot(RoomId roomId, PlayerInfo playerInfo) =>
             channel.SendMessage(new AddBotMessage(roomId, playerInfo));
    }
}
