using System;
using System.Collections.Generic;
using System.Diagnostics;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Connection;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Service;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents state for starting new realtime game. Connects to the server and joins to the room.
    /// </summary>
    public class GameEntryState : IGameState
    {
        private const int MaxNumberOfAttempts = 2;
        private const int JoiningTimeoutMs = 10000;

        private readonly GameStateController stateController;
        private readonly IGameplayConnection connection;
        private readonly INetworkService networkService;
        private readonly IRealtimeConfiguration realtimeConfiguration;
        private readonly PlayerId playerId;

        private State state = State.ConnectingToServer;
        private int currentAttemptNumber;
        private Stopwatch startJoiningStopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameEntryState" /> class.
        /// </summary>
        /// <param name="stateController">The game state controller.</param>
        /// <param name="connection">The game connection.</param>
        /// <param name="realtimeConfiguration">The realtime configuration.</param>
        /// <param name="playerId">The player id.</param>
        public GameEntryState(
            GameStateController stateController,
            IGameplayConnection connection,
            IRealtimeConfiguration realtimeConfiguration,
            PlayerId playerId)
        {
            // set log for debugging serialization internals
            // UnmanagedTypeRegistry.Log = SerializationLog.Log;

            this.stateController = stateController.NotNull();
            this.connection = connection.NotNull();
            this.realtimeConfiguration = realtimeConfiguration.NotNull();
            this.playerId = playerId;

            var gameJoinedDataHandler = new GameJoinedDataHandler();
            var serverErrorDataHandler = new ServerErrorDataHandler();

            var handlers = new Dictionary<NetworkDataCode, IDataHandler>
            {
                { NetworkDataCode.GameJoined, gameJoinedDataHandler },
                { NetworkDataCode.SimulationState, new EmptyDataHandler() },
                { NetworkDataCode.ServerError, serverErrorDataHandler},
            };

            networkService = new NetworkService(connection, handlers, realtimeConfiguration);

            gameJoinedDataHandler.GameJoined += GameJoinedDataHandler_GameJoined;
            serverErrorDataHandler.Error += ServerErrorDataHandlerError;
        }

        public void Update()
        {
            networkService.Receive();

            switch (state)
            {
                case State.ConnectingToServer:
                    ConnectToServerAndJoinRoom();

                    break;
                case State.JoiningRoom:
                    NetLog.Log.LogTrace("Joining room...");

                    if (startJoiningStopwatch.ElapsedMilliseconds > JoiningTimeoutMs)
                    {
                        Disconnect(DisconnectReason.JoinRoomTimeout);
                    }

                    if (connection.ConnectionState == ConnectionState.Disconnected)
                    {
                        state = State.ConnectingToServer;
                    }

                    break;
                case State.Ready:
                    NetLog.Log.LogTrace("Ready to play.");

                    if (connection.ConnectionState == ConnectionState.Disconnected)
                    {
                        state = State.ConnectingToServer;
                    }

                    break;
            }

            networkService.Send();
        }

        private void ConnectToServerAndJoinRoom()
        {
            switch (connection.ConnectionState)
            {
                case ConnectionState.Disconnected:

                    currentAttemptNumber++;
                    if (currentAttemptNumber <= MaxNumberOfAttempts)
                    {
                        NetLog.Log.LogInformation(
                            "Try connect to photon server: {serverAddress}. Attempt {currentAttemptNumber} of {MaxNumberOfAttempts}.",
                            connection.ServerAddress,
                            currentAttemptNumber,
                            MaxNumberOfAttempts);

                        connection.Connect();
                    }
                    else
                    {
                        Disconnect(DisconnectReason.ConnectionTimeout);
                    }

                    break;
                case ConnectionState.Connecting:
                    NetLog.Log.LogTrace("Connecting to photon server: {serverAddress}...", connection.ServerAddress);
                    break;
                case ConnectionState.Connected:
                    NetLog.Log.LogTrace("Send join room operation. Player id is {playerId}...", playerId);
                    networkService.QueueCommand(new JoinRoomCommandData(playerId));
                    state = State.JoiningRoom;
                    startJoiningStopwatch = Stopwatch.StartNew();
                    break;
            }
        }

        private void Disconnect(DisconnectReason reason)
        {
            var disconnectState = new GameDisconnectedState(reason);
            stateController.SetCurrentState(disconnectState);
        }

        private void ServerErrorDataHandlerError(ClientErrorCode errorCode)
        {
            DisconnectReason reason;
            switch (errorCode)
            {
                case ClientErrorCode.RoomIsFull:
                    reason = DisconnectReason.RoomIsFull;
                    break;
                case ClientErrorCode.SomethingWrongHappenedAndWeDoNotKnowWhat:
                case ClientErrorCode.CommandNotSupported:
                case ClientErrorCode.NotAReleaseFeature:
                case ClientErrorCode.EntityLimitReached:
                    reason = DisconnectReason.ServerConnectionError;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null);
            }

            Disconnect(reason);
        }

        private void GameJoinedDataHandler_GameJoined(GameJoinedData gameJoinedData)
        {
            NetLog.Log.LogTrace(
                "Joined the room. Player id is {playerId}, team is {team}.",
                gameJoinedData.TrooperId,
                gameJoinedData.TeamNo);

            var room = new Room(
                connection,
                realtimeConfiguration,
                gameJoinedData.RoomId,
                gameJoinedData.TrooperId,
                gameJoinedData.TeamNo,
                gameJoinedData.PlayerInfo,
                gameJoinedData.ModeInfo);

            var nextState = new GameRunningState(stateController, room);
            stateController.SetCurrentState(nextState);
            state = State.Ready;

            // TODO: a.dezhurko move to another place when join and start will be separated. Must be called after setting next state.
            room.StartGame(
                gameJoinedData.Tick,
                gameJoinedData.TimeStamp);
        }

        private enum State
        {
            ConnectingToServer,
            JoiningRoom,
            Ready,
        }

        private class EmptyDataHandler : IDataHandler
        {
            public void Handle(NetworkData data)
            {
            }
        }
    }
}