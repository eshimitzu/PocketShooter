using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Service;
using Microsoft.Extensions.Logging;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents class form managing and adjusting tickers.
    /// The main responsibility of the class is to synchronize simulation and world ticks with server ticks.
    /// </summary>
    public sealed class TimeManager : ITickProvider
    {
        private readonly Ticker clientTicker;
        private readonly Telemetry inputBufferSizeOnServerTelemetry;

        private readonly Ticker worldTicker;
        private readonly Telemetry worldStatesBufferSizeTelemetry;

        private readonly IRttProvider rttProvider;
        private readonly IInterpolatingStateProvider<SimulationState> stateProvider;
        private readonly IRealtimeConfiguration realtimeConfig;

        private int simulationTicksCountOnLastUpdate;
        private int worldTicksCountOnLastUpdate;

        /// <summary>
        /// Gets the current simulating tick.
        /// </summary>
        public int Tick => clientTicker.Current;

        /// <summary>
        /// Gets the world tick.
        /// </summary>
        public int WorldTick => worldTicker.Current;

        /// <summary>
        /// Gets the world tick fraction.
        /// </summary>
        public float WorldTickFraction => worldTicker.Fraction;

        /// <inheritdoc cref="ITickProvider"/>
        public bool WorldTicked { get; private set; }

        /// <inheritdoc cref="ITickProvider"/>
        public int LastUsedWorldTick { get; private set; }

        public float ElapsedWorldTicks(float tick) => worldTicker.ElapsedTicks(tick);

        // TODO: a.dezhurko Move to stats
        #region [Debug Only]

        /// <summary>
        /// Gets the target size of the client commands buffer on server.
        /// </summary>
        public double TargetInputBufferSizeOnServer { get; private set; }

        /// <summary>
        /// Gets the target size of the world states buffer on client.
        /// </summary>
        public double TargetWorldStatesBufferSize { get; private set; }

        /// <summary>
        /// Gets the mean size of the client commands buffer on server.
        /// </summary>
        public double InputBufferSizeOnServerMean => inputBufferSizeOnServerTelemetry.Mean;

        /// <summary>
        /// Gets the size variance of the client commands buffer on server.
        /// </summary>
        public double InputBufferSizeOnServerVariance => inputBufferSizeOnServerTelemetry.Variance;

        /// <summary>
        /// Gets the mean size of the world states buffer on client.
        /// </summary>
        public double WorldStatesBufferSizeMean => worldStatesBufferSizeTelemetry.Mean;

        /// <summary>
        /// Gets the size variance of the world states buffer on client.
        /// </summary>
        public double WorldStatesBufferSizeVariance => worldStatesBufferSizeTelemetry.Variance;

        public int ClientTickerAdjustment => clientTicker.Adjustment;

        public int WorldTickerAdjustment => worldTicker.Adjustment;

        #endregion

        private int LastReceivedWorldTick => stateProvider.LastTick;

        private int InputBufferSizeOnServer => stateProvider.GetLastState.ServerInputBufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeManager" /> class.
        /// </summary>
        /// <param name="clientTicker">The client ticker.</param>
        /// <param name="worldTicker">The world ticker.</param>
        /// <param name="rttProvider">The RTT provider.</param>
        /// <param name="stateProvider">The state provider.</param>
        /// <param name="realtimeConfig">The realtime configuration.</param>
        public TimeManager(
            Ticker clientTicker,
            Ticker worldTicker,
            IRttProvider rttProvider,
            IInterpolatingStateProvider<SimulationState> stateProvider,
            IRealtimeConfiguration realtimeConfig)
        {
            this.rttProvider = rttProvider;
            this.stateProvider = stateProvider;
            this.realtimeConfig = realtimeConfig;

            // TODO: v.shimkovich move checks to config init
            Assert.IsTrue(realtimeConfig.TickIntervalMs > 0, $"Invalid TickInterval value in {nameof(realtimeConfig)}.");
            Assert.IsTrue(realtimeConfig.MaximumTickerSkewInMilliseconds > 0, $"Invalid MaximumTickerSkewInMilliseconds value in {nameof(realtimeConfig)}.");
            Assert.IsTrue(realtimeConfig.MaximumTickerSkewInTicks > 0, $"Invalid MaximumTickerSkewInTicks value in {nameof(realtimeConfig)}.");
            Assert.IsTrue(realtimeConfig.MaximumTickerSkewInTicks < realtimeConfig.StatesBufferSize / 2, "MaximumTickerSkewInTicks should be less than StatesBufferSize / 2");

            this.clientTicker = clientTicker;
            inputBufferSizeOnServerTelemetry = new Telemetry();

            this.worldTicker = worldTicker;
            worldStatesBufferSizeTelemetry = new Telemetry();
        }

        /// <summary>
        /// Starts simulation with the specified start world tick.
        /// </summary>
        /// <param name="startWorldTick">The start world tick.</param>
        /// <param name="serverTimeStamp">The server time stamp.</param>
        public void Start(int startWorldTick, int serverTimeStamp)
        {
            int serverDelayMs;
            // NOTE: ServerTimeMs property sometimes (high ping, drops, no 100% repro) not set and returns 0  (see implementation).
            // In this case we can use rtt/2 which (based on hand tests) returns correct value. It is probably can be always used instead of ServerTimeMs.
            if (rttProvider.ServerTimeMs == 0)
            {
                var rtt = rttProvider.LastRoundTripTimeMs;
                NetLog.Log.LogWarning("Server time is unavailable. Using LastRtt / 2. Rtt = {rtt}", rtt);

                serverDelayMs = rtt / 2;
            }
            else
            {
                serverDelayMs = rttProvider.ServerTimeMs - serverTimeStamp;
            }

            var initialTickersOffset = TimeUtils.CalculateInitialTickersOffset(
                serverDelayMs,
                realtimeConfig.TickIntervalMs,
                realtimeConfig.MaximumTickerSkewInTicks);

            worldTicker.Start(startWorldTick, serverDelayMs, -initialTickersOffset);
            clientTicker.Start(startWorldTick, serverDelayMs, initialTickersOffset);
            SaveLastUsedWorldTick();

            /*
            MLog.Metrics.Log.LogTrace($", Adj, " +
                                      $"SizeBefore, SizeAfter, " +
                                      $"Mean, " +
                                      $"Variance, " +
                                      $"Target, Min, Max," +
                                      $"Last, " +
                                      $"CurrentBefore, CurrentAfter,");


            MLog.Metrics.Log.LogTrace($", Adj, " +
                                      $"Size, " +
                                      $"Mean, " +
                                      $"Variance, " +
                                      $"Target, Min, Max,");
            */
        }

        /// <summary>
        /// Calculates world and local simulation ticks passed from the last update.
        /// </summary>
        public (bool worldTicked, bool clientTicked) UpdateTicks()
        {
            UpdateWorldStatesBufferSizeTelemetry(GetActualWorldStatesBufferSize());
            AdjustWorldTick();
            worldTicksCountOnLastUpdate = worldTicker.Tick();

            UpdateInputBufferSizeOnServerTelemetry(InputBufferSizeOnServer);
            AdjustSimulationTick();
            simulationTicksCountOnLastUpdate = clientTicker.Tick();

            var worldTicked = worldTicksCountOnLastUpdate > 0;
            var clientTicked = simulationTicksCountOnLastUpdate > 0;

            if (Tick >= WorldTick + realtimeConfig.StatesBufferSize)
            {
                throw new ArgumentException(
                    $"Client buffer tick({Tick}) is further than {realtimeConfig.StatesBufferSize} from World buffer tick({WorldTick}).");
            }

            if (WorldTick > Tick)
            {
                SimulationLog.Log.LogWarning(
                    "World tick({wTick}) cannot be further than client tick({tick}). Skipping world update",
                    WorldTick,
                    Tick);

                worldTicked = false;
            }

            WorldTicked = worldTicked;

            return (worldTicked, clientTicked);
        }

        public void SaveLastUsedWorldTick()
        {
            LastUsedWorldTick = WorldTick;
        }

        private void UpdateInputBufferSizeOnServerTelemetry(int inputBufferSizeOnServer)
        {
            inputBufferSizeOnServerTelemetry.Update(inputBufferSizeOnServer);
        }

        private void UpdateWorldStatesBufferSizeTelemetry(double worldStatesBufferSize)
        {
            worldStatesBufferSizeTelemetry.Update(worldStatesBufferSize);
        }

        private void AdjustWorldTick()
        {
            /*
            var beforeWorldStatesBufferSize = GetActualWorldStatesBufferSize();
            var beforeTick = worldTicker.ActualCurrent;
            */

            if (!IsValidWorldTick())
            {
                return;
            }

            var adjustment = TimeUtils.CalculateWorldStopwatchAdjustment(
                realtimeConfig.TickIntervalMs,
                GetActualWorldStatesBufferSize(),
                worldStatesBufferSizeTelemetry.Mean,
                worldStatesBufferSizeTelemetry.Variance,
                realtimeConfig.MaximumTickerSkewInMilliseconds,
                out var stats);

            worldTicker.Adjust(adjustment);

            TargetWorldStatesBufferSize = stats.Item1;

            /*
                MLog.Metrics.Log.LogTrace($", {adjustment / tickInterval}, " +
                                          $"{beforeWorldStatesBufferSize}, {GetActualWorldStatesBufferSize()}, " +
                                          $"{worldStatesBufferSizeTelemetry.Mean}, " +
                                          $"{worldStatesBufferSizeTelemetry.Variance}, " +
                                          $"{stats.Item1}, {stats.Item2}, {stats.Item3}," +
                                          $"{bufferStats.LastReceivedWorldTick}, " +
                                          $"{beforeTick}, {worldTicker.ActualCurrent},");
                                          */
        }

        private bool IsValidWorldTick()
        {
            return worldTicker.Current < LastReceivedWorldTick + realtimeConfig.MaximumTickerSkewInTicks &&
                   worldTicker.Current > LastReceivedWorldTick - realtimeConfig.MaximumTickerSkewInTicks;
        }

        private void AdjustSimulationTick()
        {
            if (!IsValidSimulationTick())
            {
                return;
            }

            var adjustment = TimeUtils.CalculateClientStopwatchAdjustment(
                rttProvider.LastRoundTripTimeMs,
                realtimeConfig.TickIntervalMs,
                // TODO: v.shimkovich Point of improvement. if bufer size is -MANY because of lag/freeze,
                // when game resume ticker will jump to correct position, but adjuster will try to apply big adjustments,
                // to the moment when server receive and send us positive buffer size(about rtt time).
                // We can handle this situation separately not to do redundant adjustments.
                InputBufferSizeOnServer,
                inputBufferSizeOnServerTelemetry.Mean,
                inputBufferSizeOnServerTelemetry.Variance,
                realtimeConfig.MaximumTickerSkewInMilliseconds,
                out var stats);

            clientTicker.Adjust(adjustment);

            TargetInputBufferSizeOnServer = stats.Item1;

            /*
                MLog.Metrics.Log.LogTrace($", {10 * adjustment / tickInterval}, " +
                                          $"{bufferStats.InputBufferSizeOnServer}, " +
                                          $"{inputBufferSizeOnServerTelemetry.Mean}, " +
                                          $"{inputBufferSizeOnServerTelemetry.Variance}, " +
                                          $"{stats.Item1}, {stats.Item2}, {stats.Item3},");
                                          */
        }

        private bool IsValidSimulationTick()
        {
            var rtt = rttProvider.LastRoundTripTimeMs / realtimeConfig.TickIntervalMs;
            return clientTicker.ActualCurrent - rtt < LastReceivedWorldTick + realtimeConfig.MaximumTickerSkewInTicks &&
                   clientTicker.ActualCurrent - rtt > LastReceivedWorldTick - realtimeConfig.MaximumTickerSkewInTicks;
        }

        private float GetActualWorldStatesBufferSize()
        {
            return LastReceivedWorldTick - worldTicker.ActualCurrent;
        }

        #region Debug Only

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void RandomizeWorldTick()
        {
            worldTicker.Reset(worldTicker.Current + Random.Range(-100000, 100000));
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void RandomizeSimulationTick()
        {
            worldTicker.Reset(clientTicker.Current + Random.Range(-100000, 100000));
        }

        #endregion
    }
}
