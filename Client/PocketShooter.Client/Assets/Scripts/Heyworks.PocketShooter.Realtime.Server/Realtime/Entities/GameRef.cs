using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Game state reference.
    /// </summary>
    public struct GameRef : IRef<GameState>
    {
        private readonly IRefIndex<GameState> refIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRef"/> struct.
        /// </summary>
        /// <param name="refIndex">Reference index.</param>
        /// <param name="tick">Game state tick.</param>
        public GameRef(IRefIndex<GameState> refIndex, int tick)
        {
            this.refIndex = refIndex;
            this.Tick = tick;
        }

        /// <summary>Gets the game state.</summary>
        public ref GameState Value => ref refIndex[Tick];

        /// <summary>
        /// Gets game state tick.
        /// </summary>
        public int Tick { get; }
    }
}