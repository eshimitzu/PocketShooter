using System;
using System.Collections.Generic;
using System.Diagnostics;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Connection;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Service
{
    public class NetworkService : INetworkService
    {
        private SimulationCommandsDataSerializer commandsSerializer = new SimulationCommandsDataSerializer();
        private IRealtimeConfiguration configuration;

        private readonly IDictionary<Type, NetworkDataCode> commandDataToNetworkDataCode =
            new Dictionary<Type, NetworkDataCode>
            {
                { typeof(JoinRoomCommandData), NetworkDataCode.JoinRoomCommand },
                { typeof(SpawnTrooperCommandData), NetworkDataCode.SpawnTrooperCommand },
                { typeof(RequestBotCommandData), NetworkDataCode.RequestBotCommand },
                { typeof(SpawnBotTrooperCommandData), NetworkDataCode.SpawnBotTrooperCommand },
                { typeof(LeaveRoomCommandData), NetworkDataCode.LeaveRoomCommand},
            };

        private readonly IDictionary<NetworkDataCode, bool> reliableConfiguration =
            new Dictionary<NetworkDataCode, bool>
            {
                { NetworkDataCode.JoinRoomCommand, true },
                { NetworkDataCode.SpawnTrooperCommand, true },
                { NetworkDataCode.RequestBotCommand, true },
                { NetworkDataCode.SpawnBotTrooperCommand, true },
                { NetworkDataCode.LeaveRoomCommand, true },
            };

        private readonly IDictionary<NetworkDataCode, IDataSerializer> commandSerializers =
            new Dictionary<NetworkDataCode, IDataSerializer>
            {
                { NetworkDataCode.JoinRoomCommand, new JoinRoomCommandDataSerializer() },
                { NetworkDataCode.SpawnTrooperCommand, new SpawnTrooperCommandDataSerializer() },
                { NetworkDataCode.RequestBotCommand, new EmptyServiceCommandDataSerializer() },
                { NetworkDataCode.SpawnBotTrooperCommand, new SpawnBotTrooperCommandDataSerializer() },
                { NetworkDataCode.LeaveRoomCommand, new EmptyServiceCommandDataSerializer() },
            };

        private readonly ICommunication communication;
        private readonly IDictionary<NetworkDataCode, IDataHandler> dataHandlers;

        public NetworkService(
            ICommunication communication,
            IDictionary<NetworkDataCode, IDataHandler> dataHandlers,
            IRealtimeConfiguration configuration)
        {
            this.communication = communication;
            this.dataHandlers = dataHandlers;
            this.configuration = configuration;
        }

        public void QueueCommand(SimulationCommandData commandData) => commandsSerializer.Add(commandData);

        public void QueueCommand(IServiceCommandData commandData)
        {
            var dataCode = commandDataToNetworkDataCode[commandData.GetType()];
            QueueCommandData(dataCode, commandData);
        }

        public void Send()
        {
            if (commandsSerializer.HasData)
            {
                var data = commandsSerializer.Serialize();
                if (data.Length > PhotonConnectionConfiguration.GuarantedMTU)
                {
                    NetLog.Log.LogWarning("Will send {SendBytes} commands' bytes to server", data.Length);
                }

                communication.QueueData(
                    new NetworkData
                    {
                        Code = NetworkDataCode.SimulationCommand,
                        Data = data,
                    },
                    configuration.ReliableCommands); // reliable until will do resend commands for several ticks
            }

            communication.Send();
        }

        public void Receive()
        {
            communication.Receive();

            while (communication.HasData())
            {
                var data = communication.GetData();
                if (dataHandlers.ContainsKey(data.Code))
                {
                    var handler = dataHandlers[data.Code];
                    handler.Handle(data);
                }
                else
                {
                    NetLog.Log.LogError("Data handler for NetworkDataCode={code} was not found.", data.Code);
                }
            }
        }

        private void QueueCommandData(NetworkDataCode dataCode, object commandData)
        {
            var serializer = commandSerializers[dataCode];
            var isReliable = reliableConfiguration[dataCode];

            communication.QueueData(
                new NetworkData
                {
                    Code = dataCode,
                    Data = serializer.Serialize(commandData),
                },
                isReliable);
        }

        public void AddPing(SimulationMetaCommandData pingCommandData, int possiblyStillAcceptedTick)
        {
            Debug.Assert(possiblyStillAcceptedTick >= 0);
            Debug.Assert(possiblyStillAcceptedTick <= pingCommandData.Tick);
            if (configuration.ReliableCommands)
                possiblyStillAcceptedTick = pingCommandData.Tick;
            if (!configuration.DiffState)
                pingCommandData = new SimulationMetaCommandData(pingCommandData.Tick, -1);
            commandsSerializer.AddPing(pingCommandData, possiblyStillAcceptedTick);
        }
    }
}