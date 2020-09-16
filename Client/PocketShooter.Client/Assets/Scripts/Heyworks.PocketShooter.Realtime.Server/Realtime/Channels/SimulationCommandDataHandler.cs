using System;
using System.Threading.Channels;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.DataHandlers
{
    public class SimulationCommandDataHandler : IDataHandler
    {
        private readonly IDataSerializer<SpawnTrooperCommandData> spawnTrooperSerializer = new SpawnTrooperCommandDataSerializer();
        private readonly IDataSerializer<SpawnBotTrooperCommandData> spawnBotTrooperSerializer = new SpawnBotTrooperCommandDataSerializer();
        private readonly SimulationCommandsDataSerializer commandsSerializer = new SimulationCommandsDataSerializer();

        private readonly PlayerId playerId;

        private readonly IGameManagementChannel gameManagementChannel;
        private readonly ChannelWriter<IMessage> roomChannel;
        private readonly ChannelWriter<IMessage> playerChannel;

        public SimulationCommandDataHandler(
            PlayerId playerId,
            IGameManagementChannel gameManagementChannel,
            ChannelWriter<IMessage> roomChannel,
            ChannelWriter<IMessage> playerChannel)
        {
            this.playerId = playerId;
            this.gameManagementChannel = gameManagementChannel;
            this.roomChannel = roomChannel;
            this.playerChannel = playerChannel;
        }

        public bool CanHandleData(byte dataCode)
        {
            return
                (NetworkDataCode)dataCode == NetworkDataCode.RequestBotCommand ||
                (NetworkDataCode)dataCode == NetworkDataCode.SpawnTrooperCommand ||
                (NetworkDataCode)dataCode == NetworkDataCode.SpawnBotTrooperCommand ||
                (NetworkDataCode)dataCode == NetworkDataCode.SimulationCommand;
        }

        public void HandleData(byte dataCode, byte[] data)
        {
            var code = (NetworkDataCode)dataCode;
            if (code == NetworkDataCode.RequestBotCommand)
            {
                gameManagementChannel.RequestBotControl(playerId);
            }
            else if (code == NetworkDataCode.SpawnTrooperCommand)
            {
                var commandData = spawnTrooperSerializer.Deserialize(data);
                roomChannel.WriteAsync(new SpawnTrooperMessage(playerId, commandData.TrooperClass));
            }
            else if (code == NetworkDataCode.SpawnBotTrooperCommand)
            {
                var commandData = spawnBotTrooperSerializer.Deserialize(data);
                roomChannel.WriteAsync(new SpawnBotTrooperMessage(commandData.BotId, commandData.TrooperClass));
            }
            else if (code == NetworkDataCode.LeaveRoomCommand)
            {
                gameManagementChannel.LeaveRoom(playerId);
            }
            else if (code == NetworkDataCode.SimulationCommand)
            {
                HandleSimulationCommandData(data);
            }
        }

        private void HandleSimulationCommandData(byte[] data)
        {
            try
            {
                var (ping, simulationCommands) = commandsSerializer.Deserialize(data);
                roomChannel.WriteAsync(new PingMessage(playerId, ping, simulationCommands));
            }
            catch (NotImplementedException ex)
            {
                var error = new ServerException(ClientErrorCode.CommandNotSupported, "Could be client wrong version or network-serialization issue", ex);
                NetLog.Warning("HandleSimulationCommandData", ex);
                playerChannel.WriteAsync(new ServerErrorMessage(error));
            }
        }
    }
}
