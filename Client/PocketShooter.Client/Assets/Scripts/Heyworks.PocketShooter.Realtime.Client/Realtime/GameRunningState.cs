using System;
using Heyworks.PocketShooter.Realtime.Connection;
using Heyworks.PocketShooter.Realtime.Entities;
using Microsoft.Extensions.Logging;
using UniRx;

namespace Heyworks.PocketShooter.Realtime
{
    public class GameRunningState : IGameState
    {
        private readonly GameStateController stateController;
        private readonly IDisposable subscription;

        public Room Room { get; }

        private IGameplayConnection Connection => Room.Connection;

        public GameRunningState(GameStateController stateController, Room room)
        {
            stateController.NotNull();
            room.NotNull();

            this.stateController = stateController;
            Room = room;
            subscription = Room.GameEnded.Subscribe(Room_GameEnded);

            // TODO: a.dezhurko create GameController here with factory (?)
        }

        public void Update()
        {
            if (Connection.ConnectionState == ConnectionState.Disconnected)
            {
                stateController.SetCurrentState(
                    Room.ManualDisconnectRequested
                        ? new GameDisconnectedState(DisconnectReason.ManualDisconnect)
                        : new GameDisconnectedState(DisconnectReason.InGameDisconnect));

                subscription.Dispose();
            }
            else
            {
                Room.Update();
            }
        }

        private void Room_GameEnded(Game game) => stateController.SetCurrentState(new GameEndedState(stateController, Room));
    }
}
