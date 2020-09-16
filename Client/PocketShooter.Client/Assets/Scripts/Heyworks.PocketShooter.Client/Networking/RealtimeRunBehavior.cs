using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Communication;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Gameplay;
using Heyworks.PocketShooter.Modules.GameEnvironment;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Connection;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.UI.Core;
using Microsoft.Extensions.Logging;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Networking
{
    public class RealtimeRunBehavior : MonoBehaviour
    {
        [SerializeField]
        private ZoneView zonePrefab = null;

        [Inject]
        private TrooperCreator trooperCreator = null;
        [Inject]
        private ScreensController screensController = null;
        [Inject]
        private IRealtimeConfiguration realtimeConfiguration;
        [Inject]
        private MapSceneManager mapSceneManager;
        [Inject]
        private Main main;

        private GameStateController stateController;

        #region [Debug Only]
#pragma warning disable SA1300, SA1516, SA1309        // ReSharper disable InconsistentNaming
        internal IBuffersStatsProvider _BuffersStatsProvider => (IBuffersStatsProvider)roomController?.NetworkSimulationController;
        internal TimeManager _TimeManager => (TimeManager)roomController?.NetworkSimulationController?.TickProvider;
        internal IClientSimulation _LocalPlayerSimulation => roomController?.LocalPlayerSimulation;
        internal Game _Game => roomController?.Game;
        internal RoomController _RoomController => roomController;
        internal ServerAddress _CurrentRealtimeServerAddress;
#pragma warning restore SA1300, SA1516, SA1309        // ReSharper restore InconsistentNaming
        #endregion

        private RoomController roomController;

        internal event Action Connected;
        internal event Action<DisconnectReason> Disconnected;

        private void Awake()
        {
            stateController = new GameStateController();
            stateController.StateChanged += StateControllerStateChanged;
        }

        private void Update() => stateController.UpdateCurrentState();

        /// <summary>
        /// Connect the specified server.
        /// </summary>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="playerId">The player id.</param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        internal void Connect(ServerAddress serverAddress, PlayerId playerId)
        {
            _CurrentRealtimeServerAddress = serverAddress;

            var connection = PhotonConnection.CreateDefault(serverAddress.Address);

            var entryState = new GameEntryState(
                stateController,
                connection,
                realtimeConfiguration,
                playerId);

            stateController.SetCurrentState(entryState);
        }

        internal void ManualDisconnect() => stateController.SetCurrentState(new GameDisconnectedState(DisconnectReason.ManualDisconnect));

        private void StateControllerStateChanged(IGameState state)
        {
            switch (state)
            {
                case GameDisconnectedState disconnectedState:
                    NetLog.Trace("State changed to 'GameDisconnectedState'. Clean up resources.");
                    roomController?.Dispose();
                    roomController = null;
                    Disconnected?.Invoke(disconnectedState.DisconnectReason);
                    break;
                case GameEntryState _:
                    NetLog.Trace("State changed to 'GameEntryState'.");
                    break;
                case GameRunningState runningState:
                    NetLog.Trace("State changed to 'GameRunningState'. Creating game controller.");
                    Connected?.Invoke();

                    // TODO: v.filippov Create inside roomcontoller using factories.
                    var zonesManager = new ZonesManager(zonePrefab);
                    var characterManager = new CharacterManager(trooperCreator);
                    var botManager = new BotManager(trooperCreator);
                    roomController = new RoomController(runningState.Room, zonesManager, characterManager, botManager, screensController, mapSceneManager, main.MetaGame);
                    break;
                case GameEndedState _:
                    NetLog.Trace("State changed to '{state}'.", nameof(GameEndedState));
                    break;
            }
        }
    }
}