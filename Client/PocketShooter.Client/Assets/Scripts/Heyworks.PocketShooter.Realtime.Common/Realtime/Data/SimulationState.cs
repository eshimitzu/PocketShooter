using System;
using System.Text;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Represents the state of game simulation.
    /// </summary>
    public struct SimulationState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationState"/> struct.
        /// </summary>
        /// <param name="tick">The simulation tick.</param>
        /// <param name="serverInputBufferSize">The size of the server input buffer.</param>
        /// <param name="gameState">The game state.</param>
        public SimulationState(int tick, int serverInputBufferSize, GameState gameState)
        {
            Tick = tick;
            ServerInputBufferSize = serverInputBufferSize;
            GameState = gameState;
        }

        /// <summary>
        /// Gets the simulation tick.
        /// </summary>
        public readonly int Tick;

        /// <summary>
        /// Gets the size of the server input buffer.
        /// </summary>
        public readonly int ServerInputBufferSize;

        /// <summary>
        /// Gets a game state.
        /// </summary>
        public GameState GameState;

        /// <summary>
        /// Clones the simulation state and set tick value to new tick.
        /// </summary>
        /// <param name="newTick">The new tick to set.</param>
        // TODO: block copy with collections replacement into cyclic buffer, optimize clone via UnsafeClone and UnsafeLayout (handle PooledList<T>, T[], Span<T> where T : unamanged)
        public SimulationState CloneWithNewTick(int newTick) => new SimulationState(newTick, ServerInputBufferSize, GameState.Clone());

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>The fully qualified type name.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"tick: {Tick}");
            builder.AppendLine($"server input buffer size: {ServerInputBufferSize}");
            builder.AppendLine($"game state: {GameState}");

            return builder.ToString();
        }

        // TODO: call this on old item of sequence buffer to immediate return pooled arrays (to avoid GC by finalizers) - i.e. pass clean up into sequence buffer.
        internal void Clear()
        {
            if (GameState.Players != null)
            {
                for (var i = 0; i < GameState.Players.Count; i++)
                {
                    ref var player = ref GameState.Players.Span[i];
                    player.Damages?.Dispose();
                    player.Shots?.Dispose();
                    player.Heals?.Dispose();
                }

                GameState.Players?.Clear();
            }
        }
    }
}
