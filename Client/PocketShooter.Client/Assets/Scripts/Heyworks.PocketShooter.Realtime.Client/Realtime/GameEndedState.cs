using System;
using Heyworks.PocketShooter.Realtime.Connection;
using Heyworks.PocketShooter.Realtime.Service;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime
{
    public class GameEndedState : IGameState
    {
        private readonly GameStateController stateController;

        private IGameplayConnection Connection { get; }

        private INetworkService NetworkService { get; }

        public GameEndedState(GameStateController stateController, Room room)
        {
            stateController.NotNull();
            room.NotNull();

            this.stateController = stateController;
            Connection = room.Connection;
            NetworkService = room.NetworkService;
        }

        public void Update()
        {
            NetworkService.Receive();

            if (Connection.ConnectionState == ConnectionState.Disconnected)
            {
                stateController.SetCurrentState(new GameDisconnectedState(DisconnectReason.GameEnd));
            }
        }
    }
}