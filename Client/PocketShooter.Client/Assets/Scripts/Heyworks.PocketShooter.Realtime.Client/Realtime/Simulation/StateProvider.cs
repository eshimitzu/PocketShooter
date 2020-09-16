using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public class StateProvider<T> : ILocalStateProvider<T>
    {
        private readonly int size;
        private CyclicSequenceBuffer<T> buffer;

        public int LastTick { get; private set; }

        public StateProvider(int size)
        {
            this.size = size;

            buffer = new CyclicSequenceBuffer<T>(size);
        }

        public void Start(int startWorldTick, in T initState)
        {
            LastTick = startWorldTick;
            buffer.Insert(startWorldTick, in initState);
        }

        /// <summary>
        /// Gets value reference for the tick.
        /// </summary>
        /// <returns>Value referene.</returns>
        public ref T this[int tick] => ref buffer[tick];

        public ref T GetLastState => ref this[LastTick];

        public void Insert(int tick, in T state) => TryInsert(tick, state);

        public bool TryInsert(int tick, in T state)
        {
            if (tick <= LastTick)
            {
                SimulationLog.Log.LogWarning("Skipping the new state in the state buffer. Next tick {nextTick} is less then the last tick {lastTick}.", tick, LastTick);
                return false;
            }

            // TODO: v.shimkovich redundant?
            if (!buffer.ContainsKey(LastTick))
            {
                return false;
            }

            T prevState = buffer[LastTick];
            if (!buffer.TryInsert(tick, state))
            {
                SimulationLog.Log.LogWarning("The tick {nextTick} was not inserted in the state buffer. Invalid next tick number.", tick);
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
            for (int i = tick - 1; i > newLastTick; i--)
            {
                // TODO: v.shimkovich !!! cant just copy PlayerState, it has lists.
                buffer.Insert(i, in prevState);
            }

            LastTick = tick;

            return true;
        }

        public bool ContainsKey(int sequenceNo)
        {
            return buffer.ContainsKey(sequenceNo);
        }

        public bool TryEnsureRef(int tick)
        {
            if (tick <= LastTick)
            {
                SimulationLog.Log.LogWarning("Skipping the new state in the state buffer. Next tick {nextTick} is less then the last tick {lastTick}.", tick, LastTick);
                return false;
            }

            //TODO: may optimize not to create default on stack, but allocated directly in array
            return buffer.TryInsert(tick, default);
        }
    }
}