using System;
using System.Collections.Generic;
using System.Text;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    class DummyGameRef : IRef<GameState>
    {
        private GameState state;

        public DummyGameRef()
        {
            ZoneState[] zones =
            {
                new ZoneState(0, TeamNo.None, 0, false),
                new ZoneState(1, TeamNo.None, 0, false),
                new ZoneState(2, TeamNo.None, 0, false)
            };
            state = new GameState(default,default, zones, new PooledList<PlayerState>());            
        }

        public ref GameState Value => ref state;
    }
}
