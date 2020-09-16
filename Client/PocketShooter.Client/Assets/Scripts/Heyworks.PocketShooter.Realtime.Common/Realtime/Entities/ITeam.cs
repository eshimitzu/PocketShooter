using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public interface ITeam : IEntity<TeamNo>
    {
        ref readonly TeamState State { get; }

        void ApplyState(IRef<TeamState> zoneStateRef);
    }
}