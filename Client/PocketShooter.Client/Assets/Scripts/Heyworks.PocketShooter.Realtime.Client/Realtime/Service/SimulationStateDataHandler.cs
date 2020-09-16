using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Connection;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.Realtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Service
{
    public class SimulationStateDataHandler : IDataHandler
    {
        private readonly IStateProvider<SimulationState> simulationStateProvider;
        private readonly IDeltaDataSerializer<SimulationState> serializer;
        private readonly IRealtimeConfiguration realtimeConfiguration;
        private readonly ITickProvider tickProvider;

        public SimulationStateDataHandler(
            IStateProvider<SimulationState> simulationStateProvider,
            IDeltaDataSerializer<SimulationState> serializer,
            IRealtimeConfiguration realtimeConfiguration,
            ITickProvider tickProvider)
        {
            this.simulationStateProvider = simulationStateProvider;
            this.serializer = serializer;
            this.realtimeConfiguration = realtimeConfiguration;
            this.tickProvider = tickProvider;
        }

        public void Handle(NetworkData data)
        {
            // TODO: d.lahoda: read buffer here for header tick, and pass buffer into
            SimulationState? state = serializer.Deserialize(data.Data);

            if (state.HasValue)
            {
                if (data.Data.Length > PhotonConnectionConfiguration.GuarantedMTU)
                {
                    NetLog.Warning("Received {bytesCount} bytes of state from network at {serverTick} server tick", data.Data.Length, state.Value.Tick);
                }

                // NOTE: This check goal is to prevent new states from overwrite the statesBuffer when simulation will not use this states.
                // This is bad because buffer will be overwritten, but game still will reference old tick, which is not in thw buffer anymore.
                // This is possible (v.shimkovich knows another example) if ping is too high,
                // so that world ticker *cannot adjust* so much and WorldTick will point in front of states buffer.
                if (state.Value.Tick >= tickProvider.LastUsedWorldTick + realtimeConfiguration.StatesBufferSize)
                {
                    // 5% of time with bad internet
                    if (!(// ISSUE: v.shimkovich TESTS REQUIRED
                        // there is assumption that next statement is equivalent to
                        // {
                        //     stateProvider.Insert(state.Value); // that is done right after check
                        //     if (stateProvider.ContainsKey(tickProvider.WorldTick)) {...} // that is checked in NetworkSimulation right after NetworkService.Receive()
                        // }
                        // it relies on how InterpolatingStateProvider.Insert() works.
                        // THIS SHOULD BE TESTED, while not, debug static field is here.
                        state.Value.Tick > simulationStateProvider.LastTick
                        && tickProvider.WorldTick <= state.Value.Tick
                        && tickProvider.WorldTick > state.Value.Tick - realtimeConfiguration.StatesBufferSize

                        && tickProvider.WorldTicked))
                    {
                        // 95% of time of that case
                        SimulationLog.Log.LogWarning(
                            "New state(tick {t}) will not be used by simulation. Skipping it. Current world tick is {wt}, size is {size}",
                            state.Value.Tick,
                            tickProvider.WorldTick,
                            realtimeConfiguration.StatesBufferSize);
                        return;
                    }
                }


                simulationStateProvider.Insert(state.Value.Tick, state.Value);
            }
        }
    }
}