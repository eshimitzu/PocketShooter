using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

#pragma warning disable SA1649
namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    public struct Team1Ref : IRef<TeamState>
    {
        private IRef<GameState> gameStateRef;

        public Team1Ref(IRef<GameState> gameStateRef)
        {
            this.gameStateRef = gameStateRef;
        }

        public ref TeamState Value => ref gameStateRef.Value.Team1;
    }

    public struct Team2Ref : IRef<TeamState>
    {
        private IRef<GameState> gameStateRef;

        public Team2Ref(IRef<GameState> gameStateRef)
        {
            this.gameStateRef = gameStateRef;
        }

        public ref TeamState Value => ref gameStateRef.Value.Team2;
    }
}