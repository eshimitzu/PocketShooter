using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Data;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Represents buffer with interpolation.
    /// </summary>
    public class InterpolatingStateProvider : IInterpolatingStateProvider<SimulationState>
    {
        private readonly int size;
        private readonly IRealtimeConfiguration realtimeConfiguration;
        // Collection of ticks of the states that was dropped and interpolated from real ones.
        private readonly Queue<int> interpolatedStates;
        private readonly CyclicSequenceBuffer<SimulationState> buffer;

        /// <summary>
        /// Gets the last tick in the buffer.
        /// </summary>
        public int LastTick { get; private set; }

        /// <inheritdoc cref="IInterpolatingStateProvider{T}"/>
        // NOTE: Tracking of interpolated states is used because we cannot apply those states onto local player.
        // Most of the time local player will correctly predict his state
        // but interpolated state is just a copy of previous state, what is wrong in many cases and will reconcile accurate prediction to wrong state.
        // But its ok for remote players, because we just apply server states for them.
        public bool IsInterpolated(int tick) => interpolatedStates.Contains(tick);

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatingStateProvider"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="realtimeConfiguration">The realtime parameters.</param>
        public InterpolatingStateProvider(IRealtimeConfiguration realtimeConfiguration)
        {
            this.size = realtimeConfiguration.StatesBufferSize;
            this.realtimeConfiguration = realtimeConfiguration;

            interpolatedStates = new Queue<int>(size);
            buffer = new CyclicSequenceBuffer<SimulationState>(size);
        }

        public void Start(int startWorldTick, in SimulationState initState)
        {
            LastTick = startWorldTick;
            buffer.Insert(startWorldTick, in initState);
        }

        /// <summary>
        /// Gets the state by tick.
        /// </summary>
        /// <param name="tick">The tick.</param>
        public ref SimulationState this[int tick] => ref buffer[tick];

        /// <summary>
        /// Gets the last state by tick.
        /// </summary>
        public ref SimulationState GetLastState => ref this[LastTick];

        /// <summary>
        /// Sets the state.
        /// </summary>
        /// <param name="tick">The next tick.</param>
        /// <param name="state">The nest state.</param>
        // TODO: v.shimkovich rewrite method as in StateProvider then maybe be inherit from StateProvider
        public bool TryInsert(int tick, in SimulationState state)
        {
            // TODO: v.shimkovich if you decide to not skip previous tick, you need to revalidate interpolated ticks
            // that are after new tick and before LastTick.
            if (tick <= LastTick)
            {
                SimulationLog.Log.LogWarning("Skipping the new state in the interpolating state buffer. Next tick {tick} is less then last tick {lastTick}.", tick, LastTick);
                return false;
            }

            while (interpolatedStates.Count > 0 && interpolatedStates.Peek() <= tick - size)
            {
                interpolatedStates.Dequeue();
            }

            SimulationState lastState = buffer[LastTick];
            SafeDispose(tick);
            if (!buffer.TryInsert(tick, state))
            {
                SimulationLog.Log.LogWarning("The tick {tick} was not inserted in the state buffer. Invalid next tick number.", tick);
                return false;
            }

            int newLastTick;
            if (tick >= LastTick + buffer.Size)
            {
                newLastTick = tick - buffer.Size;
                SimulationLog.Log.LogWarning("Next tick is far forward the last tick. Only the last {size} previous ticks will be duplicated. Next tick: {nextTick}, last tick: {newLastTick}.", buffer.Size, tick, LastTick);
            }
            else
            {
                newLastTick = LastTick;
            }

            // TODO: v.shimkovich "block copy"
            if (realtimeConfiguration.InterpolateLostTicks && buffer.ContainsKey(LastTick))
            {
                // Clone and insert last state to all states to be interpolated.
                for (int t = tick - 1; t > newLastTick; t--)
                {
                    var lastStateClone = lastState.CloneWithNewTick(t);
                    SafeDispose(t);
                    buffer.Insert(t, in lastStateClone);

                    interpolatedStates.Enqueue(t);
                }

                LerpStatesTransform(newLastTick, in lastState, in state, buffer);
            }
            else
            {
                // TODO: v.shimkovich "block copy"
                for (int t = tick - 1; t > newLastTick; t--)
                {
                    var lastStateClone = lastState.CloneWithNewTick(t);
                    SafeDispose(t);
                    buffer.Insert(t, in lastStateClone);
                }
            }

            /*
            // Debug interpolation
            if (tick - LastTick > 1 && buffer.ContainsKey(LastTick))
            {
                MLog.My.Log(LogLevel.Debug, LogCategory.Interpolation, "================Lerping state!!!================");

                var builder = new StringBuilder();
                for (int tick = LastTick; tick < tick + 1; tick++)
                {
                    builder.AppendLine(buffer.GetItem(tick).ToString());
                }

                MLog.My.Log(LogLevel.Debug, LogCategory.Interpolation, builder.ToString());
            }
            */

            LastTick = tick;

            return true;
        }

        public bool ContainsKey(int sequenceNo) => buffer.ContainsKey(sequenceNo);

        private static void LerpStatesTransform(
            int newLatTick,
            in SimulationState lastState,
            in SimulationState nextState,
            CyclicSequenceBuffer<SimulationState> statesBuffer)
        {
            int lastTick = lastState.Tick;
            var lastGameState = lastState.GameState;
            int lastLength = lastGameState.Players.Count();

            int nextTick = nextState.Tick;
            var nextGameState = nextState.GameState;
            int nextLength = nextGameState.Players.Count();

            // loop through all the PlayerState's in the last GameState
            for (var i = 0; i < lastLength; i++)
            {
                // and find corresponding PlayerState in the next GameState
                // FIXME v.shimkovich O(n2) complexity because players array structure is not defined. optimize
                PlayerState? nextPlayerState = null;
                for (var j = 0; j < nextLength; j++)
                {
                    if (lastGameState.Players[i].Id == nextGameState.Players[j].Id)
                    {
                        nextPlayerState = nextGameState.Players[j];
                        break;
                    }
                }

                // then lerp the PlayerState transform for every lost tick
                if (nextPlayerState is PlayerState bPlayer)
                {
                    for (int tick = newLatTick + 1; tick < nextTick; tick++)
                    {
                        float t = (float)(tick - lastTick) / (nextTick - lastTick);
                        // BUG: v.shimkovich can't use index "i" for every tick if players order not guaranteed.
                        ref PlayerState aPlayer = ref statesBuffer[tick].GameState.Players.Span[i];
                        ref var a = ref aPlayer.Transform;
                        ref var b = ref bPlayer.Transform;
                        a.Position.X = a.Position.X + ((b.Position.X - a.Position.X) * t);
                        a.Position.Y = a.Position.Y + ((b.Position.Y - a.Position.Y) * t);
                        a.Position.Z = a.Position.Z + ((b.Position.Z - a.Position.Z) * t);
                        a.Yaw = a.Yaw + ((b.Yaw - a.Yaw) * t);
                        a.Pitch = a.Pitch + ((b.Pitch - a.Pitch) * t);
                    }
                }
            }
        }

        public void Insert(int tick, in SimulationState item) => TryInsert(tick, in item);

        private void SafeDispose(int tick)
        {
            if (buffer.TryGetItemInPlace(tick, out SimulationState state))
            {
                state.GameState.Dispose();
            }
        }
    }
}