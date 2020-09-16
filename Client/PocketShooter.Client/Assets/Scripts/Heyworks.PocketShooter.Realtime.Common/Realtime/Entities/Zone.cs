using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Game match zone.
    /// </summary>
    public class Zone : IZone
    {
        private IRef<ZoneState> zoneStateRef;

        public Zone(IRef<ZoneState> zoneStateRef, DominationZoneInfo zoneInfo)
        {
            this.zoneStateRef = zoneStateRef;
            this.ZoneInfo = zoneInfo;
        }

        /// <summary>
        /// Applies new zone state.
        /// </summary>
        /// <param name="zoneStateRef">Zone state to apply.</param>
        public void ApplyState(IRef<ZoneState> zoneStateRef)
        {
            this.zoneStateRef = zoneStateRef;
        }

        /// <summary>
        /// Gets zone id.
        /// </summary>
        /// <value>Zone id.</value>
        public byte Id => zoneStateRef.Value.Id;

        ref readonly ZoneState IZone.State => ref zoneStateRef.Value;

        public ref ZoneState State => ref zoneStateRef.Value;

        public DominationZoneInfo ZoneInfo { get; }

        /// <summary>
        /// Indicates whether the player inside the zone or not.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>If the player inside the zone.</returns>
        public bool IsInside(IPlayer player) =>
            player.IsAlive && player.Transform.IsInside(ZoneInfo.X, ZoneInfo.Z, ZoneInfo.RadiusSqr);
    }
}