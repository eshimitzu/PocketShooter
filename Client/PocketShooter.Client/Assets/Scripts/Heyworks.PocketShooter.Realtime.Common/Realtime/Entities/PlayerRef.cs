using System;
using System.Collections.Generic;
using System.Text;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Class to hold reference to player structure.
    /// </summary>
    public struct PlayerRef : IRef<PlayerState>
    {
        private readonly IRef<GameState> gameStateRef;
        private readonly byte playerIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerRef"/> struct.
        /// </summary>
        public PlayerRef(IRef<GameState> gameStateRef, byte playerIndex)
        {
            this.gameStateRef = gameStateRef;
            this.playerIndex = playerIndex;
        }

        /// <summary> Gets. </summary>
        public ref PlayerState Value => ref gameStateRef.Value.Players.Span[playerIndex];

        /// <summary> Gets. </summary>
        public EntityId Id => Value.Id;
    }
}