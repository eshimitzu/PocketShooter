using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Service;
using Microsoft.Extensions.Logging;
using UniRx;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents network simulation implementation. Is responsible for the main simulation cycle and tickers adjusting.
    /// </summary>
    public sealed class NetworkSimulation : INetworkSimulation, IBuffersStatsProvider
    {
        private readonly INetworkService networkService;
        private readonly IInterpolatingStateProvider<SimulationState> simulationStateProvider;
        private readonly IRealtimeConfiguration realtimeConfiguration;
        private readonly TimeManager timeManager;
        private readonly List<ClientSimulation> playerSimulations = new List<ClientSimulation>();
        private readonly Subject<TickEvent> simulationTick = new Subject<TickEvent>();

        /// <summary>
        /// Gets the last received world tick.
        /// </summary>
        public int LastReceivedWorldTick => simulationStateProvider.LastTick;

        /// <summary>
        /// Gets the last size value of the client commands buffer on server.
        /// </summary>
        public int ServerInputBufferSize => simulationStateProvider.GetLastState.ServerInputBufferSize;

        /// <summary>
        /// Gets the current size value of the world states buffer on client.
        /// </summary>
        public int WorldStatesBufferSize => LastReceivedWorldTick - timeManager.WorldTick;

        /// <summary>
        /// Gets the tick provider.
        /// </summary>
        public ITickProvider TickProvider => timeManager;

        /// <summary>
        /// Gets observable that fires when simulation ticks. Has parameter number of ticks passed since last tick.
        /// </summary>
        public IObservable<TickEvent> SimulationTick => simulationTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkSimulation"/> class.
        /// </summary>
        /// <param name="networkService">The network service.</param>
        /// <param name="simulationStateProvider">The simulation state provider.</param>
        /// <param name="realtimeConfiguration">The realtime parameters.</param>
        /// <param name="timeManager">The time manager.</param>
        public NetworkSimulation(
            INetworkService networkService,
            IInterpolatingStateProvider<SimulationState> simulationStateProvider,
            IRealtimeConfiguration realtimeConfiguration,
            TimeManager timeManager)
        {
            this.networkService = networkService;
            this.simulationStateProvider = simulationStateProvider;
            this.realtimeConfiguration = realtimeConfiguration;
            this.timeManager = timeManager;
        }

        /// <summary>
        /// Starts simulation with the specified start world tick.
        /// </summary>
        /// <param name="startWorldTick">The start world tick.</param>
        /// <param name="serverTimeStamp">The server time stamp.</param>
        /// <param name="initState">Initial state.</param>
        public void Start(int startWorldTick, int serverTimeStamp, in SimulationState initState)
        {
            timeManager.Start(startWorldTick, serverTimeStamp);
            simulationStateProvider.Start(startWorldTick, in initState);
            networkService.Receive();
        }

        /// <summary>
        /// Updates the simulation.
        /// </summary>
        public void Update()
        {
            (bool worldTicked, bool clientTicked) = timeManager.UpdateTicks();

            SimulationLog.Log.LogTrace("Current world tick is {wt}, local client tick is {lt}", timeManager.WorldTick, timeManager.Tick);

            // new states arrive here
            networkService.Receive();

            if (clientTicked)
            {
                PrepareSimulationsForNextTick();
            }

            if (worldTicked)
            {
                if (!simulationStateProvider.ContainsKey(timeManager.WorldTick))
                {
                    SimulationLog.Log.LogWarning(
                        "Current world tick({wt}) not found in states buffer. Skipping world update. Latest world tick is {lastWt}, size is {size}", timeManager.WorldTick, LastReceivedWorldTick, realtimeConfiguration.StatesBufferSize);
                }
                else
                {
                    UpdateWorld();
                }
            }

            if (clientTicked)
            {
                UpdateSimulations();
            }

            foreach (ClientSimulation simulation in playerSimulations)
            {
                simulation.ProcessEvents();
            }

            networkService.Send();
        }

        /// <summary>
        /// Adds the simulation.
        /// </summary>
        /// <param name="simulation">The simulation.</param>
        public void AddSimulation(ClientSimulation simulation)
        {
            playerSimulations.Add(simulation);
        }

        private void PrepareSimulationsForNextTick()
        {
            foreach (ClientSimulation simulation in playerSimulations)
            {
                if (simulation.Game.LocalPlayer != null)
                {
                    simulation.CreatePlayerStateForTheSimulationTick(timeManager.Tick);
                }
            }
        }

        private void UpdateWorld()
        {
            if (simulationStateProvider.LastTick
                >= timeManager.LastUsedWorldTick + realtimeConfiguration.StatesBufferSize)
            {
                SimulationLog.Log.LogWarning(
                    "Previously simulated world tick({lastWTick}) is too old and out of the buffer(size is {size}, last tick is {lastTick}). Clearing the world",
                    timeManager.LastUsedWorldTick,
                    realtimeConfiguration.StatesBufferSize,
                    simulationStateProvider.LastTick);
                ClearWorld();
            }

            UpdateWorldState(timeManager.WorldTick);
            timeManager.SaveLastUsedWorldTick();
        }

        private void ClearWorld()
        {
            foreach (var simulation in playerSimulations)
            {
                simulation.Game.ClearWorld();
            }
        }

        private void UpdateWorldState(int tickToProcess)
        {
            bool isInterpolated = simulationStateProvider.IsInterpolated(tickToProcess);
            var serverStateAtServerTick = new SimulationRef(simulationStateProvider, tickToProcess, isInterpolated);

            foreach (var simulation in playerSimulations)
            {
                simulation.UpdateWorldState(serverStateAtServerTick, tickToProcess, timeManager.Tick);
            }
        }

        private void UpdateSimulations()
        {
            simulationTick.OnNext(default);

            var possiblyStillAcceptedTick = ServerInputBufferSize < 0 ? timeManager.Tick : timeManager.Tick - ServerInputBufferSize;

            // https://heyworks.atlassian.net/browse/PSH-448
            // TODO: Send bitmask of received tick baseliend on LastReceivedWorldTick
            // TODO: bitmask is must if doing delta (not diff) - because server will resend delta until got into client
            // TODO: if at least one delta will not get in buffer size cycle - disconnect (or we need all events to be resend, not only killed)
            networkService.AddPing(new SimulationMetaCommandData(timeManager.Tick, LastReceivedWorldTick), possiblyStillAcceptedTick);

            foreach (ClientSimulation simulation in playerSimulations)
            {
                if (simulation.Game.LocalPlayer != null)
                {
                    simulation.Simulate(timeManager.Tick);
                }
            }
        }
    }
}