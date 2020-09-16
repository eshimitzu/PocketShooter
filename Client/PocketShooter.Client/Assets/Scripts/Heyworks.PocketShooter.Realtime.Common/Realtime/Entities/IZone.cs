using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public interface IZone : IEntity<byte>
    {
        ref readonly ZoneState State { get; }

        DominationZoneInfo ZoneInfo { get; }

        void ApplyState(IRef<ZoneState> zoneStateRef);

        bool IsInside(IPlayer player);
    }
}